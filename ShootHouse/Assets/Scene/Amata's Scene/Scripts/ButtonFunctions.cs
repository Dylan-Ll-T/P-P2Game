using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour 
{
    // Delvin's Additions
    public void startGame()
    {
        gamemanager.instance.startGame();
        gamemanager.instance.stateUnpause();
    }
    // End of Delvin's Additions
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }

    public void quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif

    }

    public void loadLevel(int level)
    {
        SceneManager.LoadScene(level);
        gamemanager.instance.stateUnpause();
    }
}
