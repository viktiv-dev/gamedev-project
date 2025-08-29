using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    [Header("Settings")]
    public Color defaultColor = Color.white;
    public Color highlightedColor = Color.yellow;
    
    private SpriteRenderer sr;
    private bool isHighlighted = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = defaultColor;
        SetHighlight(false);
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
        sr.color = highlight ? highlightedColor : defaultColor;
    }

    public bool IsHighlighted()
    {
        return isHighlighted;
    }
}