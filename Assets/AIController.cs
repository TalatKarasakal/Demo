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
    BallController ballController;

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
        else
            ballController = ball.GetComponent<BallController>();
            
        if (ballController == null)
            Debug.LogError("AIController: BallController bulunamadı!", this);
    }

    void Update()
    {
        if (rb == null || ball == null) return;
        HandleAI();
        ClampPosition();
    }

    void HandleAI()
    {
        Vector2 pos    = transform.position;
        Vector2 bPos   = ball.transform.position;
        Rigidbody2D bRb = ball.GetComponent<Rigidbody2D>();
        Vector2 bVel   = bRb != null ? bRb.linearVelocity : Vector2.zero;

        float dist = Vector2.Distance(pos, bPos);
        
        // AI'nın üst tarafta olduğunu varsayıyoruz (pozitif Y)
        bool comingTowards = bVel.y > 0f && bPos.y < pos.y;
        bool closeEnough = dist < reactionDistance;

        if (comingTowards && closeEnough)
        {
            // Hedef X tahmini + sapma
            float targetX = PredictBallX();
            float err = (1f - difficulty) * 2f;
            targetX += Random.Range(-err, +err);

            MoveTo(targetX);
        }
        else
        {
            // Merkeze dön
            MoveTo(0f);
        }
    }

    float PredictBallX()
    {
        Rigidbody2D bRb = ball.GetComponent<Rigidbody2D>();
        Vector2 bPos = ball.transform.position;
        Vector2 bVel = bRb != null ? bRb.linearVelocity : Vector2.zero;

        if (Mathf.Approximately(bVel.y, 0f))
            return transform.position.x;

        float timeToAI = (transform.position.y - bPos.y) / bVel.y;
        
        if (timeToAI < 0)
            return transform.position.x;

        float predictedX = bPos.x + bVel.x * timeToAI;
        
        if (predictedX < boundaryLeft || predictedX > boundaryRight)
        {
            predictedX = Mathf.Clamp(predictedX, boundaryLeft, boundaryRight);
        }
        
        return predictedX;
    }

    public void MoveTo(float targetX)
    {
        float tx = Mathf.Clamp(targetX, boundaryLeft, boundaryRight);
        float dx = tx - transform.position.x;
        
        if (Mathf.Abs(dx) < 0.1f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
        
        float dir = Mathf.Sign(dx);
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    void ClampPosition()
    {
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, boundaryLeft, boundaryRight);
        transform.position = p;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Vector2 ballVel = collision.rigidbody.linearVelocity;
            Vector2 normal = collision.contacts[0].normal;
            
            Vector2 reflection = Vector2.Reflect(ballVel, normal);
            reflection.x += rb.linearVelocity.x * 0.3f;
            
            if (reflection.magnitude < 5f)
                reflection = reflection.normalized * 5f;
                
            collision.rigidbody.linearVelocity = reflection;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, reactionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);
        
        Gizmos.color = Color.green;
        Vector3 leftBound = new Vector3(boundaryLeft, transform.position.y, 0);
        Vector3 rightBound = new Vector3(boundaryRight, transform.position.y, 0);
        Gizmos.DrawLine(leftBound, rightBound);
    }
}