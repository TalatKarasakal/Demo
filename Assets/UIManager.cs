using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    
    [Header("Buttons")]
    public Button startButton;
    public Button pauseButton;
    public Button resumeButton;
    public Button restartButton;
    public Button restartFromGameOverButton;
    public Button quitButton;
    
    [Header("Score Display")]
    public Text playerScoreText;
    public Text aiScoreText;
    public Text gameOverText;
    
    [Header("Game References")]
    public SimpleGameManager gameManager;
    
    private bool isPaused = false;
    
    void Start()
    {
        // Buton event'lerini bağla
        SetupButtons();
        
        // Başlangıçta ana menüyü göster
        ShowMainMenu();
    }
    
    void Update()
    {
        // Skor güncellemesi
        UpdateScoreDisplay();
        
        // ESC tuşu ile pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePanel.activeInHierarchy && !isPaused)
            {
                PauseGame();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
        }
    }
    
    void SetupButtons()
    {
        // Butonları kontrol et ve event'leri bağla
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
            
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
            
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (restartFromGameOverButton != null)
            restartFromGameOverButton.onClick.AddListener(RestartFromGameOver);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }
    
    public void ShowMainMenu()
    {
        SetActivePanel(mainMenuPanel);
        Time.timeScale = 0f;
    }
    
    public void ShowGamePanel()
    {
        SetActivePanel(gamePanel);
        isPaused = false;
    }
    
    public void ShowPausePanel()
    {
        SetActivePanel(pausePanel);
        isPaused = true;
    }
    
    public void ShowGameOverPanel(string winner)
    {
        SetActivePanel(gameOverPanel);
        
        if (gameOverText != null)
        {
            gameOverText.text = winner + " WINS!";
        }
        
        Time.timeScale = 0f;
    }
    
    void SetActivePanel(GameObject activePanel)
    {
        // Tüm panelleri kapat
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        // Aktif paneli aç
        if (activePanel != null)
            activePanel.SetActive(true);
    }
    
    public void StartGame()
    {
        ShowGamePanel();
        Time.timeScale = 1f;
        
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
    
    public void PauseGame()
    {
        ShowPausePanel();
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        ShowGamePanel();
        Time.timeScale = 1f;
    }
    
    public void RestartGame()
    {
        ShowGamePanel();
        Time.timeScale = 1f;
        
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
    
    public void RestartFromGameOver()
    {
        ShowGamePanel();
        Time.timeScale = 1f;
        
        if (gameManager != null)
        {
            gameManager.RestartGame();
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
    
    void UpdateScoreDisplay()
    {
        if (gameManager != null)
        {
            if (playerScoreText != null)
                playerScoreText.text = gameManager.playerScore.ToString();
                
            if (aiScoreText != null)
                aiScoreText.text = gameManager.aiScore.ToString();
        }
    }
}