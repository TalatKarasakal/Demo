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
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);

        if (pauseButton != null) pauseButton.onClick.AddListener(PauseGame);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (pauseMenuButton != null) pauseMenuButton.onClick.AddListener(GoToMainMenu);

        if (playAgainButton != null) playAgainButton.onClick.AddListener(RestartGame);
        if (menuButton != null) menuButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
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

        if (playerScoreText != null)
            playerScoreText.text = gameManager.playerScore.ToString();
        if (aiScoreText != null)
            aiScoreText.text = gameManager.aiScore.ToString();

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
        // Eğer oyunu durdurduysanız zamanı geri açın
        Time.timeScale = 1f;

        // Oyun içi değerleri sıfırlamak istiyorsanız (opsiyonel)
        gameManager?.StopGame();
        gameManager.gameEnded = false;

        // Ana Menü sahnesine dön
        SceneManager.LoadScene(0); // 0 yerine MainMenu sahnenizin Build Index’ini veya adını kullanabilirsiniz
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