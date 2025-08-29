using TMPro;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public BackgroundColorController backgroundColorController;
    public TimerUI timerUI;
    public GameTimer gameTimer;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            timerUI.enabled = true;
            timerUI.gameObject.SetActive(true);
            backgroundColorController.SetTutorial(false);
            gameTimer.enabled = true;
            gameTimer.gameObject.SetActive(true);
            gameTimer.audioSource.Play();
        }
    }
}
