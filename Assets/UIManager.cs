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
        if (startButton != null)   startButton.onClick.AddListener(OnStart);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
        ShowMainMenu();
    }

    void Update()
    {
        if (gameManager != null)
        {
            playerScoreText.text = gameManager.playerScore.ToString();
            aiScoreText.text     = gameManager.aiScore.ToString();
        }
    }

    void OnStart()
    {
        mainMenuPanel.SetActive(false);
        gameUIPanel.SetActive(true);
        Time.timeScale = 1f;
        gameManager?.RestartGame();
    }

    void OnRestart()
    {
        gameManager?.RestartGame();
    }

    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gameUIPanel.SetActive(false);
        Time.timeScale = 0f;
    }
}