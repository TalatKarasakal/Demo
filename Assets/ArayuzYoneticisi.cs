using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ArayuzYoneticisi : MonoBehaviour
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

    [Header("Çevrilecek Menü Metinleri")]
    public TMP_Text resumeText;
    public TMP_Text restartText;
    public TMP_Text pauseMenuText;
    public TMP_Text playAgainText;
    public TMP_Text gameOverMenuText;
    
    // YENİ EKLENEN BAŞLIK REFERANSLARI
    public TMP_Text pausedTitleText;
    public TMP_Text gameOverTitleText; 

    [Header("GameManager")]
    public OyunYoneticisi gameManager;

    void OnEnable()
    {
        DilYoneticisi.OnDilDegisti += MetinleriGuncelle;
    }

    void OnDisable()
    {
        DilYoneticisi.OnDilDegisti -= MetinleriGuncelle;
    }

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<OyunYoneticisi>();

        SetupButtons();
        ShowMainMenu();
        MetinleriGuncelle();
    }

    void MetinleriGuncelle()
    {
        if (DilYoneticisi.Instance == null) return;

        if (resumeText != null) resumeText.text = DilYoneticisi.Instance.CeviriAl("resume");
        if (restartText != null) restartText.text = DilYoneticisi.Instance.CeviriAl("restart");
        if (pauseMenuText != null) pauseMenuText.text = DilYoneticisi.Instance.CeviriAl("mainMenu");
        if (playAgainText != null) playAgainText.text = DilYoneticisi.Instance.CeviriAl("playAgain");
        if (gameOverMenuText != null) gameOverMenuText.text = DilYoneticisi.Instance.CeviriAl("mainMenu");
        
        // YENİ BAŞLIK ÇEVİRİLERİ
        if (pausedTitleText != null) pausedTitleText.text = DilYoneticisi.Instance.CeviriAl("pausedTitle");
        
        UpdateUI(); 
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
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager != null && gameManager.gameStarted && !gameManager.gameEnded)
        {
            PauseGame();
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        if (gameManager == null || DilYoneticisi.Instance == null) return;

        if (playerScoreText != null) playerScoreText.text = gameManager.playerScore.ToString();
        if (aiScoreText != null) aiScoreText.text = gameManager.aiScore.ToString();

        if (gameStatusText != null)
        {
            if (gameManager.gameEnded) gameStatusText.text = DilYoneticisi.Instance.CeviriAl("gameOver");
            else if (gameManager.gamePaused) gameStatusText.text = DilYoneticisi.Instance.CeviriAl("paused");
            else if (gameManager.gameStarted) gameStatusText.text = DilYoneticisi.Instance.CeviriAl("playing");
            else gameStatusText.text = DilYoneticisi.Instance.CeviriAl("ready");
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
        if (gameManager != null) gameManager.StartGame();
    }

    public void PauseGame()
    {
        if (gameManager != null)
        {
            gameManager.PauseGame();
            if (pausePanel != null) pausePanel.SetActive(gameManager.gamePaused);
        }
    }

    public void ResumeGame()
    {
        if (gameManager != null)
        {
            gameManager.ResumeGame();
            if (pausePanel != null) pausePanel.SetActive(false);
        }
    }

    public void RestartGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameManager != null) gameManager.RestartGame();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        gameManager?.StopGame();
        if (gameManager != null) gameManager.gameEnded = false;
        SceneManager.LoadScene(0); 
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
            if (winnerText != null && gameManager != null && DilYoneticisi.Instance != null)
            {
                // PLAYER VE AI İSİMLERİNİ DE ÇEVİRİYORUZ
                string winner = gameManager.GetWinner();
                string translatedWinner = winner == "Player" ? DilYoneticisi.Instance.CeviriAl("player") : DilYoneticisi.Instance.CeviriAl("ai");
                winnerText.text = translatedWinner + " " + DilYoneticisi.Instance.CeviriAl("wins");
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