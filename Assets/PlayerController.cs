using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float shortDistance = 1f;    // Kısa basışta kat edilecek mesafe
    public float longDistance = 3f;    // Uzun basışta kat edilecek mesafe
    public float maxPressTime = 0.5f;  // Long press eşik süresi (saniye)
    public float boundaryLeft = -7f;   // Sol sınır
    public float boundaryRight = 7f;   // Sağ sınır

    [Header("Hit Settings")]
    public float hitForce = 10f;
    public float hitRange = 1f;

    private Rigidbody2D rb;
    private Vector2 pressStartPos;
    private float pressStartTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleInput();
        HandleHit();  // Space ile vuruş (isteğe bağlı, mobilde de butonla çağırabilirsin)
    }

    void HandleInput()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            pressStartTime = Time.time;
            pressStartPos  = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ProcessPress(Time.time - pressStartTime, pressStartPos);
        }
#else
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                pressStartTime = Time.time;
                pressStartPos = t.position;
            }
            else if (t.phase == TouchPhase.Ended)
            {
                ProcessPress(Time.time - pressStartTime, pressStartPos);
            }
        }
#endif
    }

    void ProcessPress(float duration, Vector2 startPos)
    {
        // Sadece ekranın alt yarısına basıldığında tepki ver
        if (startPos.y > Screen.height * 0.5f) return;

        // Ekranın hangi tarafında basıldı?
        bool clickedLeft = startPos.x < Screen.width * 0.5f;
        float dir = clickedLeft ? -1f : +1f;

        // Basış süresine göre mesafe hesapla
        float t = Mathf.Clamp01(duration / maxPressTime);
        float moveDist = Mathf.Lerp(shortDistance, longDistance, t);

        // Yeni konum ve sınır kontrolü
        float newX = Mathf.Clamp(transform.position.x + dir * moveDist,
                                 boundaryLeft, boundaryRight);

        // Fizik kullanmak istersen:
        // rb.velocity = new Vector2((newX - transform.position.x) / Time.fixedDeltaTime, 0);
        // Ama basitçe pozisyonu da doğrudan set edebiliriz:
        transform.position = new Vector3(newX, transform.position.y, 0);
    }

    void HandleHit()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject ball = GameObject.FindWithTag("Ball");
            if (ball == null) return;
            if (Vector2.Distance(transform.position, ball.transform.position) > hitRange) return;

            var ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb == null) return;

            Vector2 dir = new Vector2(
                Random.Range(-0.3f, 0.3f),
                1f
            ).normalized;
            ballRb.linearVelocity = dir * hitForce;
            Debug.Log("Player hit the ball!");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Görsel test için alt sınırları göster
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}