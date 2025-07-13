using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed       = 4f;
    public float boundaryLeft    = -7f;
    public float boundaryRight   = 7f;

    [Header("AI Settings")]
    public float reactionDistance = 8f;
    public float hitRange         = 1f;
    public float hitForce         = 10f;
    [Range(0f,1f)] public float difficulty = 0.8f;

    Rigidbody2D rb;
    GameObject  ball;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("AIController: Rigidbody2D bulunamadı!", this);
        else
            rb.freezeRotation = true;
    }

    void Start()
    {
        ball = GameObject.FindWithTag("Ball");
        if (ball == null)
            Debug.LogError("AIController: Top (tag=Ball) bulunamadı!", this);
    }

    void Update()
    {
        if (rb == null || ball == null) return;
        HandleAI();
    }

    void HandleAI()
    {
        Vector2 pos    = transform.position;
        Vector2 bPos   = ball.transform.position;
        Rigidbody2D bRb = ball.GetComponent<Rigidbody2D>();
        Vector2 bVel   = bRb != null ? bRb.linearVelocity : Vector2.zero;

        float dist           = Vector2.Distance(pos, bPos);
        bool  comingTowards = bVel.y < 0f && bPos.y < pos.y;
        bool  closeEnough   = dist < reactionDistance;

        if (comingTowards && closeEnough)
        {
            // Hedef X tahmini + sapma
            float targetX = PredictBallX();
            float err     = 1f - difficulty;
            targetX += Random.Range(-err, +err);

            MoveTo(targetX);
            if (dist <= hitRange)
                HitBall();
        }
        else
        {
            MoveTo(0f);  // merkeze dön
        }
    }

    float PredictBallX()
    {
        Rigidbody2D bRb = ball.GetComponent<Rigidbody2D>();
        Vector2 bPos = ball.transform.position;
        Vector2 bVel = bRb != null ? bRb.linearVelocity : Vector2.zero;

        if (Mathf.Approximately(bVel.y, 0f))
            return transform.position.x;

        float timeToAI = (transform.position.y - bPos.y) / Mathf.Abs(bVel.y);
        return bPos.x + bVel.x * timeToAI;
    }

    /// <summary>
    /// Basitçe MoveToTarget’i public ve kısaltılmış isimle expose ettik.
    /// </summary>
    public void MoveTo(float targetX)
    {
        float tx  = Mathf.Clamp(targetX, boundaryLeft, boundaryRight);
        float dx  = tx - transform.position.x;
        if (Mathf.Abs(dx) < 0.05f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        float dir = Mathf.Sign(dx);
        rb.linearVelocity = new Vector2(dir * moveSpeed, 0f);
    }

    void HitBall()
    {
        Rigidbody2D bRb = ball.GetComponent<Rigidbody2D>();
        if (bRb == null) return;

        Vector2 dir = new Vector2(Random.Range(-0.4f, 0.4f), -1f).normalized;
        bRb.linearVelocity = dir * hitForce;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, reactionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}