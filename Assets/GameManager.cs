using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int playerScore = 0;
    public int aiScore = 0;
    public int scoreToWin = 5;
    
    [Header("UI References")]
    public Text playerScoreText;
    public Text aiScoreText;
    public Text gameStatusText;
    public Button restartButton;
    
    [Header("Game Objects")]
    public GameObject ball;
    public GameObject player;
    public GameObject aiPaddle;
    
    private bool gameEnded = false;
    
    void Start()
    {
        // UI başlangıç ayarları
        UpdateScoreUI();
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false);
        }
        
        if (gameStatusText != null)
        {
            gameStatusText.text = "Game Started! First to " + scoreToWin + " wins!";
        }
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
    }
    
    public void OnPlayerScore()
    {
        if (gameEnded) return;
        
        playerScore++;
        UpdateScoreUI();
        
        if (playerScore >= scoreToWin)
        {
            EndGame("Player");
        }
        else
        {
            ShowTemporaryMessage("Player Scores! " + playerScore + " - " + aiScore);
        }
    }
    
    public void OnAIScore()
    {
        if (gameEnded) return;
        
        aiScore++;
        UpdateScoreUI();
        
        if (aiScore >= scoreToWin)
        {
            EndGame("AI");
        }
        else
        {
            ShowTemporaryMessage("AI Scores! " + playerScore + " - " + aiScore);
        }
    }
    
    void UpdateScoreUI()
    {
        if (playerScoreText != null)
            playerScoreText.text = "Player: " + playerScore;
        
        if (aiScoreText != null)
            aiScoreText.text = "AI: " + aiScore;
    }
    
    void EndGame(string winner)
    {
        gameEnded = true;
        
        if (gameStatusText != null)
        {
            gameStatusText.text = winner + " Wins! Press R to restart";
        }
        
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }
        
        // Topu durdur
        if (ball != null)
        {
            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb != null)
                ballRb.linearVelocity = Vector2.zero;
        }
        
        Debug.Log(winner + " wins the game!");
    }
    
    public void RestartGame()
    {
        // Skorları sıfırla
        playerScore = 0;
        aiScore = 0;
        gameEnded = false;
        
        // UI'ı güncelle
        UpdateScoreUI();
        
        if (gameStatusText != null)
        {
            gameStatusText.text = "Game Restarted! First to " + scoreToWin + " wins!";
        }
        
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }
        
        // Oyun objelerini sıfırla
        ResetGameObjects();
        
        Debug.Log("Game restarted!");
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
            player.transform.position = new Vector3(0, -4, 0);
        }
        
        // AI pozisyonunu sıfırla
        if (aiPaddle != null)
        {
            aiPaddle.transform.position = new Vector3(0, 4, 0);
        }
    }
    
    void ShowTemporaryMessage(string message)
    {
        if (gameStatusText != null)
        {
            gameStatusText.text = message;
            
            // 2 saniye sonra normal mesajı göster
            Invoke("ShowNormalMessage", 2f);
        }
    }
    
    void ShowNormalMessage()
    {
        if (gameStatusText != null && !gameEnded)
        {
            gameStatusText.text = "Score: " + playerScore + " - " + aiScore;
        }
    }
    
    void TogglePause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            if (gameStatusText != null)
                gameStatusText.text = "PAUSED - Press ESC to continue";
        }
        else
        {
            Time.timeScale = 1;
            ShowNormalMessage();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
