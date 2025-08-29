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
    public CameraControler cameraController;
    public BackgroundColorController bgColorController;

    [Header("Settings")]
    public LineRenderer hookLine;
    public float hookSpeed = 20f;
    public float pullForce = 5f;
    public float pullExponent = 1.5f;
    public float hookDelay = 0.2f;
    private AnchorPoint lastHookedAnchor = null;

    [Header("Pull tuning")]
    public float maxPullSpeed = 25f;      
    public float pullLerp = 20f;            
    public float cancelCarrySpeed = 2f;     
    public float cancelDampDuration = 0.12f;

    [Header("Audio")]
    public AudioSource hookAudioSource;
    public AudioClip hookSound;

    private AnchorPoint targetAnchor;
    private bool hookActive = false;
    private bool hookConnected = false;
    private Vector3 hookPosition;
    private Coroutine pullCoroutine = null;
    private bool cancelled = false;
    
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
                    StartHook(newAnchor);
            }
            else
            {
                if (newAnchor != null)
                    StartHook(newAnchor);
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
                    cameraController?.SetHookTarget(targetAnchor.transform.position, true);
                    aimArrow?.SetIgnoredAnchor(targetAnchor);
                    if (pullCoroutine != null) StopCoroutine(pullCoroutine);
                    cancelled = false;
                    pullCoroutine = StartCoroutine(PullRoutine(targetAnchor));

                    // 🔥 NEW: trigger destruction if this is a destroyable anchor
                    DestroyAnchorPoint destroyable = targetAnchor.GetComponent<DestroyAnchorPoint>();
                    if (destroyable != null)
                    {
                        destroyable.StartDestroyCountdown();
                    }
                }

            }
            hookLine.SetPosition(0, player.position);
            hookLine.SetPosition(1, hookPosition);
        }
    }

    IEnumerator PullRoutine(AnchorPoint anchor)
    {
        float t = 0f;
        while (t < hookDelay)
        {
            if (cancelled) { pullCoroutine = null; yield break; }
            t += Time.deltaTime;
            yield return null;
        }
        
        while (!cancelled)
        {
            if (anchor == null) break;
            Vector2 toAnchor = (anchor.transform.position - player.position);
            float desiredSpeed = pullForce * Mathf.Pow(toAnchor.magnitude, pullExponent);
            desiredSpeed = Mathf.Min(desiredSpeed, maxPullSpeed);
            Vector2 desiredVel = toAnchor.normalized * desiredSpeed;
            float alpha = 1f - Mathf.Exp(-pullLerp * Time.fixedDeltaTime);
            playerRb.linearVelocity = Vector2.Lerp(playerRb.linearVelocity, desiredVel, alpha);
            yield return new WaitForFixedUpdate();
        }


        pullCoroutine = null;
    }

    void StartHook(AnchorPoint newAnchor)
    {
        if (pullCoroutine != null) { cancelled = true; StopCoroutine(pullCoroutine); pullCoroutine = null; }
        targetAnchor = newAnchor;
        hookActive = true;
        hookConnected = false;
        hookPosition = player.position;
        hookLine.enabled = true;
        hookLine.positionCount = 2; 
        hookAudioSource.PlayOneShot(hookSound);
        if (GameTimer.Instance != null ) 
            GameTimer.Instance.AddTime(1f);
        bgColorController?.TriggerHookColor();
    }

    public bool GetHookConnected()
    {
        return hookConnected;
    }

    public void ResetHook()
    {
        cancelled = true;
        if (pullCoroutine != null) { StopCoroutine(pullCoroutine); pullCoroutine = null; }
        hookActive = false;
        hookConnected = false;
        hookLine.enabled = false;
        cameraController?.SetHookTarget(Vector3.zero, false);
        targetAnchor = null;
        aimArrow?.ClearIgnoredAnchor();
        float currentSpeed = playerRb.linearVelocity.magnitude;
        float targetSpeed = Mathf.Min(currentSpeed, cancelCarrySpeed);
        StartCoroutine(DampVelocityTo(targetSpeed, cancelDampDuration));
    }


    IEnumerator DampVelocityTo(float targetSpeed, float duration)
    {
        float elapsed = 0f;
        Vector2 initialVel = playerRb.linearVelocity;
        if (initialVel.sqrMagnitude < 0.0001f)
        {
            playerRb.linearVelocity = Vector2.zero;
            yield break;
        }
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            float mag = Mathf.Lerp(initialVel.magnitude, targetSpeed, alpha);
            playerRb.linearVelocity = (initialVel.normalized) * mag;
            yield return null;
        }
        playerRb.linearVelocity = (initialVel.normalized) * targetSpeed;
    }
}
