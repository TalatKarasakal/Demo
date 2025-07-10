using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Build Settings'te MainMenu sahnesinin index'i 0, Game sahnenizin index'i 1 olmalı
    private const int GameSceneIndex = 1;

    // Play butonuna basıldığında çağrılacak
    public void OnPlayButton()
    {
        // Oyuna geçiş
        SceneManager.LoadScene(GameSceneIndex);
    }

    // Quit butonuna basıldığında çağrılacak
    public void OnQuitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}