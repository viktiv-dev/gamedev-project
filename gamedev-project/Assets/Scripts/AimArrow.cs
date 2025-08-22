using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class AimArrow : MonoBehaviour
{
    [Header("References")]
    public Transform player;                      
    public InputActionReference aimAction;        

    [Header("Settings")]
    public float radius = 1.5f;    
    private AnchorPoint ignoredAnchor;                
    public float deadZone = 0.2f;                  
    public bool hideWhenIdle = true;               
    public float detectionAngle = 15f;             
    public float detectionRange = 10f;            

    private Vector2 lastDir = Vector2.right;       
    private SpriteRenderer[] renderers;
    private AnchorPoint[] anchors;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        anchors = FindObjectsOfType<AnchorPoint>();
    }

    void OnEnable()
    {
        if (aimAction != null) aimAction.action.Enable();
    }

    void OnDisable()
    {
        if (aimAction != null) aimAction.action.Disable();
    }

    void Update()
    {
        if (player == null) return;

        Vector2 input = aimAction != null ? aimAction.action.ReadValue<Vector2>() : Vector2.zero;

        Vector2 dir;
        if (input.sqrMagnitude > deadZone * deadZone)
        {
            dir = input.normalized;
            lastDir = dir;
            SetVisible(true);
        }
        else
        {
            dir = lastDir;
            if (hideWhenIdle) SetVisible(false);
            else SetVisible(true);
        }

        Vector3 pos = player.position + (Vector3)(dir * radius);
        transform.position = new Vector3(pos.x, pos.y, 0);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        HighlightClosestAnchor(dir);
    }

    void HighlightClosestAnchor(Vector2 dir)
    {
        AnchorPoint closest = null;
        float closestDist = Mathf.Infinity;

        Vector3 arrowTip = player.position + (Vector3)(dir * radius); 
        foreach (var anchor in anchors)
        {
            if (anchor == null || anchor == ignoredAnchor) continue;

            Vector2 toAnchor = (anchor.transform.position - arrowTip);
            float distance = toAnchor.magnitude;

            if (distance > detectionRange) continue; 

            float angle = Vector2.Angle(dir, (anchor.transform.position - player.position));

            if (angle <= detectionAngle && distance < closestDist)
            {
                closest = anchor;
                closestDist = distance;
            }
        }

        foreach (var anchor in anchors)
            anchor.SetHighlight(false);
            
        if (closest != null)
            closest.SetHighlight(true);
    }

    public AnchorPoint GetHighlightedAnchor()
    {
        foreach (var anchor in anchors)
        {
            if (anchor != null && anchor.IsHighlighted())
                return anchor;
        }
        return null;
    }

    void SetVisible(bool visible)
    {
        foreach (var r in renderers)
            r.enabled = visible;
    }

    public void SetIgnoredAnchor(AnchorPoint anchor)
    {
        ignoredAnchor = anchor;
    }

    public void ClearIgnoredAnchor()
    {
        ignoredAnchor = null;
    }

}
