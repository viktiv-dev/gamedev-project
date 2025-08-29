using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public TMP_Text timerText;
    public Color normalColor = Color.black;
    public Color addedColor = Color.green;
    public Color removedColor = Color.red;
    public float blinkDuration = 0.3f;

    void Start()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnTimeAdded += BlinkGreen;
            GameTimer.Instance.OnTimeRemoved += BlinkRed;
        }
    }

    private void OnEnable()
    {
        timerText.enabled=true;
        timerText.gameObject.SetActive(true);
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnTimeAdded += BlinkGreen;
            GameTimer.Instance.OnTimeRemoved += BlinkRed;
        }
    }

    void Update()
    {
        if (GameTimer.Instance == null) return;

        float t = Mathf.Max(GameTimer.Instance.timeLeft, 0f);
        timerText.text = t.ToString("F1");
    }

    void BlinkGreen() => StartCoroutine(Blink(addedColor));
    void BlinkRed() => StartCoroutine(Blink(removedColor));

    IEnumerator Blink(Color blinkColor)
    {
        timerText.color = blinkColor;
        yield return new WaitForSeconds(blinkDuration);
        timerText.color = normalColor;
    }
}