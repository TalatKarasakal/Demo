using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings (Classic)")]
    public float moveSpeed = 8f;
    public float acceleration = 20f;
    public float deceleration = 15f;

    [Header("Movement Settings (Dash Jump)")]
    public float minDashPower = 5f;   // Kısa basçek gücü
    public float maxDashPower = 25f;  // Uzun basılı tutma (maksimum) gücü
    public float dashChargeRate = 40f;// Saniyede dolan güç
    public float dashFriction = 10f;  // Zıpladıktan sonra kayarak durması için sürtünme

    [Header("Hit Settings")]
    public float hitForce = 10f;
    public float hitRange = 1f;

    [Header("Boundaries")]
    public float boundaryLeft = -7f;
    public float boundaryRight = 7f;

    private Rigidbody2D rb;
    private GameSettings.MovementMode currentMode;

    private Vector3 originalScale;

    // Dash (Zıplama) Değişkenleri
    private float currentCharge = 0f;
    private bool isCharging = false;
    private float chargeDirection = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("PlayerController: Rigidbody2D bulunamadı!");
        else rb.freezeRotation = true;
    }

    void Start()
    {
        // Menüden seçilen modu al
        currentMode = GameSettings.SelectedMovementMode;
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Seçilen moda göre hareket sistemini çalıştır
        if (currentMode == GameSettings.MovementMode.Classic)
        {
            HandleClassicMovement();
        }
        else if (currentMode == GameSettings.MovementMode.DashJump)
        {
            HandleDashJumpMovement();
        }

        HandleHit();
        ClampPosition();
    }

    void HandleClassicMovement()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input = +1f;

        float currentX = rb.linearVelocity.x;
        float desiredX = input * moveSpeed;
        float diff = desiredX - currentX;
        float rate = (Mathf.Approximately(desiredX, 0f) ? deceleration : acceleration);
        float change = diff * rate * Time.deltaTime;

        rb.linearVelocity = new Vector2(currentX + change, rb.linearVelocity.y);
    }

    void HandleDashJumpMovement()
    {
        bool leftDown = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        bool rightDown = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        bool leftUp = Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow);
        bool rightUp = Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow);

        // 1. Şarja Başlama (Tuşa ilk basıldığı an)
        if (!isCharging)
        {
            if (leftDown) { isCharging = true; chargeDirection = -1f; currentCharge = minDashPower; }
            else if (rightDown) { isCharging = true; chargeDirection = 1f; currentCharge = minDashPower; }
        }

        // 2. Şarj Olurken (Tuşa basılı tutulduğu sürece)
        if (isCharging)
        {
            currentCharge += dashChargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, minDashPower, maxDashPower);

            // Şarj olurken raket olduğu yerde dursun (kaymasın)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            // 3. Zıplama Anı (Tuş bırakıldığında)
            if ((chargeDirection == -1f && leftUp) || (chargeDirection == 1f && rightUp))
            {
                ExecuteDashJump();
            }
        }
        else
        {
            // Şarjda değilken (havada zıplamış kayıyorken) sürtünmeyle yavaşla
            float currentX = rb.linearVelocity.x;
            if (Mathf.Abs(currentX) > 0.1f)
            {
                float change = -Mathf.Sign(currentX) * dashFriction * Time.deltaTime;
                if (Mathf.Sign(currentX + change) != Mathf.Sign(currentX))
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Yön değişirse durdur
                else
                    rb.linearVelocity = new Vector2(currentX + change, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    void ExecuteDashJump()
    {
        isCharging = false;
        // Şarj edilen güçle hedef yöne fırlat
        rb.linearVelocity = new Vector2(chargeDirection * currentCharge, rb.linearVelocity.y);
        currentCharge = 0f;
        chargeDirection = 0f;
    }

    void HandleHit()
    {
        if (Input.GetKeyDown(KeyCode.Space)) TryHit();
    }

    void TryHit()
    {
        var ball = GameObject.FindWithTag("Ball");
        if (ball == null) return;
        if (Vector2.Distance(transform.position, ball.transform.position) > hitRange) return;

        var bRb = ball.GetComponent<Rigidbody2D>();
        if (bRb == null) return;

        Vector2 dir = new Vector2(rb.linearVelocity.x * 0.3f + Random.Range(-0.2f, 0.2f), 1f).normalized;
        bRb.linearVelocity = dir * hitForce;
    }

    void ClampPosition()
    {
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, boundaryLeft, boundaryRight);
        transform.position = p;
    }

    public void ResetPlayer()
    {
        transform.position = new Vector3(0f, transform.position.y, 0f);
        if (rb != null) rb.linearVelocity = Vector2.zero;
        isCharging = false;
        currentCharge = 0f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }

    // --- GÜÇLENDİRME (POWER-UP) FONKSİYONLARI ---

    public void ActivateSizeUp(float duration)
    {
        StartCoroutine(SizeUpRoutine(duration));
    }

    private IEnumerator SizeUpRoutine(float duration)
    {
        // Y ekseninde (uzunluk) raketi 1.5 kat büyüt
        transform.localScale = new Vector3(originalScale.x, originalScale.y * 1.5f, originalScale.z);
        yield return new WaitForSeconds(duration);
        // Süre bitince eski haline dön
        transform.localScale = originalScale;
    }

    public void ActivateSlowDown(float duration)
    {
        StartCoroutine(SlowDownRoutine(duration));
    }

    private IEnumerator SlowDownRoutine(float duration)
    {
        // Mevcut hızları hafızaya al
        float origMove = moveSpeed;
        float origCharge = dashChargeRate;
        float origMax = maxDashPower;

        // Hızları ve gücü yarı yarıya düşür
        moveSpeed *= 0.5f;
        dashChargeRate *= 0.5f;
        maxDashPower *= 0.5f;

        yield return new WaitForSeconds(duration);

        // Süre bitince hızları eski haline döndür
        moveSpeed = origMove;
        dashChargeRate = origCharge;
        maxDashPower = origMax;
    }
}