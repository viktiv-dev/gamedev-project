using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelButtonInfo
{
    public Button button;
    public GameObject lockIcon;
    public TMP_Text bestText;
    public int levelNumber;       
    public int sceneBuildIndex;   
}

public class LevelSelectController : MonoBehaviour
{
    public LevelButtonInfo[] levels;
    public Button firstButton; 
    void Start()
    {
        Refresh();
        if (firstButton != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }

    public void Refresh()
    {
        foreach (var lv in levels)
        {
            bool isUnlocked = PlayerPrefs.GetInt($"LevelUnlocked_{lv.levelNumber}", lv.levelNumber == 1 ? 1 : 0) == 1;
            lv.button.interactable = isUnlocked;

            if (lv.lockIcon != null)
                lv.lockIcon.SetActive(!isUnlocked);

            if (lv.bestText != null)
            {
                float best = PlayerPrefs.GetFloat($"Highscore_Level{lv.levelNumber}", 0f);
                lv.bestText.text = best > 0f ? $"{best:F1}s" : "No score";
            }
        }
    }

    public void LoadLevel(int buildIndex) => SceneManager.LoadScene(buildIndex);

    public void BackToMenu() => SceneManager.LoadScene(0);

    public static void UnlockNextLevel(int completedLevelNumber)
    {
        int nextLevel = completedLevelNumber + 1;
        PlayerPrefs.SetInt($"LevelUnlocked_{nextLevel}", 1);
        PlayerPrefs.Save();
    }

    public static void UpdateHighscore(int levelNumber, float timeRemaining)
    {
        float best = PlayerPrefs.GetFloat($"Highscore_Level{levelNumber}", 0f);
        if (timeRemaining > best)
        {
            PlayerPrefs.SetFloat($"Highscore_Level{levelNumber}", timeRemaining);
            PlayerPrefs.Save();
        }
    }
}
