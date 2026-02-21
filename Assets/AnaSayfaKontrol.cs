using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AnaSayfaKontrol : MonoBehaviour
{
    private const int GameSceneIndex = 1;

    [Header("Menü Elemanları")]
    public Slider difficultySlider;
    public Toggle movementModeToggle;

    [Header("Başlatma Butonları")]
    public Button normalStartButton;
    public Button arcadeStartButton;
    public Button quitButton;

    [Header("Dil Butonları")]
    public Button trButton;
    public Button enButton;

    [Header("Tema Butonu")]
    public Button themeButton;
    public TextMeshProUGUI themeButtonText;

    [Header("Çevrilecek Metinler")]
    public TextMeshProUGUI normalStartText;
    public TextMeshProUGUI arcadeStartText;
    public TextMeshProUGUI quitText;
    public TextMeshProUGUI chooseLevelText;
    public TextMeshProUGUI easyText;
    public TextMeshProUGUI mediumText;
    public TextMeshProUGUI hardText;
    public TextMeshProUGUI dashModeText;

    void OnEnable() { DilYoneticisi.OnDilDegisti += MetinleriGuncelle; }
    void OnDisable() { DilYoneticisi.OnDilDegisti -= MetinleriGuncelle; }

    void Start()
    {
        if (difficultySlider != null)
        {
            difficultySlider.minValue = 0; difficultySlider.maxValue = 2; difficultySlider.wholeNumbers = true;
            difficultySlider.value = (int)OyunAyarlari.SelectedDifficulty;
            difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
        }

        if (movementModeToggle != null)
        {
            movementModeToggle.isOn = (OyunAyarlari.SelectedMovementMode == OyunAyarlari.MovementMode.DashJump);
            movementModeToggle.onValueChanged.AddListener(OnMovementModeChanged);
        }

        if (normalStartButton != null) normalStartButton.onClick.AddListener(StartNormalGame);
        if (arcadeStartButton != null) arcadeStartButton.onClick.AddListener(StartArcadeGame);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (trButton != null) trButton.onClick.AddListener(() => DilYoneticisi.Instance.DilDegistir(DilYoneticisi.Diller.Turkce));
        if (enButton != null) enButton.onClick.AddListener(() => DilYoneticisi.Instance.DilDegistir(DilYoneticisi.Diller.Ingilizce));

        // Tema butonunu bağla
        if (themeButton != null) themeButton.onClick.AddListener(TemaDegistir);

        MetinleriGuncelle();
    }

    void TemaDegistir()
    {
        // 0, 1, 2 arasında sırayla geçiş yap
        int siradakiTema = ((int)OyunAyarlari.SeciliTema + 1) % 3;
        OyunAyarlari.SeciliTema = (OyunAyarlari.Tema)siradakiTema;

        // Anında arkaplanı ve butonları boya
        TemaUygulayici uygulayici = FindFirstObjectByType<TemaUygulayici>();
        if (uygulayici != null) uygulayici.Uygula();

        MetinleriGuncelle();
    }

    void MetinleriGuncelle()
    {
        if (DilYoneticisi.Instance == null) return;

        if (normalStartText != null) normalStartText.text = DilYoneticisi.Instance.CeviriAl("normalPlay");
        if (arcadeStartText != null) arcadeStartText.text = DilYoneticisi.Instance.CeviriAl("arcadePlay");
        if (quitText != null) quitText.text = DilYoneticisi.Instance.CeviriAl("quit");
        if (chooseLevelText != null) chooseLevelText.text = DilYoneticisi.Instance.CeviriAl("chooseLevel");
        if (easyText != null) easyText.text = DilYoneticisi.Instance.CeviriAl("easy");
        if (mediumText != null) mediumText.text = DilYoneticisi.Instance.CeviriAl("medium");
        if (hardText != null) hardText.text = DilYoneticisi.Instance.CeviriAl("hard");
        if (dashModeText != null) dashModeText.text = DilYoneticisi.Instance.CeviriAl("dashMode");

        // Tema yazısını güncelle (Örn: "Tema: Karanlık")
        if (themeButtonText != null)
        {
            string temaIsmi = OyunAyarlari.SeciliTema switch
            {
                OyunAyarlari.Tema.Pastel => DilYoneticisi.Instance.CeviriAl("themePastel"),
                OyunAyarlari.Tema.Karanlik => DilYoneticisi.Instance.CeviriAl("themeDark"),
                OyunAyarlari.Tema.Klasik => DilYoneticisi.Instance.CeviriAl("themeClassic"),
                _ => ""
            };
            themeButtonText.text = DilYoneticisi.Instance.CeviriAl("theme") + ": " + temaIsmi;
        }
    }

    public void OnDifficultyChanged(float val) { OyunAyarlari.SelectedDifficulty = (YapayZekaKontrol.DifficultyLevel)(int)val; }
    public void OnMovementModeChanged(bool isOn) { OyunAyarlari.SelectedMovementMode = isOn ? OyunAyarlari.MovementMode.DashJump : OyunAyarlari.MovementMode.Classic; }
    public void StartNormalGame() { OyunAyarlari.IsArcadeMode = false; SceneManager.LoadScene(GameSceneIndex); }
    public void StartArcadeGame() { OyunAyarlari.IsArcadeMode = true; SceneManager.LoadScene(GameSceneIndex); }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }
}