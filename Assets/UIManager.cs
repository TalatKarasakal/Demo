using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;

    [Header("Buttons")]
    public Button startButton;
    public Button restartButton;

    [Header("Score Texts")]
    public Text playerScoreText;
    public Text aiScoreText;

    [Header("GameManager")]
    public SimpleGameManager gameManager;

    void Start()
    {
        // Button event'lerini bağla
        if (startButton != null) 
        {
            startButton.onClick.AddListener(OnStart);
            Debug.Log("Start button listener added");
        }
        else
        {
            Debug.LogError("Start button is null!");
        }

        if (restartButton != null) 
        {
            restartButton.onClick.AddListener(OnRestart);
            Debug.Log("Restart button listener added");
        }

        // GameManager kontrolü
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<SimpleGameManager>();
            if (gameManager == null)
                Debug.LogError("GameManager bulunamadı!");
        }

        ShowMainMenu();
    }

    void Update()
    {
        // Score güncellemesi
        if (gameManager != null)
        {
            if (playerScoreText != null)
                playerScoreText.text = gameManager.playerScore.ToString();
            if (aiScoreText != null)
                aiScoreText.text = gameManager.aiScore.ToString();
        }
    }

    void OnStart()
    {
        Debug.Log("OnStart called!");
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (gameUIPanel != null)
            gameUIPanel.SetActive(true);
        
        Time.timeScale = 1f;
        
        if (gameManager != null)
        {
            gameManager.StartGame();
            Debug.Log("GameManager.StartGame() called");
        }
        else
        {
            Debug.LogError("GameManager is null in OnStart!");
        }
    }

    void OnRestart()
    {
        Debug.Log("OnRestart called!");
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }

    void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (gameUIPanel != null)
            gameUIPanel.SetActive(false);
        
        Time.timeScale = 0f;
    }

    // Test için buton
    void OnGUI()
    {
        if (Application.isEditor)
        {
            if (GUI.Button(new Rect(10, 110, 100, 30), "Test Start"))
            {
                OnStart();
            }
        }
    }
}