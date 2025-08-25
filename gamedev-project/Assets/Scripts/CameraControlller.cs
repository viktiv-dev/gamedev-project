using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);
    public float lookAheadFactor = 0.5f;
    public float smoothTime = 0.15f;
    public float maxSpeedZoom = 2f; 
    public float speedZoomFactor = 0.05f; 

    private Vector3 velocity = Vector3.zero;
    private Vector3 hookTarget;
    private bool hookActive = false;

    void LateUpdate()
    {
        Vector3 targetPos = player.position + offset;

        // Lookahead based on player velocity
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector3 lookAhead = (Vector3)playerRb.linearVelocity * lookAheadFactor;
            targetPos += lookAhead;
        }

        // Shift toward hook point if active
        if (hookActive)
        {
            Vector3 toHook = (hookTarget - targetPos) * 0.3f; // 30% toward hook
            targetPos += toHook;
        }

        // Smooth movement
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // Optional speed zoom
        if (playerRb != null)
        {
            float targetZoom = offset.z - Mathf.Clamp(playerRb.linearVelocity.magnitude * speedZoomFactor, 0, maxSpeedZoom);
            Vector3 camPos = transform.position;
            camPos.z = targetZoom;
            transform.position = camPos;
        }
    }

    // Call from grappling hook
    public void SetHookTarget(Vector3 target, bool active)
    {
        hookTarget = target;
        hookActive = active;
    }
}