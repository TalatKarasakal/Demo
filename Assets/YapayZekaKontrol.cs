using UnityEngine;
using System.Collections;

public class YapayZekaKontrol : MonoBehaviour
{
    public enum DifficultyLevel { Easy, Medium, Hard }

    [Header("Zorluk Seviyesi")]
    public DifficultyLevel currentDifficulty = DifficultyLevel.Medium;

    [Header("Hareket Ayarları (Classic)")]
    public float moveSpeed = 6f;
    public float boundaryLeft = -7f;
    public float boundaryRight = 7f;

    [Header("Algılama Mesafesi")]
    public float reactionDistance = 10f;

    [Header("Sapma Payı (± hata)")]
    public float errorEasy = 3f;
    public float errorMedium = 1.5f;
    public float errorHard = 0.3f;

    [Header("Hareket Ayarları (Dash Mode)")]
    public float minDashPower = 5f;
    public float maxDashPower = 20f;
    public float dashFriction = 10f;

    // Cooldown artık zorluğa göre kod içinden ayarlanacak
    private float aiDashCooldown = 0.5f;

    private Rigidbody2D rb;
    private GameObject ball;
    private Rigidbody2D ballRb;
    private float homeX;

    private OyunAyarlari.MovementMode currentMode;

    private bool isCharging = false;
    private float currentCharge = 0f;
    private float targetDashPower = 0f;
    private float chargeDirection = 0f;
    private float dynamicChargeRate = 20f;

    private float currentRandomOffset = 0f;
    private float lastBallVy = 0f;
    private float lastDashTime = -10f;

    private Vector3 originalScale;

    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.freezeRotation = true;
        homeX = transform.position.x;
    }

    void Start()
    {
        currentDifficulty = OyunAyarlari.SelectedDifficulty;
        currentMode = OyunAyarlari.SelectedMovementMode;
        originalScale = transform.localScale;

        ball = GameObject.FindWithTag("Ball");
        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody2D>();
        }
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }

    }

    void Update()
    {
        if (ballRb == null) return;

        // Top yön değiştirdiğinde hata payını bir kere hesapla
        if (ballRb.linearVelocity.y != 0 && Mathf.Sign(ballRb.linearVelocity.y) != Mathf.Sign(lastBallVy))
        {
            float err = currentDifficulty switch
            {
                DifficultyLevel.Easy => errorEasy,
                DifficultyLevel.Medium => errorMedium,
                DifficultyLevel.Hard => errorHard,
                _ => errorMedium
            };
            currentRandomOffset = Random.Range(-err, err);
        }
        lastBallVy = ballRb.linearVelocity.y;

        // Zorluğa göre dinamik bekleme süresi (Cooldown)
        aiDashCooldown = currentDifficulty switch
        {
            DifficultyLevel.Easy => 1.0f,
            DifficultyLevel.Medium => 0.5f,
            DifficultyLevel.Hard => 0.15f, // Zor modda çok seri atılacak
            _ => 0.5f
        };

        if (currentMode == OyunAyarlari.MovementMode.Classic)
        {
            HandleClassicMovement();
        }
        else if (currentMode == OyunAyarlari.MovementMode.DashJump)
        {
            HandleDashMovement();
        }

        ClampPosition();
    }

    void HandleClassicMovement()
    {
        float dist = Vector2.Distance(transform.position, ball.transform.position);
        if (ballRb.linearVelocity.y > 0f && dist < reactionDistance)
            MoveTo(GetTargetX());
        else
            MoveTo(homeX);
    }

    void MoveTo(float x)
    {
        x = Mathf.Clamp(x, boundaryLeft, boundaryRight);
        float dx = x - transform.position.x;
        if (Mathf.Abs(dx) < 0.2f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            float dir = Mathf.Sign(dx);
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        }
    }

    void HandleDashMovement()
    {
        float dist = Vector2.Distance(transform.position, ball.transform.position);
        bool ballComing = (ballRb.linearVelocity.y > 0f && dist < reactionDistance);

        if (!isCharging)
        {
            float targetX = ballComing ? GetTargetX() : homeX;
            float dx = targetX - transform.position.x;

            // Hedefe yaklaştıysa zınk diye durmak yerine hızı yumuşatarak azalt (Daha akıcı görünüm)
            if (Mathf.Abs(dx) < 0.5f && Mathf.Abs(rb.linearVelocity.x) > 0.5f)
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, new Vector2(0f, rb.linearVelocity.y), Time.deltaTime * 10f);
            }
            // Bekleme süresi dolduysa ve hedeften 0.5 birim bile uzaksak hemen zıplamaya hazırlan
            else if (Time.time > lastDashTime + aiDashCooldown && Mathf.Abs(dx) > 0.5f)
            {
                StartDashCharge(dx);
            }
            else
            {
                ApplyFriction();
            }
        }
        else
        {
            currentCharge += dynamicChargeRate * Time.deltaTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (currentCharge >= targetDashPower || currentCharge >= maxDashPower)
            {
                ExecuteDash();
            }
        }
    }

    void StartDashCharge(float distanceToTarget)
    {
        isCharging = true;
        chargeDirection = Mathf.Sign(distanceToTarget);
        currentCharge = 0f;

        // Hedefe varacak tam gücü hesapla ve %20 (1.2f) ekstra güç ver ki seri ve atik olsun
        float exactPower = Mathf.Sqrt(2f * dashFriction * Mathf.Abs(distanceToTarget)) * 1.2f;
        targetDashPower = Mathf.Clamp(exactPower, minDashPower, maxDashPower);

        dynamicChargeRate = currentDifficulty switch
        {
            DifficultyLevel.Easy => 20f,
            DifficultyLevel.Medium => 35f,
            DifficultyLevel.Hard => 60f, // Zor modda anında şarj olup fırlayacak
            _ => 35f
        };
    }

    void ExecuteDash()
    {
        isCharging = false;
        float finalPower = Mathf.Max(currentCharge, minDashPower);
        rb.linearVelocity = new Vector2(chargeDirection * finalPower, rb.linearVelocity.y);
        currentCharge = 0f;
        lastDashTime = Time.time;
    }

    void ApplyFriction()
    {
        float currentX = rb.linearVelocity.x;
        if (Mathf.Abs(currentX) > 0.1f)
        {
            float change = -Mathf.Sign(currentX) * dashFriction * Time.deltaTime;
            if (Mathf.Sign(currentX + change) != Mathf.Sign(currentX))
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(currentX + change, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    float GetTargetX()
    {
        float dy = transform.position.y - ball.transform.position.y;
        float vy = ballRb.linearVelocity.y;
        if (vy <= 0f) return transform.position.x;

        float t = dy / vy;
        if (t <= 0f) return transform.position.x;

        float targetX = ball.transform.position.x + ballRb.linearVelocity.x * t;
        targetX += currentRandomOffset;

        return Mathf.Clamp(targetX, boundaryLeft, boundaryRight);
    }

    void ClampPosition()
    {
        var p = transform.position;
        p.x = Mathf.Clamp(p.x, boundaryLeft, boundaryRight);
        transform.position = p;
    }

    public void SetDifficulty(DifficultyLevel lvl)
    {
        currentDifficulty = lvl;
    }

    // --- GÜÇLENDİRME (POWER-UP) FONKSİYONLARI ---

    public void ActivateSizeUp(float duration)
    {
        StartCoroutine(SizeUpRoutine(duration));
    }

    private IEnumerator SizeUpRoutine(float duration)
    {
        transform.localScale = new Vector3(originalScale.x, originalScale.y * 1.5f, originalScale.z);
        if (sr != null) sr.color = new Color(1f, 0.6f, 0f); // Turuncu (Büyüme)

        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
        if (sr != null) sr.color = originalColor; // Eski renge dön
    }

    public void ActivateSlowDown(float duration)
    {
        StartCoroutine(SlowDownRoutine(duration));
    }

    private IEnumerator SlowDownRoutine(float duration)
    {
        float origMove = moveSpeed;
        float origMax = maxDashPower;

        moveSpeed *= 0.5f;
        maxDashPower *= 0.5f;

        if (sr != null) sr.color = Color.cyan; // Buz Mavisi (Yavaşlama/Ceza)

        yield return new WaitForSeconds(duration);

        moveSpeed = origMove;
        maxDashPower = origMax;

        if (sr != null) sr.color = originalColor; // Eski renge dön
    }
}