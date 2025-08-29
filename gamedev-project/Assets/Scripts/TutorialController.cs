using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [Header("References")]
    public Image blackOverlay;
    public TextMeshProUGUI instructionText;
    public AimArrow aimArrow;
    public AnchorHook anchorHook;
    public GameObject anchorPoint;  
    public Transform player;
    public BackgroundColorController bgColorController;
    [Header("Timings")]
    public float fadeDuration = 1f;
    public float timeBeforeShowMoveText = 0.2f;
    public float timeAfterMoveDetected = 0.4f;
    public float timeBeforeSpawnAnchor = 0.4f;
    public CameraControler cameraControler;

    [Header("Detection")]
    public float joystickDetectThreshold = 0.2f;
    bool tutorialRunning = false;

    void Start()
    {
        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);
            SetOverlayAlpha(1f);
        }

        if (instructionText != null) instructionText.text = "";
        aimArrow.gameObject.SetActive(false);
        anchorHook.gameObject.SetActive(false);
        aimArrow.enabled = false;
        anchorHook.enabled = false;
        anchorPoint.SetActive(false); 
        bgColorController.SetTutorial(true);
        StartCoroutine(RunTutorial());
    }

    IEnumerator RunTutorial()
    {
        if (tutorialRunning) yield break;
        tutorialRunning = true;
        yield return StartCoroutine(FadeOverlay(1f, 0f, fadeDuration));
        yield return new WaitForSeconds(timeBeforeShowMoveText);
        yield return StartCoroutine(FadeInText(instructionText, "Move the left joystick to use the arrow.", 0.5f));
        aimArrow.enabled = true;
        aimArrow.gameObject.SetActive(true);
        bool moved = false;
        while (!moved)
        {
            if (aimArrow != null && aimArrow.aimAction != null && aimArrow.aimAction.action != null)
            {
                Vector2 v = aimArrow.aimAction.action.ReadValue<Vector2>();
                if (v.sqrMagnitude > joystickDetectThreshold * joystickDetectThreshold)
                    moved = true;
            }
            yield return null;
        }
        yield return StartCoroutine(FadeOutText(instructionText, 0.5f));
        yield return new WaitForSeconds(timeAfterMoveDetected);
        yield return StartCoroutine(FadeInText(instructionText, "Use your grappling hook to attach yourself to the anchor point. (RED BUTTON)", 0.5f));
        anchorPoint.SetActive(true);
        yield return new WaitForSeconds(timeBeforeSpawnAnchor);
        anchorHook.enabled = true;
        anchorHook.gameObject.SetActive(true);
        bool playerMoved = false;
        while (!playerMoved)
        {
            if (anchorHook.GetHookConnected())
            {
                playerMoved = true;
            }
            yield return null;
        }
        
        yield return new WaitForSeconds(2);
        anchorHook.gameObject.SetActive(false);
        anchorHook.ResetHook();
        aimArrow.gameObject.SetActive(false);
        yield return StartCoroutine(FadeOutText(instructionText, 0.5f));
        yield return StartCoroutine(FadeInText(instructionText, "You can let yourself go from the anchor point by pressing YELLOW BUTTON", 0.5f));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(FadeOutText(instructionText, 0.5f));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeInText(instructionText, "Now continue forward. Good luck", 0.5f));
        yield return new WaitForSeconds(2f);
        cameraControler.ZoomTo(15, 3);
        yield return StartCoroutine(FadeOutText(instructionText, 0.5f));
        anchorHook.gameObject.SetActive(true);
        aimArrow.gameObject.SetActive(true);
        tutorialRunning = false;
    }

    IEnumerator FadeOverlay(float from, float to, float duration)
    {
        if (blackOverlay == null) yield break;
        float t = 0f;
        SetOverlayAlpha(from);
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            SetOverlayAlpha(a);
            yield return null;
        }
        SetOverlayAlpha(to);
        if (to <= 0.001f) blackOverlay.gameObject.SetActive(false);
    }
    
    public IEnumerator FadeInText(TextMeshProUGUI text, string message, float fadeInTime)
    {
        if (text == null) yield break;

        text.text = message;
        Color c = text.color;
        c.a = 0f;
        text.color = c;

        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeInTime);
            text.color = c;
            yield return null;
        }
        c.a = 1f;
        text.color = c;
    }
    
    public IEnumerator FadeOutText(TextMeshProUGUI text, float fadeOutTime)
    {
        if (text == null) yield break;

        Color c = text.color;
        float t = 0f;
        float startAlpha = c.a;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, t / fadeOutTime);
            text.color = c;
            yield return null;
        }
        c.a = 0f;
        text.color = c;
        text.text = "";
    }



    void SetOverlayAlpha(float a)
    {
        if (blackOverlay == null) return;
        Color c = blackOverlay.color;
        c.a = a;
        blackOverlay.color = c;
    }
}
