using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;           // Hareket hızı
    public float acceleration = 20f;       // İvmelenme
    public float deceleration = 15f;       // Yavaşlama
    public float boundaryLeft = -7f;       // Sol sınır
    public float boundaryRight = 7f;       // Sağ sınır

    [Header("Hit Settings")]
    public float hitForce = 10f;
    public float hitRange = 1f;

    private Rigidbody2D rb;
    private float targetVelocity = 0f;
    private bool leftPressed = false;
    private bool rightPressed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleHit();
        CheckBoundaries();
    }

    void HandleInput()
    {
        leftPressed = false;
        rightPressed = false;

#if UNITY_EDITOR
        // Mouse kontrolü (Editor için)
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Input.mousePosition;
            // Sadece ekranın alt yarısına basıldığında tepki ver
            if (mousePos.y <= Screen.height * 0.5f)
            {
                if (mousePos.x < Screen.width * 0.5f)
                    leftPressed = true;
                else
                    rightPressed = true;
            }
        }

        // Klavye kontrolü (test için)
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            leftPressed = true;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            rightPressed = true;
#else
        // Touch kontrolü (mobil için)
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                // Sadece ekranın alt yarısına basıldığında tepki ver
                if (touch.position.y <= Screen.height * 0.5f)
                {
                    if (touch.position.x < Screen.width * 0.5f)
                        leftPressed = true;
                    else
                        rightPressed = true;
                }
            }
        }
#endif
    }

    void HandleMovement()
    {
        // Hedef hızı belirle
        if (leftPressed && !rightPressed)
        {
            targetVelocity = -moveSpeed;
        }
        else if (rightPressed && !leftPressed)
        {
            targetVelocity = moveSpeed;
        }
        else
        {
            targetVelocity = 0f;
        }

        // Mevcut hızı hedef hıza doğru yumuşak geçiş
        float currentVelocityX = rb.linearVelocity.x;
        float velocityDifference = targetVelocity - currentVelocityX;
        
        float accelerationRate = targetVelocity != 0f ? acceleration : deceleration;
        float velocityChange = velocityDifference * accelerationRate * Time.deltaTime;
        
        // Hızı güncelle
        rb.linearVelocity = new Vector2(currentVelocityX + velocityChange, rb.linearVelocity.y);
    }

    void CheckBoundaries()
    {
        Vector3 pos = transform.position;
        
        // Sınır kontrolü ve pozisyon düzeltmesi
        if (pos.x < boundaryLeft)
        {
            pos.x = boundaryLeft;
            transform.position = pos;
            if (rb.linearVelocity.x < 0)
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (pos.x > boundaryRight)
        {
            pos.x = boundaryRight;
            transform.position = pos;
            if (rb.linearVelocity.x > 0)
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void HandleHit()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HitBall();
        }
    }

    public void HitBall()
    {
        GameObject ball = GameObject.FindWithTag("Ball");
        if (ball == null) return;
        if (Vector2.Distance(transform.position, ball.transform.position) > hitRange) return;

        var ballRb = ball.GetComponent<Rigidbody2D>();
        if (ballRb == null) return;

        // Raket hareket yönüne göre topa açı ver
        float horizontalInfluence = rb.linearVelocity.x * 0.3f;
        Vector2 dir = new Vector2(
            horizontalInfluence + Random.Range(-0.2f, 0.2f),
            1f
        ).normalized;
        
        ballRb.linearVelocity = dir * hitForce;
        Debug.Log("Player hit the ball!");
    }

    // Oyun sıfırlandığında çağrılır
    public void ResetPlayer()
    {
        transform.position = new Vector3(0, transform.position.y, 0);
        rb.linearVelocity = Vector2.zero;
        targetVelocity = 0f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}