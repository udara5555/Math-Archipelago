using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
    /// <summary>
    /// Navigates to a specified scene by name
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads the Home scene
    /// </summary>
    public void LoadHomeScene()
    {
        LoadScene("Home");
    }

    /// <summary>
    /// Loads the Menu scene
    /// </summary>
    public void LoadMenuScene()
    {
        LoadScene("Menu");
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}