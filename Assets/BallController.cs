using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    public float initialSpeed = 5f;
    public float maxSpeed = 15f;
    public float bounceMultiplier = 1.1f;
    public float wallBounceForce = 0.8f;
    
    [Header("Boundaries")]
    public float leftWall = -8f;
    public float rightWall = 8f;
    public float topBoundary = 5f;
    public float bottomBoundary = -5f;
    
    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ball physics settings
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        
        // Oyunu başlat
        StartGame();
    }
    
    void Update()
    {
        // Hızı sınırla
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        
        // Minimum hız kontrolü
        if (rb.linearVelocity.magnitude < 1f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * 2f;
        }
        
        lastVelocity = rb.linearVelocity;
        
        // Sınır kontrolü
        CheckBoundaries();
    }
    
    void StartGame()
    {
        // Rastgele yönde oyunu başlat
        Vector2 startDirection = new Vector2(
            Random.Range(-0.5f, 0.5f),
            Random.Range(0f, 1f) > 0.5f ? 1f : -1f
        ).normalized;
        
        rb.linearVelocity = startDirection * initialSpeed;
    }
    
    void CheckBoundaries()
    {
        Vector3 pos = transform.position;
        
        // Yan duvarlardan sekme
        if (pos.x <= leftWall || pos.x >= rightWall)
        {
            Vector2 newVelocity = new Vector2(-lastVelocity.x * wallBounceForce, lastVelocity.y);
            rb.linearVelocity = newVelocity;
            
            // Pozisyonu düzelt
            pos.x = Mathf.Clamp(pos.x, leftWall + 0.1f, rightWall - 0.1f);
            transform.position = pos;
        }
        
        // Üst ve alt sınırları kontrol et (gol durumu)
        if (pos.y > topBoundary)
        {
            // Player kazandı
            OnGoal("Player");
        }
        else if (pos.y < bottomBoundary)
        {
            // AI kazandı
            OnGoal("AI");
        }
    }
    
    void OnGoal(string winner)
    {
        Debug.Log(winner + " scored!");
        
        // Topu merkeze getir ve oyunu yeniden başlat
        transform.position = Vector3.zero;
        
        // Kısa bir bekleme sonrası yeniden başlat
        Invoke("StartGame", 1f);
        
        // Geçici olarak durdur
        rb.linearVelocity = Vector2.zero;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Raket ile çarpışma
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("AI"))
        {
            // Çarpışma noktasından yansıma hesapla
            Vector2 reflection = Vector2.Reflect(lastVelocity, collision.contacts[0].normal);
            
            // Hızı biraz artır
            rb.linearVelocity = reflection * bounceMultiplier;
            
            // Minimum Y hızı garantisi (top çok yatay gitmesin)
            if (Mathf.Abs(rb.linearVelocity.y) < 2f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y > 0 ? 2f : -2f);
            }
            
            Debug.Log("Ball hit " + collision.gameObject.name);
        }
    }
    
    // Oyunu manuel olarak yeniden başlatmak için
    public void ResetBall()
    {
        transform.position = Vector3.zero;
        StartGame();
    }
    
    void OnDrawGizmosSelected()
    {
        // Sınırları göster
        Gizmos.color = Color.green;
        
        // Yan duvarlar
        Gizmos.DrawLine(new Vector3(leftWall, -10, 0), new Vector3(leftWall, 10, 0));
        Gizmos.DrawLine(new Vector3(rightWall, -10, 0), new Vector3(rightWall, 10, 0));
        
        // Üst ve alt sınırlar
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-10, topBoundary, 0), new Vector3(10, topBoundary, 0));
        Gizmos.DrawLine(new Vector3(-10, bottomBoundary, 0), new Vector3(10, bottomBoundary, 0));
    }
}