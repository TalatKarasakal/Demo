using UnityEngine;

public class AIController : MonoBehaviour
{
    public enum DifficultyLevel { Easy, Medium, Hard }

    [Header("Zorluk Seviyesi")]
    public DifficultyLevel currentDifficulty = DifficultyLevel.Medium;

    [Header("Hareket Ayarları")]
    public float moveSpeed       = 6f;
    public float boundaryLeft    = -7f;
    public float boundaryRight   =  7f;

    [Header("Algılama")]
    public float reactionDistance = 10f;

    [Header("Hata Payı (Ne kadar sapacak)")]
    public float errorEasy    =  2f;
    public float errorMedium  =  1f;
    public float errorHard    =  0.3f;

    Rigidbody2D rb;
    GameObject   ball;
    Rigidbody2D ballRb;
    float        homeX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("AIController: Rigidbody2D eksik!", this);
        else
            rb.freezeRotation = true;

        homeX = transform.position.x;
    }

    void Start()
    {
        ball = GameObject.FindWithTag("Ball");
        if (ball == null)
        {
            Debug.LogError("AIController: Tag=\"Ball\" objesi bulunamadı!", this);
            return;
        }

        ballRb = ball.GetComponent<Rigidbody2D>();
        if (ballRb == null)
            Debug.LogError("AIController: Ball'ın Rigidbody2D'si eksik!", this);
    }

    void Update()
    {
        if (ballRb == null) return;

        // Sadece top bize doğru geliyorsa (y > 0) ve yeterince yakınsa takip et
        if (ballRb.linearVelocity.y > 0f &&
            Vector2.Distance(transform.position, ball.transform.position) < reactionDistance)
        {
            TrackBall();
        }
        else
        {
            ReturnHome();
        }

        ClampPosition();
    }

    void TrackBall()
    {
        // Topla aynı düşey düzlemde ne kadar sürede kesişiriz?
        float dy = transform.position.y - ball.transform.position.y;
        float vy = ballRb.linearVelocity.y;
        if (vy <= 0) return; // aşağıya iniyorsa vazgeç

        float t = dy / vy;
        if (t <= 0) return;

        // Tahmini X
        float targetX = ball.transform.position.x + ballRb.linearVelocity.x * t;

        // Hata payını zorluğa göre seç
        float err = currentDifficulty switch
        {
            DifficultyLevel.Easy   => errorEasy,
            DifficultyLevel.Medium => errorMedium,
            _                      => errorHard,
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
    /// Dışarıdan zorluk değiştirmek istersen çağır:
    /// aiController.SetDifficulty(AIController.DifficultyLevel.Hard);
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
            new Vector3(boundaryLeft, transform.position.y, 0),
            new Vector3(boundaryRight, transform.position.y, 0)
        );
    }
}