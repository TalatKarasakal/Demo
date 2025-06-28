using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float boundaryLeft = -7;
    public float boundaryRight = 7;

    [Header("AI Settings")]
    public float reactionDistance = 8f;
    public float hitRange = 1f;
    public float hitForce = 10f;
    public float difficulty = 0.8f; // 0-1 arası, 1 = mükemmel AI

    private Rigidbody2D rb;
    private GameObject ball;
    private bool isMovingToBall = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("AIController: Rigidbody2D bulunamadı!", this);
        ball = GameObject.FindWithTag("Ball");
        if (ball == null)
            Debug.LogError("AIController: Top (Ball tag’li) bulunamadı!", this);

        // Rigidbody2D ayarları
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (ball == null)
        {
            ball = GameObject.FindWithTag("Ball");
            if (ball == null)
                Debug.LogError("AIController: Top (Ball tag’li) bulunamadı!", this);
            return;
        }

        HandleAI();
    }

    void HandleAI()
    {
        // 1) Topa olan mesafe ve hızı al
        float distanceToBall = Vector2.Distance(transform.position, ball.transform.position);
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        Vector2 ballVelocity = ballRb != null ? ballRb.linearVelocity : Vector2.zero;

        // 2) Koşulları daha okunaklı ara değişkenlerle tut
        bool ballMovingDown = ballVelocity.y < 0f;  // Y negatif olunca top AI'ya doğru geliyor (yukarıdan aşağı iniyor)
        bool aboveBall = transform.position.y > ball.transform.position.y;
        bool withinReaction = distanceToBall < reactionDistance;

        if (ballMovingDown && aboveBall && withinReaction)
        {
            // AI aktive olsun
            float predictedX = PredictBallPosition();

            // Zorluk seviyesine göre sapma ekle (-1+difficulty … +1-difficulty)
            float errorRange = 1f - difficulty;
            predictedX += Random.Range(-errorRange, +errorRange);

            MoveToTarget(predictedX);

            // Vurma menziline girerse topa vur
            if (distanceToBall <= hitRange)
                HitBall();
        }
        else
        {
            // Hiçbir koşul sağlanmıyorsa ortada bekle (hedef X = 0)
            MoveToTarget(0f);
        }
    }

    void MoveToTarget(float targetX)
    {
        // 1) Hedefi klampla
        float clampedX = Mathf.Clamp(targetX, boundaryLeft, boundaryRight);

        // 2) AI’nın ne kadar uzak olduğunu al
        float deltaX = clampedX - transform.position.x;

        // 3) Yeterince yakınsa hareketi durdur
        if (Mathf.Abs(deltaX) < 0.05f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 4) Hangi yönde hareket edilecek?
        float dir = Mathf.Sign(deltaX);

        // 5) Hızı uygula (sadece X komponenti)
        rb.linearVelocity = new Vector2(dir * moveSpeed, 0);
    }

    float PredictBallPosition()
    {
        if (ball == null) return transform.position.x;

        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        Vector2 ballPos = ball.transform.position;
        Vector2 ballVel = (ballRb != null ? ballRb.linearVelocity : Vector2.zero);

        // Y hızı sıfıra çok yakınsa bölme yapma
        if (Mathf.Approximately(ballVel.y, 0f))
            return transform.position.x;

        // AI paddle ile top arasındaki düşey mesafe / topun Y hızı
        float timeToReach = (transform.position.y - ballPos.y) / Mathf.Abs(ballVel.y);

        // Tahmini X pozisyonu
        return ballPos.x + ballVel.x * timeToReach;
    }

    void HitBall()
    {
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        if (ballRb == null) return;

        // Aşağı doğru bir vuruş yönü oluştur
        Vector2 hitDir = new Vector2(
            Random.Range(-0.4f, +0.4f),
            -1f
        ).normalized;

        ballRb.linearVelocity = hitDir * hitForce;
        Debug.Log("AI hit the ball!");
    }

    void OnDrawGizmosSelected()
    {
        // Hit range ve reaction distance görselleştir
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, reactionDistance);
    }
}
