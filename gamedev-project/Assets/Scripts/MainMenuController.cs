using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject startButton; // assign StartButton (for first selected)

    void Start()
    {
        // set initial selected so controller can navigate immediately
        if (startButton != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(startButton);
    }

    public void OpenLevelSelect()
    {
        SceneManager.LoadScene(1); // LevelSelect scene index
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