using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private const int GameSceneIndex = 1;

    [Header("Menü Elemanları")]
    public Slider difficultySlider;     // Zorluk Sürgüsü
    public Toggle movementModeToggle;   // Hareket Modu Şalteri

    [Header("Başlatma Butonları")]
    public Button normalStartButton;
    public Button arcadeStartButton;
    public Button quitButton;

    void Start()
    {
        // 1. Zorluk Sürgüsünü Ayarla
        if (difficultySlider != null)
        {
            difficultySlider.minValue = 0;
            difficultySlider.maxValue = 2;
            difficultySlider.wholeNumbers = true; // Sadece tam sayılar (0, 1, 2)
            difficultySlider.value = (int)GameSettings.SelectedDifficulty;

            // Sürgü her kaydırıldığında OnDifficultyChanged metodunu çalıştır
            difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
        }

        // 2. Mod Şalterini Ayarla (Kapalı = Classic, Açık = DashJump)
        if (movementModeToggle != null)
        {
            movementModeToggle.isOn = (GameSettings.SelectedMovementMode == GameSettings.MovementMode.DashJump);
            movementModeToggle.onValueChanged.AddListener(OnMovementModeChanged);
        }

        // 3. Butonları Bağla
        if (normalStartButton != null) normalStartButton.onClick.AddListener(StartNormalGame);
        if (arcadeStartButton != null) arcadeStartButton.onClick.AddListener(StartArcadeGame);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    public void OnDifficultyChanged(float val)
    {
        // 0=Easy, 1=Medium, 2=Hard
        GameSettings.SelectedDifficulty = (AIController.DifficultyLevel)(int)val;
    }

    public void OnMovementModeChanged(bool isOn)
    {
        // Toggle aktifse DashJump, değilse Classic
        GameSettings.SelectedMovementMode = isOn ? GameSettings.MovementMode.DashJump : GameSettings.MovementMode.Classic;
    }

    public void StartNormalGame()
    {
        GameSettings.IsArcadeMode = false;
        SceneManager.LoadScene(GameSceneIndex);
    }

    public void StartArcadeGame()
    {
        GameSettings.IsArcadeMode = true;
        SceneManager.LoadScene(GameSceneIndex);
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