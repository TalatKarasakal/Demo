using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Build Settings'te MainMenu=index0, Game=index1 olduğundan emin ol.
    private const int GameSceneIndex = 1;

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