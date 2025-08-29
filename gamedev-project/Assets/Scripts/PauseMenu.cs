using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public InputActionReference pauseAction;

    void OnEnable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPausePressed;
        }
    }

    void OnDisable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPausePressed;
            pauseAction.action.Disable();
        }

    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        QuitToLevelSelector();
    }

    public void QuitToLevelSelector()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }
}