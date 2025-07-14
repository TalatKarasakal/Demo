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

    bool gameEnded;
    bool gameStarted;

    void Start()
    {
        Debug.Log($"GameManager Ready! First to {scoreToWin} wins!");
        
        // GameObjects'leri otomatik bul
        if (ball == null) ball = GameObject.FindWithTag("Ball");
        if (player == null) player = GameObject.FindWithTag("Player");
        if (aiPaddle == null) aiPaddle = GameObject.FindWithTag("AI");
        
        // Eksik objeler için uyarı
        if (ball == null) Debug.LogError("Ball GameObject bulunamadı!");
        if (player == null) Debug.LogError("Player GameObject bulunamadı!");
        if (aiPaddle == null) Debug.LogError("AI Paddle GameObject bulunamadı!");
    }

    // UI'dan çağrılacak
    public void StartGame()
    {
        if (gameStarted) return;
        
        gameStarted = true;
        gameEnded = false;
        playerScore = 0;
        aiScore = 0;
        
        Debug.Log("Game Started!");
        
        ResetGameObjects();
        
        // Topu başlat
        if (ball != null)
        {
            StartCoroutine(StartBallAfterDelay());
        }
    }

    IEnumerator StartBallAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        var ballController = ball.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.StartGame();
            Debug.Log("Ball started!");
        }
    }

    public void OnPlayerScore() => Score(true);
    public void OnAIScore() => Score(false);

    void Score(bool isPlayer)
    {
        if (gameEnded) return;
        
        if (isPlayer) playerScore++; 
        else aiScore++;
        
        Debug.Log($"Score: Player {playerScore} - AI {aiScore}");
        
        if (playerScore >= scoreToWin || aiScore >= scoreToWin)
        {
            EndGame(isPlayer ? "Player" : "AI");
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
            Debug.Log("Restarting round...");
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
        Debug.Log($"=== GAME OVER: {winner} wins! ===");
        
        var ballController = ball?.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.StopBall();
        }
    }

    public void RestartGame()
    {
        Debug.Log("Game Restarted!");
        playerScore = aiScore = 0;
        gameEnded = false;
        gameStarted = false;
        
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

    // Debug için oyun durumunu göster
    void OnGUI()
    {
        if (Application.isEditor)
        {
            GUI.Label(new Rect(10, 70, 300, 20), $"Game Started: {gameStarted}");
            GUI.Label(new Rect(10, 90, 300, 20), $"Game Ended: {gameEnded}");
            
            // Manuel test butonu
            if (GUI.Button(new Rect(10, 150, 100, 30), "Force Start"))
            {
                StartGame();
            }
        }
    }
}