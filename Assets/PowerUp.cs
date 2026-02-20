using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SizeUp, SlowOpponent, SpeedBall }
    public PowerUpType type;

    // Obje sahnede ne kadar kalacak? Alınmazsa kaybolsun.
    public float lifetime = 7f; 

    void Start()
    {
        // Alınmazsa kendini yok et
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Sadece top çarptığında çalışsın
        if (col.CompareTag("Ball"))
        {
            BallController ball = col.GetComponent<BallController>();
            
            if (ball != null && !string.IsNullOrEmpty(ball.lastHitter))
            {
                ApplyEffect(ball.lastHitter, ball);
                
                // Efekt (isteğe bağlı - sende hitEffectPrefab vardı, benzer bir şey eklenebilir)
                // Instantiate(powerupParticle, transform.position, Quaternion.identity);

                Destroy(gameObject); // Güçlendirmeyi yok et
            }
        }
    }

    void ApplyEffect(string hitterTag, BallController ball)
    {
        float effectDuration = 5f; // Etkiler 5 saniye sürecek
        
        // Sahnede oyuncuyu ve yapay zekayı bul
        PlayerController player = FindObjectOfType<PlayerController>();
        AIController ai = FindObjectOfType<AIController>();

        if (type == PowerUpType.SizeUp)
        {
            // Büyüme kutusunu alanın KENDİ raketi büyür
            if (hitterTag == "Player" && player != null) player.ActivateSizeUp(effectDuration);
            else if (hitterTag == "AI" && ai != null) ai.ActivateSizeUp(effectDuration);
        }
        else if (type == PowerUpType.SlowOpponent)
        {
            // Yavaşlatma kutusunu alan, RAKİBİNİ yavaşlatır
            if (hitterTag == "Player" && ai != null) ai.ActivateSlowDown(effectDuration);
            else if (hitterTag == "AI" && player != null) player.ActivateSlowDown(effectDuration);
        }
        else if (type == PowerUpType.SpeedBall)
        {
            // Topun hızını anlık olarak %50 artır
            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb != null) ballRb.linearVelocity *= 1.5f; 
        }
    }
}