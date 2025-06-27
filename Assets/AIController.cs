using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float boundaryLeft = -7f;
    public float boundaryRight = 7f;
    
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
        ball = GameObject.FindGameObjectWithTag("Ball");
        
        // Rigidbody2D ayarları
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }
    
    void Update()
    {
        if (ball == null)
        {
            ball = GameObject.FindGameObjectWithTag("Ball");
            return;
        }
        
        HandleAI();
    }
    
    void HandleAI()
    {
        float distanceToBall = Vector2.Distance(transform.position, ball.transform.position);
        Vector2 ballVelocity = ball.GetComponent<Rigidbody2D>().linearVelocity;
        
        // Top AI'ya doğru geliyorsa ve yakınsa
        if (ballVelocity.y > 0 && ball.transform.position.y < transform.position.y && distanceToBall < reactionDistance)
        {
            isMovingToBall = true;
            
            // Topun geldiği pozisyonu tahmin et
            float predictedX = PredictBallPosition();
            
            // Zorluk seviyesine göre tahmin hassasiyetini ayarla
            predictedX += Random.Range(-1f + difficulty, 1f - difficulty);
            
            // Hedefe doğru hareket et
            MoveToTarget(predictedX);
            
            // Vurma mesafesindeyse vur
            if (distanceToBall <= hitRange)
            {
                HitBall();
            }
        }
        else
        {
            isMovingToBall = false;
            // Merkeze dön
            MoveToTarget(0f);
        }
    }
    
    void MoveToTarget(float targetX)
    {
        // Sınırları kontrol et
        targetX = Mathf.Clamp(targetX, boundaryLeft, boundaryRight);
        
        float direction = 0f;
        if (Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            direction = targetX > transform.position.x ? 1f : -1f;
        }
        
        Vector2 movement = new Vector2(direction * moveSpeed, 0);
        rb.linearVelocity = movement;
        
        // Pozisyonu sınırla
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, boundaryLeft, boundaryRight);
        transform.position = pos;
    }
    
    float PredictBallPosition()
    {
        if (ball == null) return 0f;
        
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        Vector2 ballPos = ball.transform.position;
        Vector2 ballVel = ballRb.linearVelocity;
        
        // Basit fizik hesaplaması - topun AI seviyesine ulaşma zamanını hesapla
        float timeToReach = (transform.position.y - ballPos.y) / Mathf.Abs(ballVel.y);
        
        // Topun o zamandaki X pozisyonunu tahmin et
        float predictedX = ballPos.x + (ballVel.x * timeToReach);
        
        return predictedX;
    }
    
    void HitBall()
    {
        if (ball != null)
        {
            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                // Topu aşağı doğru vur (oyuncuya doğru)
                Vector2 hitDirection = new Vector2(
                    Random.Range(-0.4f, 0.4f), // Rastgele yatay açı
                    -1f // Aşağı doğru
                ).normalized;
                
                ballRb.linearVelocity = hitDirection * hitForce;
                
                Debug.Log("AI hit the ball!");
            }
        }
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
