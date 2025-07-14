using UnityEngine;
using System.Collections;

public class SimpleGameManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int playerScore;
    public int aiScore;
    public int scoreToWin = 5;

    [Header("Game Objects")]
    public GameObject ball;
    public GameObject player;
    public GameObject aiPaddle;

    [Header("Game State")]
    public bool gameEnded;
    public bool gameStarted;
    public bool gamePaused;

    void Start()
    {
        // GameObjects'leri otomatik bul
        if (ball == null) ball = GameObject.FindWithTag("Ball");
        if (player == null) player = GameObject.FindWithTag("Player");
        if (aiPaddle == null) aiPaddle = GameObject.FindWithTag("AI");

        // Eksik objeler için uyarı
        if (ball == null) Debug.LogError("Ball GameObject bulunamadı!");
        if (player == null) Debug.LogError("Player GameObject bulunamadı!");
        if (aiPaddle == null) Debug.LogError("AI Paddle GameObject bulunamadı!");
        
        StartGame();
    }

    public void StartGame()
    {
        if (gameStarted) return;
        
        gameStarted = true;
        gameEnded = false;
        gamePaused = false;
        playerScore = 0;
        aiScore = 0;
        
        ResetGameObjects();
        
        // Topu başlat
        if (ball != null)
        {
            StartCoroutine(StartBallAfterDelay());
        }
    }

    public void PauseGame()
    {
        if (!gameStarted || gameEnded) return;
        
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        if (!gameStarted || gameEnded) return;
        
        gamePaused = false;
        Time.timeScale = 1f;
    }

    public void StopGame()
    {
        gameStarted = false;
        gameEnded = true;
        gamePaused = false;
        Time.timeScale = 1f;
        
        var ballController = ball?.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.StopBall();
        }
    }

    IEnumerator StartBallAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        var ballController = ball.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.StartGame();
        }
    }

    public void OnPlayerScore() => Score(true);
    public void OnAIScore() => Score(false);

    void Score(bool isPlayer)
    {
        if (gameEnded) return;
        
        if (isPlayer) playerScore++; 
        else aiScore++;
        
        if (playerScore >= scoreToWin)
        {
            EndGame("Player");
        }
        else if (aiScore >= scoreToWin)
        {
            EndGame("AI");
        }
        else
        {
            StartCoroutine(RestartRoundAfterDelay());
        }
    }

    IEnumerator RestartRoundAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        RestartRound();
    }

    void RestartRound()
    {
        if (!gameEnded && ball != null)
        {
            ball.transform.position = Vector3.zero;
            var ballController = ball.GetComponent<BallController>();
            if (ballController != null)
            {
                ballController.ResetBall();
            }
        }
    }

    void EndGame(string winner)
    {
        gameEnded = true;
        gameStarted = false;
        
        var ballController = ball?.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.StopBall();
        }
    }

    public void RestartGame()
    {
        playerScore = aiScore = 0;
        gameEnded = false;
        gameStarted = false;
        gamePaused = false;
        Time.timeScale = 1f;
        
        ResetGameObjects();
        StartGame();
    }

    void ResetGameObjects()
    {
        // Player'ı reset et
        var playerController = player?.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ResetPlayer();
        }

        // AI'yi reset et
        var aiController = aiPaddle?.GetComponent<AIController>();
        if (aiController != null)
        {
            aiController.MoveTo(0f);
        }
        
        // Topu reset et
        if (ball != null)
        {
            ball.transform.position = Vector3.zero;
            var ballController = ball.GetComponent<BallController>();
            if (ballController != null)
            {
                ballController.StopBall();
            }
        }
    }

    public string GetWinner()
    {
        if (playerScore >= scoreToWin) return "Player";
        if (aiScore >= scoreToWin) return "AI";
        return "";
    }

    public bool IsGameActive()
    {
        return gameStarted && !gameEnded && !gamePaused;
    }
}