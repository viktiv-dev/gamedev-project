using System.Collections;
using UnityEngine;

public class BackgroundColorController : MonoBehaviour
{
    [Header("Rereferences")]
    public Camera targetCamera;
    
    [Header("Settings")]
    public Color[] hookColors; 
    public float glowDuration = 0.2f; 
    [Range(0f, 1f)]
    public float alphaMultiplier = 1f;

    private Color defaultColor = Color.white;
    private int currentColorIndex = 0;
    private bool isTutorial = false;
    void Awake()
    {
        if (targetCamera != null)
        {
            defaultColor.a = alphaMultiplier;
            targetCamera.backgroundColor = defaultColor;
        }
    }

    public void SetTutorial(bool tutorial)
    {
        isTutorial = tutorial;
    }
    public void TriggerHookColor()
    {  
        if (hookColors.Length == 0 || targetCamera == null || isTutorial) return;

        Color nextColor = hookColors[currentColorIndex];
        nextColor.a = alphaMultiplier;
        currentColorIndex = (currentColorIndex + 1) % hookColors.Length;

        StopAllCoroutines();
        StartCoroutine(GlowRoutine(nextColor));
    }

    private IEnumerator GlowRoutine(Color targetColor)
    {
        float t = 0f;
        Color startColor = defaultColor;
        while (t < glowDuration)
        {
            t += Time.deltaTime;
            targetCamera.backgroundColor = Color.Lerp(startColor, targetColor, t / glowDuration);
            yield return null;
        }
        t = 0f;
        startColor = targetCamera.backgroundColor;
        while (t < glowDuration)
        {
            t += Time.deltaTime;
            targetCamera.backgroundColor = Color.Lerp(startColor, defaultColor, t / glowDuration);
            yield return null;
        }

        targetCamera.backgroundColor = defaultColor;
    }
}