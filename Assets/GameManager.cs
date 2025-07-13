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

    void Start()
    {
        Debug.Log($"GameManager Ready! First to {scoreToWin} wins!");
        ResetGameObjects();
    }

    public void OnPlayerScore() => Score(true);
    public void OnAIScore()    => Score(false);

    void Score(bool isPlayer)
    {
        if (gameEnded) return;

        if (isPlayer) playerScore++; else aiScore++;
        Debug.Log($"Score: {playerScore} - {aiScore}");

        if (playerScore >= scoreToWin || aiScore >= scoreToWin)
            EndGame(isPlayer ? "Player" : "AI");
        else
            Invoke(nameof(RestartRound), 2f);
    }

    void RestartRound()
    {
        if (!gameEnded && ball != null)
        {
            ball.transform.position = Vector3.zero;
            ball.GetComponent<BallController>()?.ResetBall();
        }
    }

    void EndGame(string winner)
    {
        gameEnded = true;
        Debug.Log($"=== GAME OVER: {winner} wins! ===");
        ball?.GetComponent<BallController>()?.StopBall();
    }

    public void RestartGame()
    {
        Debug.Log("Game Restarted!");
        playerScore = aiScore = 0;
        gameEnded = false;
        ResetGameObjects();
    }

    void ResetGameObjects()
    {
        ball?.GetComponent<BallController>()?.StopBall();
        player?.GetComponent<PlayerController>()?.ResetPlayer();
        aiPaddle?.GetComponent<AIController>()?.MoveTo(0f);
    }
}