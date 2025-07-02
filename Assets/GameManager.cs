using UnityEngine;

public class SimpleGameManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int playerScore = 0;
    public int aiScore = 0;
    public int scoreToWin = 5;
    
    [Header("Game Objects")]
    public GameObject ball;
    public GameObject player;
    public GameObject aiPaddle;
    
    private bool gameEnded = false;
    
    void Start()
    {
        Debug.Log("Game Started! First to " + scoreToWin + " wins!");
        Debug.Log("Press R to restart, ESC to pause");
        
        // Oyunu başlat
        StartGame();
    }
    
    void Update()
    {
        // R tuşu ile oyunu yeniden başlat
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        // ESC ile oyunu duraklat/devam ettir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Space ile oyunu başlat (duraklı durumdayken)
        if (Input.GetKeyDown(KeyCode.Space) && Time.timeScale == 0f && !gameEnded)
        {
            Time.timeScale = 1f;
            Debug.Log("Game Resumed!");
        }
    }
    
    void StartGame()
    {
        gameEnded = false;
        Time.timeScale = 1f;
        
        // Topu başlat
        if (ball != null)
        {
            BallController ballController = ball.GetComponent<BallController>();
            if (ballController != null)
            {
                ballController.ResetBall();
            }
        }
    }
    
    public void OnPlayerScore()
    {
        if (gameEnded) return;
        
        playerScore++;
        Debug.Log("Player Scores! Score: " + playerScore + " - " + aiScore);
        
        if (playerScore >= scoreToWin)
        {
            EndGame("Player");
        }
        else
        {
            // Kısa bekleme sonrası topu yeniden başlat
            Invoke("RestartRound", 2f);
        }
    }
    
    public void OnAIScore()
    {
        if (gameEnded) return;
        
        aiScore++;
        Debug.Log("AI Scores! Score: " + playerScore + " - " + aiScore);
        
        if (aiScore >= scoreToWin)
        {
            EndGame("AI");
        }
        else
        {
            // Kısa bekleme sonrası topu yeniden başlat
            Invoke("RestartRound", 2f);
        }
    }
    
    void RestartRound()
    {
        if (!gameEnded)
        {
            // Sadece topu yeniden başlat
            if (ball != null)
            {
                ball.transform.position = Vector3.zero;
                BallController ballController = ball.GetComponent<BallController>();
                if (ballController != null)
                {
                    ballController.ResetBall();
                }
            }
        }
    }
    
    void EndGame(string winner)
    {
        gameEnded = true;
        
        Debug.Log("=== GAME OVER ===");
        Debug.Log(winner + " WINS!");
        Debug.Log("Final Score: Player " + playerScore + " - " + aiScore + " AI");
        Debug.Log("Press R to play again");
        
        // Topu durdur
        if (ball != null)
        {
            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb != null)
                ballRb.linearVelocity = Vector2.zero;
        }
        
        // Oyunu duraklat
        Time.timeScale = 0f;
    }
    
    public void RestartGame()
    {
        Debug.Log("Game Restarted!");
        
        // Skorları sıfırla
        playerScore = 0;
        aiScore = 0;
        gameEnded = false;
        
        // Oyun objelerini sıfırla
        ResetGameObjects();
        
        // Oyunu başlat
        Time.timeScale = 1f;
        
        Debug.Log("Score: " + playerScore + " - " + aiScore);
    }
    
    void ResetGameObjects()
    {
        // Topu merkeze getir
        if (ball != null)
        {
            ball.transform.position = Vector3.zero;
            BallController ballController = ball.GetComponent<BallController>();
            if (ballController != null)
            {
                ballController.ResetBall();
            }
        }
        
        // Player pozisyonunu sıfırla
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ResetPlayer();
            }
            else
            {
                player.transform.position = new Vector3(0, -4, 0);
            }
        }
        
        // AI pozisyonunu sıfırla
        if (aiPaddle != null)
        {
            aiPaddle.transform.position = new Vector3(0, 4, 0);
        }
    }
    
    void TogglePause()
    {
        if (gameEnded) return;
        
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            Debug.Log("Game PAUSED - Press ESC to continue or Space to resume");
        }
        else
        {
            Time.timeScale = 1;
            Debug.Log("Game RESUMED");
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && !gameEnded)
        {
            Time.timeScale = 0;
        }
    }
}