using UnityEngine;
using System.Collections;

public class SimpleGameManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int playerScore;
    public int aiScore;
    public int scoreToWin    = 3; // Kaç puan kazanınca biter
    public int currentRound  = 1;
    public int maxRounds     = 5; // Best of 5 (isteğe bağlı)

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
    public float roundEndDelay  = 2f;
    public float gameStartDelay = 1f;

    void Start()
    {
        // Eğer Inspector’dan atamadıysan, tag’leriyle bul
        if (ball     == null) ball     = GameObject.FindWithTag("Ball");
        if (player   == null) player   = GameObject.FindWithTag("Player");
        if (aiPaddle == null) aiPaddle = GameObject.FindWithTag("AI");

        // AIController bileşenini al
        if (aiPaddle != null)
            aiController = aiPaddle.GetComponent<AIController>();

        // Eksik obje uyarıları
        if (ball     == null) Debug.LogError("Ball GameObject bulunamadı!");
        if (player   == null) Debug.LogError("Player GameObject bulunamadı!");
        if (aiPaddle == null) Debug.LogError("AI Paddle GameObject bulunamadı!");

        InitializeGame();
        StartGame();
    }

    void InitializeGame()
    {
        gameStarted = false;
        gameEnded   = false;
        gamePaused  = false;
        roundEnded  = false;
        playerScore = 0;
        aiScore     = 0;
        currentRound= 1;
        Time.timeScale = 1f;

        ResetGameObjects();
    }

    public void StartGame()
    {
        if (gameStarted) return;

        gameStarted = true;
        gameEnded   = false;
        gamePaused  = false;
        roundEnded  = false;

        ResetGameObjects();
        StartCoroutine(StartBallAfterDelay());
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
        gameEnded   = false; // Main menu’e dönerken “kazandın” mesajı çıkmasın
        gamePaused  = false;
        roundEnded  = false;
        Time.timeScale = 1f;

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
    public void OnAIScore()     => Score(false);

    void Score(bool isPlayer)
    {
        if (gameEnded || roundEnded) return;
        roundEnded = true;

        if (isPlayer)
        {
            playerScore++;
            Debug.Log($"Player scored! Score: {playerScore}-{aiScore}");
        }
        else
        {
            aiScore++;
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

        // Topu ortala ve yeniden başlat
        if (ball != null)
        {
            ball.transform.position = Vector3.zero;
            var bc = ball.GetComponent<BallController>();
            if (bc != null) bc.ResetBall();
        }

        Debug.Log($"Round {currentRound} started! Score: {playerScore}-{aiScore}");
    }

    void EndGame(string winner)
    {
        gameEnded   = true;
        gameStarted = false;
        roundEnded  = true;

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
        // Player’ı reset et
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.ResetPlayer();
        }

        // AI paddle’ı merkeze al & durdur
        if (aiPaddle != null)
        {
            var p = aiPaddle.transform.position;
            aiPaddle.transform.position = new Vector3(0f, p.y, p.z);
            var rb = aiPaddle.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        // Topu reset et
        if (ball != null)
        {
            ball.transform.position = Vector3.zero;
            var bc = ball.GetComponent<BallController>();
            if (bc != null) bc.StopBall();
        }
    }

    public string GetWinner()
    {
        if (playerScore >= scoreToWin) return "Player";
        if (aiScore     >= scoreToWin) return "AI";
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

    public string GetScoreText() => $"{playerScore} - {aiScore}";
    public string GetRoundText()
    {
        if (gameEnded) return "Game Over";
        if (roundEnded) return "Round End";
        return $"Round {currentRound}";
    }
}