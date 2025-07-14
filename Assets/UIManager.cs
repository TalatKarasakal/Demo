using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button quitButton;

    [Header("Game UI Buttons")]
    public Button pauseButton;
    public Button mainMenuButton;

    [Header("Pause Panel Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button pauseMenuButton;

    [Header("Game Over Panel")]
    public Button playAgainButton;
    public Button menuButton;
    public TMP_Text winnerText;

    [Header("Score Texts")]
    public TMP_Text playerScoreText;
    public TMP_Text aiScoreText;
    public TMP_Text gameStatusText;

    [Header("GameManager")]
    public SimpleGameManager gameManager;

    void Start()
    {
        // GameManager kontrolü
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<SimpleGameManager>();
            if (gameManager == null)
                Debug.LogError("GameManager bulunamadı!");
        }

        SetupButtons();
        ShowMainMenu();
    }

    void SetupButtons()
    {
        // Main Menu Buttons
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);

        // Game UI Buttons
        if (pauseButton != null) pauseButton.onClick.AddListener(PauseGame);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);

        // Pause Panel Buttons
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (pauseMenuButton != null) pauseMenuButton.onClick.AddListener(GoToMainMenu);

        // Game Over Panel Buttons
        if (playAgainButton != null) playAgainButton.onClick.AddListener(RestartGame);
        if (menuButton != null) menuButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
        // ESC tuşu ile pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameManager != null && gameManager.gameStarted && !gameManager.gameEnded)
            {
                PauseGame();
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (gameManager == null) return;

        // Score güncellemesi
        if (playerScoreText != null)
            playerScoreText.text = gameManager.playerScore.ToString();
        if (aiScoreText != null)
            aiScoreText.text = gameManager.aiScore.ToString();

        // Game status güncellemesi
        if (gameStatusText != null)
        {
            if (gameManager.gameEnded)
            {
                gameStatusText.text = "Game Over";
            }
            else if (gameManager.gamePaused)
            {
                gameStatusText.text = "Paused";
            }
            else if (gameManager.gameStarted)
            {
                gameStatusText.text = "Playing";
            }
            else
            {
                gameStatusText.text = "Ready";
            }
        }

        // Game Over paneli kontrolü
        if (gameManager.gameEnded && gameOverPanel != null && !gameOverPanel.activeSelf)
        {
            ShowGameOver();
        }
    }

    public void StartGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameUIPanel != null) gameUIPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        Time.timeScale = 1f;
        
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }

    public void PauseGame()
    {
        if (gameManager != null)
        {
            gameManager.PauseGame();
            
            if (pausePanel != null)
                pausePanel.SetActive(gameManager.gamePaused);
        }
    }

    public void ResumeGame()
    {
        if (gameManager != null)
        {
            gameManager.ResumeGame();
            
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }
    }

    public void RestartGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        
        if (gameManager != null)
        {
            gameManager.StopGame();
        }
        
        ShowMainMenu();
    }

    void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (winnerText != null && gameManager != null)
            {
                string winner = gameManager.GetWinner();
                winnerText.text = winner + " Wins!";
            }
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}