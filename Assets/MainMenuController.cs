using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private const int GameSceneIndex = 1;

    public void OnEasyButton()
    {
        GameSettings.SelectedDifficulty = AIController.DifficultyLevel.Easy;
    }

    public void OnMediumButton()
    {
        GameSettings.SelectedDifficulty = AIController.DifficultyLevel.Medium;
    }

    public void OnHardButton()
    {
        GameSettings.SelectedDifficulty = AIController.DifficultyLevel.Hard;
    }

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