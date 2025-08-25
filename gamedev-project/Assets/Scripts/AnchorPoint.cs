using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.white;
    }

    public void SetHighlight(bool highlight)
    {
        sr.color = highlight ? Color.green : Color.white;
    }

    public bool IsHighlighted()
    {
        return sr.color == Color.green;
    }
    

}
