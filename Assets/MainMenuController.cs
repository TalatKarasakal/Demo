using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    private const int GameSceneIndex = 1;

    [Header("Difficulty Buttons")]
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    [Header("Selection Marker")]
    public RectTransform marker;              // DifficultyMarker’ın RectTransform’u
    public Vector2 markerOffset = new Vector2(120f, 0f); // düğme sağına ne kadar kayacak

    void Awake()
    {
        // Butonlara listener ekle
        easyButton.onClick.AddListener(() => OnDifficultySelected(AIController.DifficultyLevel.Easy, easyButton.transform as RectTransform));
        mediumButton.onClick.AddListener(() => OnDifficultySelected(AIController.DifficultyLevel.Medium, mediumButton.transform as RectTransform));
        hardButton.onClick.AddListener(() => OnDifficultySelected(AIController.DifficultyLevel.Hard, hardButton.transform as RectTransform));
    }

    void Start()
    {
        // Başlangıçta “Medium” seçiliyse marker’ı oraya taşı
        OnDifficultySelected(GameSettings.SelectedDifficulty, mediumButton.transform as RectTransform);
    }

    void OnDifficultySelected(AIController.DifficultyLevel lvl, RectTransform btnRect)
    {
        // 1) Global seçimi kaydet
        GameSettings.SelectedDifficulty = lvl;

        // 2) Marker’ı aktif et ve doğru yere taşı
        if (marker != null && btnRect != null)
        {
            marker.gameObject.SetActive(true);
            marker.anchoredPosition = btnRect.anchoredPosition + markerOffset;
        }
    }

    // … Mevcut OnPlayButton/OnQuitButton metotlarınız …
    public void OnPlayButton()
    {
        SceneManager.LoadScene(GameSceneIndex);
    }

    public void OnQuitButton()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}