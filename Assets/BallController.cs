using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    public float initialSpeed = 5f;
    public float maxSpeed = 15f;
    public float bounceMultiplier = 1.05f;
    public float wallBounceForce = 0.9f;

    [Header("Boundaries")]
    public float leftWall = -8f;
    public float rightWall = 8f;
    public float topBoundary = 5f;
    public float bottomBoundary = -5f;

    Rigidbody2D rb;
    Vector2 lastVel;
    SimpleGameManager gm;
    bool gameStarted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("BallController: Rigidbody2D bulunamadı!", this);
        else
            rb.freezeRotation = true;
    }

    void Start()
    {
        gm = Object.FindAnyObjectByType<SimpleGameManager>();
        if (gm == null)
            Debug.LogError("BallController: SimpleGameManager bulunamadı!", this);

        rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        if (rb == null || Time.timeScale == 0f) return;

        // Hız kontrolleri
        float speed = rb.linearVelocity.magnitude;
        if (speed > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        else if (speed < 1f && speed > 0.1f && gameStarted)
            rb.linearVelocity = rb.linearVelocity.normalized * 2f;

        lastVel = rb.linearVelocity;
        CheckBoundaries();
    }

    public void StartGame()
    {
        gameStarted = true;
        Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f),
                                  Random.value < 0.5f ? -1f : 1f).normalized;
        rb.linearVelocity = dir * initialSpeed;
    }

    void CheckBoundaries()
    {
        Vector3 p = transform.position;

        // Yan duvar sekmesi
        if (p.x <= leftWall || p.x >= rightWall)
        {
            Vector2 v = rb.linearVelocity;
            v.x = -v.x * wallBounceForce;
            rb.linearVelocity = v;
            p.x = Mathf.Clamp(p.x, leftWall + 0.1f, rightWall - 0.1f);
            transform.position = p;
        }

        // Üst-alt puan kontrolü
        if (p.y > topBoundary)
        {
            OnGoal("Player"); // Top üstten çıktı = Player'a puan
        }
        else if (p.y < bottomBoundary)
        {
            OnGoal("AI"); // Top alttan çıktı = AI'ya puan
        }
    }

    void OnGoal(string who)
    {
        gameStarted = false;
        rb.linearVelocity = Vector2.zero;
        if (gm != null)
        {
            if (who == "Player") gm.OnPlayerScore();
            else gm.OnAIScore();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (rb == null || Time.timeScale == 0f || !gameStarted) return;

        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("AI"))
        {
            Vector2 currentVel = rb.linearVelocity;
            Vector2 refl = Vector2.Reflect(currentVel, col.contacts[0].normal) * bounceMultiplier;

            // Paddle'ın hızını ekle
            var prb = col.gameObject.GetComponent<Rigidbody2D>();
            if (prb != null) 
                refl.x += prb.linearVelocity.x * 0.3f;

            // Minimum Y hızı garanti et
            if (Mathf.Abs(refl.y) < 2f) 
                refl.y = Mathf.Sign(refl.y) * 2f;

            // Çok dik açıları sınırla
            float ang = Mathf.Atan2(refl.y, refl.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(ang) > 75f && Mathf.Abs(ang) < 105f)
            {
                float clampedAngle = Mathf.Sign(ang) * 75f;
                float magnitude = refl.magnitude;
                refl = new Vector2(
                    Mathf.Cos(clampedAngle * Mathf.Deg2Rad) * magnitude, 
                    Mathf.Sin(clampedAngle * Mathf.Deg2Rad) * magnitude
                );
            }

            rb.linearVelocity = refl;
        }
    }

    public void ResetBall()
    {
        gameStarted = false;
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        
        // Kısa bir gecikme sonrası oyunu başlat
        Invoke(nameof(StartGame), 1f);
    }

    public void StopBall()
    {
        gameStarted = false;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(leftWall, -10), new Vector3(leftWall, 10));
        Gizmos.DrawLine(new Vector3(rightWall, -10), new Vector3(rightWall, 10));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-10, topBoundary), new Vector3(10, topBoundary));
        Gizmos.DrawLine(new Vector3(-10, bottomBoundary), new Vector3(10, bottomBoundary));
    }
}