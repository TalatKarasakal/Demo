using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float boundaryLeft = -7f;
    public float boundaryRight = 7f;
    
    [Header("Hit Settings")]
    public float hitForce = 10f;
    public float hitRange = 1f;
    
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Rigidbody2D ayarları
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }
    
    void Update()
    {
        HandleMovement();
        HandleHit();
    }
    
    void HandleMovement()
    {
        // A-D veya ok tuşları ile hareket
        float horizontalInput = Input.GetAxis("Horizontal");
        
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, 0);
        rb.linearVelocity = movement;
        
        // Sınırları kontrol et
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, boundaryLeft, boundaryRight);
        transform.position = pos;
    }
    
    void HandleHit()
    {
        // Space tuşu ile vuruş
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HitBall();
        }
    }
    
    void HitBall()
    {
        // Yakındaki topu bul
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null)
        {
            float distance = Vector2.Distance(transform.position, ball.transform.position);
            
            if (distance <= hitRange)
            {
                Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
                if (ballRb != null)
                {
                    // Topu yukarı doğru vur
                    Vector2 hitDirection = new Vector2(
                        Random.Range(-0.3f, 0.3f), // Rastgele yatay açı
                        1f // Yukarı doğru
                    ).normalized;
                    
                    ballRb.linearVelocity = hitDirection * hitForce;
                    
                    Debug.Log("Player hit the ball!");
                }
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Hit range'i görselleştir
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}