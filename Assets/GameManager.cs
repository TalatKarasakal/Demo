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
    
    [Header("UI Reference")]
    public UIManager uiManager;
    
    private bool gameEnded = false;
    private bool gameStarted = false;
    
    void Start()
    {
        // UI Manager'ı bul
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();
            
        Debug.Log("Game Manager Ready! First to " + scoreToWin + " wins!");
        
        // Oyun başlamadan önce objeler hazır olsun
        ResetGameObjects();
    }
    
    void Update()
    {
        // Sadece oyun başladıysa keyboard kontrollerini dinle
        if (gameStarted)
        {
            // R tuşu ile oyunu yeniden başlat
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }
    
    public void StartGame()
    {
        gameEnded = false;
        gameStarted = true;
        Time.timeScale = 1f;
        
        Debug.Log("Game Started!");
        
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
        gameStarted = false;
        
        Debug.Log("=== GAME OVER ===");
        Debug.Log(winner + " WINS!");
        Debug.Log("Final Score: Player " + playerScore + " - " + aiScore + " AI");
        
        // Topu durdur
        if (ball != null)
        {
            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb != null)
                ballRb.linearVelocity = Vector2.zero;
        }
        
        // UI Manager'a oyun bittiğini bildir
        if (uiManager != null)
        {
            uiManager.ShowGameOverPanel(winner);
        }
        else
        {
            // UI Manager yoksa oyunu duraklat
            Time.timeScale = 0f;
        }
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
        StartGame();
        
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
                ballController.StopBall();
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
            Rigidbody2D aiRb = aiPaddle.GetComponent<Rigidbody2D>();
            if (aiRb != null)
            {
                aiRb.linearVelocity = Vector2.zero;
            }
        }
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Game PAUSED");
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Game RESUMED");
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && !gameEnded && gameStarted)
        {
            Time.timeScale = 0f;
        }
    }
}