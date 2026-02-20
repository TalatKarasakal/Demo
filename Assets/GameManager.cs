using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SimpleGameManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int playerScore;
    public int aiScore;
    public int scoreToWin = 5; // Kaç puan kazanınca biter
    public int currentRound = 1;
    public int maxRounds = 5; // Best of 5 (isteğe bağlı)

    [Header("Game Objects")]
    public GameObject ball;
    public GameObject player;
    public GameObject aiPaddle;

    // AIController referansı, zorluk seviyesini ayarlamak için
    private AIController aiController;

    [Header("Game State")]
    public bool gameEnded;
    public bool gameStarted;
    public bool gamePaused;
    public bool roundEnded;

    [Header("Round Management")]
    public float roundEndDelay = 2f;
    public float gameStartDelay = 1f;

    [Header("UI Elements - Score Display")]
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI aiScoreText;
    public TextMeshProUGUI gameStatusText; // "Playing", "Paused", "Game Over" vs.

    // Difficulty selection
    public enum Difficulty { Easy, Medium, Hard }
    private Difficulty selectedDifficulty = Difficulty.Medium;
    public Difficulty CurrentDifficulty => selectedDifficulty;

    void Start()
    {
        // Eğer Inspector'dan atamadıysan, tag'leriyle bul
        if (ball == null) ball = GameObject.FindWithTag("Ball");
        if (player == null) player = GameObject.FindWithTag("Player");
        if (aiPaddle == null) aiPaddle = GameObject.FindWithTag("AI");

        // UI elementlerini otomatik bul (eğer Inspector'dan atanmadıysa)
        if (playerScoreText == null) playerScoreText = GameObject.Find("PlayerScoreText")?.GetComponent<TextMeshProUGUI>();
        if (aiScoreText == null) aiScoreText = GameObject.Find("AIScoreText")?.GetComponent<TextMeshProUGUI>();
        if (gameStatusText == null) gameStatusText = GameObject.Find("GameStatusText")?.GetComponent<TextMeshProUGUI>();

        // AIController bileşenini al
        if (aiPaddle != null)
            aiController = aiPaddle.GetComponent<AIController>();

        // Eksik obje uyarıları
        if (ball == null) Debug.LogError("Ball GameObject bulunamadı!");
        if (player == null) Debug.LogError("Player GameObject bulunamadı!");
        if (aiPaddle == null) Debug.LogError("AI Paddle GameObject bulunamadı!");
        if (playerScoreText == null) Debug.LogError("PlayerScoreText bulunamadı!");
        if (aiScoreText == null) Debug.LogError("AIScoreText bulunamadı!");

        InitializeGame();
        StartGame();
    }

    void InitializeGame()
    {
        gameStarted = false;
        gameEnded = false;
        gamePaused = false;
        roundEnded = false;
        playerScore = 0;
        aiScore = 0;
        currentRound = 1;
        UpdateScoreUI();
        UpdateGameStatusUI();
        Time.timeScale = 1f;

        ResetGameObjects();
    }

    public void StartGame()
    {
        if (gameStarted) return;

        gameStarted = true;
        gameEnded = false;
        gamePaused = false;
        roundEnded = false;

        UpdateGameStatusUI();
        ResetGameObjects();
        StartCoroutine(StartBallAfterDelay());
    }

    public void PauseGame()
    {
        if (!gameStarted || gameEnded) return;
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : 1f;
        UpdateGameStatusUI();
    }

    public void ResumeGame()
    {
        if (!gameStarted || gameEnded) return;
        gamePaused = false;
        Time.timeScale = 1f;
        UpdateGameStatusUI();
    }

    public void StopGame()
    {
        gameStarted = false;
        gameEnded = false; // Main menu'e dönerken "kazandın" mesajı çıkmasın
        gamePaused = false;
        roundEnded = false;
        Time.timeScale = 1f;
        UpdateGameStatusUI();

        var bc = ball?.GetComponent<BallController>();
        if (bc != null) bc.StopBall();
    }

    IEnumerator StartBallAfterDelay()
    {
        yield return new WaitForSeconds(gameStartDelay);
        if (gameStarted && !gameEnded)
        {
            var bc = ball?.GetComponent<BallController>();
            if (bc != null) bc.StartGame();
        }
    }

    public void OnPlayerScore() => Score(true);
    public void OnAIScore() => Score(false);

    void Score(bool isPlayer)
    {
        if (gameEnded || roundEnded) return;
        roundEnded = true;

        if (isPlayer)
        {
            playerScore++;
            UpdateScoreUI();
            Debug.Log($"Player scored! Score: {playerScore}-{aiScore}");
        }
        else
        {
            aiScore++;
            UpdateScoreUI();
            Debug.Log($"AI scored! Score: {playerScore}-{aiScore}");
        }

        if (playerScore >= scoreToWin)
            StartCoroutine(EndGameAfterDelay("Player"));
        else if (aiScore >= scoreToWin)
            StartCoroutine(EndGameAfterDelay("AI"));
        else
            StartCoroutine(StartNextRoundAfterDelay());
    }

    IEnumerator StartNextRoundAfterDelay()
    {
        yield return new WaitForSeconds(roundEndDelay);
        if (gameStarted && !gameEnded)
            StartNextRound();
    }

    IEnumerator EndGameAfterDelay(string winner)
    {
        yield return new WaitForSeconds(roundEndDelay);
        EndGame(winner);
    }

    void StartNextRound()
    {
        if (gameEnded) return;
        roundEnded = false;
        currentRound++;
        UpdateGameStatusUI();

        // 1. Raketleri ve topu merkeze al (Topu durdurur)
        ResetGameObjects();

        // 2. Topun 1 saniye bekleyip tekrar fırlamasını sağla
        if (ball != null)
        {
            var bc = ball.GetComponent<BallController>();
            if (bc != null) bc.ResetBall();
        }

        Debug.Log($"Round {currentRound} started! Score: {playerScore}-{aiScore}");
    }

    void EndGame(string winner)
    {
        gameEnded = true;
        gameStarted = false;
        roundEnded = true;
        UpdateGameStatusUI();

        var bc = ball?.GetComponent<BallController>();
        if (bc != null) bc.StopBall();

        Debug.Log($"Game Over! Winner: {winner}. Final Score: {playerScore}-{aiScore}");
    }

    public void RestartGame()
    {
        StopAllCoroutines();
        InitializeGame();
        StartGame();
    }

    void ResetGameObjects()
    {
        // Player'ı reset et
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.ResetPlayer();
        }

        // AI paddle'ı merkeze al & durdur
        if (aiPaddle != null)
        {
            var p = aiPaddle.transform.position;
            aiPaddle.transform.position = new Vector3(0f, p.y, p.z);
            var rb = aiPaddle.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        // Topu sıfırla
        if (ball != null)
        {
            ball.transform.position = Vector3.zero;
            var bc = ball.GetComponent<BallController>();
            if (bc != null) bc.StopBall();
        }
    }

    // BU FONKSİYON ÇOK ÖNEMLİ - SKORLARI GÜNCELLER
    private void UpdateScoreUI()
    {
        if (playerScoreText != null)
            playerScoreText.text = playerScore.ToString();
        if (aiScoreText != null)
            aiScoreText.text = aiScore.ToString();
    }

    // OYUN DURUMUNU GÖSTERIR
    private void UpdateGameStatusUI()
    {
        if (gameStatusText != null)
        {
            if (gameEnded)
            {
                string winner = GetWinner();
                gameStatusText.text = $"Game Over - {winner} Wins!";
            }
            else if (gamePaused)
            {
                gameStatusText.text = "PAUSED";
            }
            else if (gameStarted)
            {
                gameStatusText.text = $"Round {currentRound} - Playing";
            }
            else
            {
                gameStatusText.text = "Ready to Play";
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
        => gameStarted && !gameEnded && !gamePaused;

    public void SetAIDifficulty(AIController.DifficultyLevel diff)
    {
        if (aiController != null)
        {
            aiController.SetDifficulty(diff);
            Debug.Log($"AI Difficulty set to: {diff}");
        }
    }

    /// <summary>
    /// Called from menu buttons to set difficulty and load the game scene.
    /// </summary>
    public void SetDifficultyAndStart(Difficulty diff, string sceneName)
    {
        selectedDifficulty = diff;
        SceneManager.LoadScene(sceneName);
    }

    public string GetScoreText() => $"{playerScore} - {aiScore}";
    public string GetRoundText()
    {
        if (gameEnded) return "Game Over";
        if (roundEnded) return "Round End";
        return $"Round {currentRound}";
    }
}