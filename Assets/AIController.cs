using UnityEngine;

public class AIController : MonoBehaviour
{
    public enum DifficultyLevel { Easy, Medium, Hard }

    [Header("Zorluk Seviyesi")]
    public DifficultyLevel currentDifficulty = DifficultyLevel.Medium;

    [Header("Hareket Ayarları")]
    public float moveSpeed = 6f;
    public float boundaryLeft = -7f;
    public float boundaryRight = 7f;

    [Header("Algılama Mesafesi")]
    public float reactionDistance = 10f;

    [Header("Sapma Payı (± hata)")]
    public float errorEasy = 3f;   // easy'de büyük sapma
    public float errorMedium = 1.5f; // medium'da orta sapma
    public float errorHard = 0.3f; // hard'da neredeyse hassas

    Rigidbody2D rb;
    GameObject ball;
    Rigidbody2D ballRb;
    float homeX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.freezeRotation = true;
        homeX = transform.position.x;
    }

    void Start()
    {
        // Menüden seçilen zorluğu al
        currentDifficulty = GameSettings.SelectedDifficulty;

        ball = GameObject.FindWithTag("Ball");
        if (ball != null)
            ballRb = ball.GetComponent<Rigidbody2D>();
        else
            Debug.LogError("AIController: Tag=\"Ball\" objesi bulunamadı!", this);
    }

    void Update()
    {
        if (ballRb == null) return;

        // Top bize doğru geliyorsa ve mesafe uygunsa takip et
        float dist = Vector2.Distance(transform.position, ball.transform.position);
        if (ballRb.linearVelocity.y > 0f && dist < reactionDistance)
            TrackBall();
        else
            ReturnHome();

        ClampPosition();
    }

    void TrackBall()
    {
        // düşeyde kesişme süresi
        float dy = transform.position.y - ball.transform.position.y;
        float vy = ballRb.linearVelocity.y;
        if (vy <= 0f) return;

        float t = dy / vy;
        if (t <= 0f) return;

        // ileriye tahmin
        float targetX = ball.transform.position.x + ballRb.linearVelocity.x * t;

        // hatayı zorluğa göre seç
        float err = currentDifficulty switch
        {
            DifficultyLevel.Easy => errorEasy,
            DifficultyLevel.Medium => errorMedium,
            DifficultyLevel.Hard => errorHard,
            _ => errorMedium
        };
        targetX += Random.Range(-err, err);

        MoveTo(targetX);
    }

    void ReturnHome()
    {
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

    void ClampPosition()
    {
        var p = transform.position;
        p.x = Mathf.Clamp(p.x, boundaryLeft, boundaryRight);
        transform.position = p;
    }

    /// <summary>
    /// İstersen runtime’da da zorluk değiştir:
    /// aiController.SetDifficulty(DifficultyLevel.Hard);
    /// </summary>
    public void SetDifficulty(DifficultyLevel lvl)
    {
        currentDifficulty = lvl;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, reactionDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(boundaryLeft, transform.position.y, 0f),
            new Vector3(boundaryRight, transform.position.y, 0f)
        );
    }
}