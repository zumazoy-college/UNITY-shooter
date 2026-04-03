using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Level_1()
    {
        SceneManager.LoadScene(2);
    }

    public void Level_2()
    {
        SceneManager.LoadScene(3);
    }

    public void Level_3()
    {
        SceneManager.LoadScene(4);
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
