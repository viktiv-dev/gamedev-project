using System.Collections;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera cam;
    
    [Header("Settings")]
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);
    public float lookAheadFactor = 0.5f;
    public float smoothTime = 0.15f;
    public float maxSpeedZoom = 2f; 
    public float speedZoomFactor = 0.05f; 
    
    private Coroutine zoomCoroutine = null;
    private Vector3 velocity = Vector3.zero;
    private Vector3 hookTarget;
    private bool hookActive = false;

    void LateUpdate()
    {
        Vector3 targetPos = player.position + offset;
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector3 lookAhead = (Vector3)playerRb.linearVelocity * lookAheadFactor;
            targetPos += lookAhead;
        }
        if (hookActive)
        {
            Vector3 toHook = (hookTarget - targetPos) * 0.3f; 
            targetPos += toHook;
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        if (playerRb != null)
        {
            float targetZoom = offset.z - Mathf.Clamp(playerRb.linearVelocity.magnitude * speedZoomFactor, 0, maxSpeedZoom);
            Vector3 camPos = transform.position;
            camPos.z = targetZoom;
            transform.position = camPos;
        }
    }
    public void SetHookTarget(Vector3 target, bool active)
    {
        hookTarget = target;
        hookActive = active;
    }
    public void ZoomTo(float targetSize, float duration)
    {
        if (cam == null) return;
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomCoroutine(targetSize, duration));
    }

    private IEnumerator ZoomCoroutine(float targetSize, float duration)
    {
        float startSize = cam.orthographicSize;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t / duration);
            yield return null;
        }

        cam.orthographicSize = targetSize;
        zoomCoroutine = null;
    }

}