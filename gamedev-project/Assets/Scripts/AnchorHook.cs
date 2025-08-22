using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorHook : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Rigidbody2D playerRb;
    public InputActionReference hookAction;   
    public InputActionReference cancelAction;
    public AimArrow aimArrow;         

    [Header("Settings")]
    public LineRenderer hookLine;
    public float hookSpeed = 20f;      
    public float pullForce = 5f;
    public float pullExponent = 1.5f;
    public float snapDistance = 0.2f;

    private AnchorPoint targetAnchor;
    private bool hookActive = false;
    private bool hookConnected = false;
    private Vector3 hookPosition;

    void OnEnable()
    {
        if (hookAction != null) hookAction.action.Enable();
        if (cancelAction != null) cancelAction.action.Enable();
    }

    void OnDisable()
    {
        if (hookAction != null) hookAction.action.Disable();
        if (cancelAction != null) cancelAction.action.Disable();
    }

    void Update()
    {
        if (player == null || aimArrow == null) return;

        if (hookAction != null && hookAction.action.WasPressedThisFrame())
        {
            AnchorPoint newAnchor = aimArrow.GetHighlightedAnchor();

            if (hookActive)
            {
                if (newAnchor != null && newAnchor != targetAnchor)
                {
                    StartHook(newAnchor);
                }
            }
            else
            {
                if (newAnchor != null)
                {
                    StartHook(newAnchor);
                }
            }
        }

        if (cancelAction != null && cancelAction.action.WasPressedThisFrame())
        {
            ResetHook();
        }

        if (hookActive && targetAnchor != null)
        {
            if (!hookConnected)
            {
                hookPosition = Vector3.MoveTowards(hookPosition, targetAnchor.transform.position, hookSpeed * Time.deltaTime);
                if (Vector3.Distance(hookPosition, targetAnchor.transform.position) < 0.05f)
                {
                    hookConnected = true;
                    hookPosition = targetAnchor.transform.position;
                    if (aimArrow != null)
                        aimArrow.SetIgnoredAnchor(targetAnchor);
                }
            }
            else
            {
                Vector2 toAnchor = (targetAnchor.transform.position - player.position);
                float distance = toAnchor.magnitude;

                if (distance > snapDistance)
                {
                    float force = pullForce * Mathf.Pow(distance, pullExponent);
                    playerRb.linearVelocity = toAnchor.normalized * force;
                }
                else
                {
                    player.position = targetAnchor.transform.position;
                    playerRb.linearVelocity = Vector2.zero;
                }
            }
            hookLine.SetPosition(0, player.position);
            hookLine.SetPosition(1, hookPosition);
        }
    }

    void StartHook(AnchorPoint newAnchor)
    {
        targetAnchor = newAnchor;
        hookActive = true;
        hookConnected = false;
        hookPosition = player.position;
        hookLine.enabled = true;
        hookLine.positionCount = 2;
    }

    void ResetHook()
    {
        hookActive = false;
        hookConnected = false;
        hookLine.enabled = false;
        targetAnchor = null;
        playerRb.linearVelocity = playerRb.linearVelocity.normalized * (pullForce * 0.5f);
        if (aimArrow != null)
            aimArrow.ClearIgnoredAnchor();
    }
}
