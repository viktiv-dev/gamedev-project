using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinishTrigger : MonoBehaviour
{
    public int levelNumber = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        float timeLeft = GameTimer.Instance != null ? GameTimer.Instance.timeLeft : 0f;
        int nextLevel = levelNumber + 1;
        if (nextLevel <= 3)
        {
            PlayerPrefs.SetInt("LevelUnlocked_" + nextLevel, 1);
        }
        PlayerPrefs.SetFloat("LevelScore_" + levelNumber, timeLeft);
        PlayerPrefs.Save();
        SceneManager.LoadScene("LevelSelect");
    }
}