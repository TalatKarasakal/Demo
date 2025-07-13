using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Build Profiles/Settings’te MainMenu index=0, oyun index=1 olmalı
    const int GameSceneIndex = 1;

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