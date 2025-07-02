using UnityEngine;

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
    
    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    private SimpleGameManager gameManager;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D bileşeni eksik!");
            return;
        }
        
        gameManager = FindFirstObjectByType<SimpleGameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager sahnede bulunamadı!");
        }
        
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.linearVelocity = Vector2.zero;
    }
    
    void Update()
    {
        if (Time.timeScale == 0f || rb == null) return;
        
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        
        if (rb.linearVelocity.magnitude < 1f && rb.linearVelocity.magnitude > 0.1f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * 2f;
        }
        
        lastVelocity = rb.linearVelocity;
        CheckBoundaries();
    }
    
    public void StartGame()
    {
        Vector2 startDirection = new Vector2(
            Random.Range(-0.5f, 0.5f),
            Random.Range(0f, 1f) > 0.5f ? 1f : -1f
        ).normalized;
        
        rb.linearVelocity = startDirection * initialSpeed;
    }
    
    void CheckBoundaries()
    {
        Vector3 pos = transform.position;
        
        if (pos.x <= leftWall || pos.x >= rightWall)
        {
            Vector2 newVelocity = new Vector2(-lastVelocity.x * wallBounceForce, lastVelocity.y);
            rb.linearVelocity = newVelocity;
            pos.x = Mathf.Clamp(pos.x, leftWall + 0.1f, rightWall - 0.1f);
            transform.position = pos;
        }
        
        if (pos.y > topBoundary)
        {
            OnGoal("Player");
        }
        else if (pos.y < bottomBoundary)
        {
            OnGoal("AI");
        }
    }
    
    void OnGoal(string winner)
    {
        rb.linearVelocity = Vector2.zero;
        if (gameManager != null)
        {
            if (winner == "Player")
                gameManager.OnPlayerScore();
            else
                gameManager.OnAIScore();
        }
        else
        {
            Debug.LogError("GameManager bulunamadı!");
        }
        Debug.Log(winner + " scored!");
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.timeScale == 0f) return;
        
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("AI"))
        {
            Vector2 reflection = Vector2.Reflect(lastVelocity, collision.contacts[0].normal);
            
            Rigidbody2D paddleRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (paddleRb != null)
            {
                float paddleVelocityInfluence = paddleRb.linearVelocity.x * 0.3f;
                reflection.x += paddleVelocityInfluence;
            }
            
            rb.linearVelocity = reflection * bounceMultiplier;
            
            if (Mathf.Abs(rb.linearVelocity.y) < 2f)
            {
                float yDirection = rb.linearVelocity.y > 0 ? 1f : -1f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, yDirection * 2f);
            }
            
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            float maxAngle = 75f;
            
            if (Mathf.Abs(angle) > maxAngle && Mathf.Abs(angle) < 180f - maxAngle)
            {
                float clampedAngle = Mathf.Sign(angle) * maxAngle;
                float speed = rb.linearVelocity.magnitude;
                rb.linearVelocity = new Vector2(
                    Mathf.Cos(clampedAngle * Mathf.Deg2Rad) * speed,
                    Mathf.Sin(clampedAngle * Mathf.Deg2Rad) * speed
                );
            }
            
            Debug.Log("Ball hit " + collision.gameObject.name);
        }
    }
    
    public void ResetBall()
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        Invoke("StartGame", 0.5f);
    }
    
    public void StopBall()
    {
        rb.linearVelocity = Vector2.zero;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(leftWall, -10, 0), new Vector3(leftWall, 10, 0));
        Gizmos.DrawLine(new Vector3(rightWall, -10, 0), new Vector3(rightWall, 10, 0));
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-10, topBoundary, 0), new Vector3(10, topBoundary, 0));
        Gizmos.DrawLine(new Vector3(-10, bottomBoundary, 0), new Vector3(10, bottomBoundary, 0));
    }
}