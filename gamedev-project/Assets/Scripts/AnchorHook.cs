using System.Collections;
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
    public CameraControler cameraController; // Reference to your camera script

    [Header("Settings")]
    public LineRenderer hookLine;
    public float hookSpeed = 20f;      
    public float pullForce = 5f;
    public float pullExponent = 1.5f;
    public float snapDistance = 0.2f;
    public float hookDelay = 0.2f; 

    private AnchorPoint targetAnchor;
    private bool hookActive = false;
    private bool hookConnected = false;
    private Vector3 hookPosition;
    
    public AudioSource hookAudioSource;
    public AudioClip hookSound;
    
    void OnEnable()
    {
        hookAction?.action.Enable();
        cancelAction?.action.Enable();
    }

    void OnDisable()
    {
        hookAction?.action.Disable();
        cancelAction?.action.Disable();
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

                    // Notify camera to focus on hook
                    cameraController?.SetHookTarget(targetAnchor.transform.position, true);

                    if (aimArrow != null)
                        aimArrow.SetIgnoredAnchor(targetAnchor);
                }
            }
            else
            {
                StartCoroutine(DelayHook(hookDelay, targetAnchor));
            }

            hookLine.SetPosition(0, player.position);
            hookLine.SetPosition(1, hookPosition);
        }
    }

    IEnumerator DelayHook(float delayTime, AnchorPoint targetAnchor)
    {
        yield return new WaitForSeconds(delayTime);
        MovePlayerToAnchor(targetAnchor);
    }

    void MovePlayerToAnchor(AnchorPoint anchor)
    {
        Vector2 toAnchor = (anchor.transform.position - player.position);
        float distance = toAnchor.magnitude;

        if (distance > snapDistance)
        {
            float force = pullForce * Mathf.Pow(distance, pullExponent);
            playerRb.linearVelocity = toAnchor.normalized * force;
        }
        else
        {
            player.position = anchor.transform.position;
            playerRb.linearVelocity = Vector2.zero;
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
        hookAudioSource.PlayOneShot(hookSound);
        GameTimer.Instance.AddTime(1f);
    }

    void ResetHook()
    {
        hookActive = false;
        hookConnected = false;
        hookLine.enabled = false;

        // Stop camera focus on hook
        cameraController?.SetHookTarget(Vector3.zero, false);

        targetAnchor = null;
        playerRb.linearVelocity = playerRb.linearVelocity.normalized * (pullForce * 0.5f);
        aimArrow?.ClearIgnoredAnchor();
    }
}
