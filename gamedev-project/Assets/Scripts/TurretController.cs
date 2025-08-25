using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class TurretController : MonoBehaviour
{
    public Transform gunPivot;
    public Transform gunSprite;
    public Transform player;
    public LineRenderer warningLine;
    public LineRenderer fireLine;
    public LayerMask hitMask;

    public float fireInterval = 3f;
    public float chargeTime = 0.6f;
    public float detectionRange = 20f; // small detection range
    public float lineRange = 100f;     // very long laser
    public float angleOffset = 0f;
    public bool useChargeWarning = true;
    public bool rotateContinuously = true;

    public float rotationSpeed = 180f;

    public int warningPulses = 3;
    public Color warningColor = new Color(1f, 0.8f, 0.2f, 0.25f);
    public float warningWidth = 0.06f;

    public Color fireColor = Color.red;
    public float fireWidth = 0.08f;
    public float finalVisibleTime = 1f;
    public float finalDelay = 0.2f; // small delay before final shot

    public bool invertDirection = true;

    float cooldown;
    bool isFiring;

    void Start()
    {
        cooldown = Random.Range(0f, fireInterval * 0.5f);
        InitLine(warningLine, warningColor, warningWidth);
        InitLine(fireLine, fireColor, fireWidth);
        warningLine.gameObject.SetActive(false);
        fireLine.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector2 toPlayer = player.position - gunPivot.position;

        if (toPlayer.magnitude <= detectionRange)
        {
            float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg + angleOffset;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, targetAngle);

            if (rotateContinuously || !isFiring)
                gunPivot.rotation = Quaternion.RotateTowards(gunPivot.rotation, targetRot, rotationSpeed * Time.deltaTime);

            cooldown -= Time.deltaTime;
            if (cooldown <= 0f && !isFiring)
            {
                StartCoroutine(FireRoutine());
                cooldown = fireInterval;
            }
        }
    }

    IEnumerator FireRoutine()
    {
        isFiring = true;

        // --- Warning pulses ---
        if (useChargeWarning && warningPulses > 0)
        {
            float half = Mathf.Max(0.01f, chargeTime / (warningPulses * 2f));
            for (int i = 0; i < warningPulses; i++)
            {
                Vector2 dir = GetMuzzleDirection();
                Vector3 start = gunPivot.position;
                Vector3 end = start + new Vector3(dir.x, dir.y, 0f) * lineRange;

                warningLine.gameObject.SetActive(true);
                warningLine.startWidth = warningWidth;
                warningLine.endWidth = warningWidth;
                warningLine.startColor = warningColor;
                warningLine.endColor = warningColor;
                warningLine.sortingOrder = 100;

                float t = 0f;
                while (t < half)
                {
                    start = gunPivot.position;
                    dir = GetMuzzleDirection();
                    end = start + new Vector3(dir.x, dir.y, 0f) * lineRange;
                    warningLine.SetPosition(0, start);
                    warningLine.SetPosition(1, end);
                    t += Time.deltaTime;
                    yield return null;
                }

                warningLine.gameObject.SetActive(false);
                t = 0f;
                while (t < half) { t += Time.deltaTime; yield return null; }
            }
        }
        else if (chargeTime > 0f)
        {
            yield return new WaitForSeconds(chargeTime);
        }

        // --- Freeze turret rotation for final delay ---
        bool prevRotate = rotateContinuously;
        rotateContinuously = false;

        // --- Final warning visible ---
        Vector2 finalDir = GetMuzzleDirection();
        Vector3 finalStart = gunPivot.position;
        Vector3 finalEnd = finalStart + new Vector3(finalDir.x, finalDir.y, 0f) * lineRange;
        warningLine.gameObject.SetActive(true);
        warningLine.SetPosition(0, finalStart);
        warningLine.SetPosition(1, finalEnd);

        // --- Small final delay ---
        yield return new WaitForSeconds(finalDelay);

        // --- Blink warning -> fire ---
        RaycastHit2D hit = Physics2D.Raycast(finalStart, finalDir, detectionRange, hitMask);
        if (hit.collider != null) finalEnd = hit.point;

        fireLine.gameObject.SetActive(true);
        fireLine.SetPosition(0, finalStart);
        fireLine.SetPosition(1, finalEnd);

        // quick blink: hide warning for a frame
        warningLine.gameObject.SetActive(false);
        yield return null;

        // show fire for the full duration
        float elapsed = 0f;
        while (elapsed < finalVisibleTime)
        {
            fireLine.SetPosition(0, gunPivot.position);
            fireLine.SetPosition(1, finalEnd);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fireLine.gameObject.SetActive(false);
        warningLine.gameObject.SetActive(false);

        rotateContinuously = prevRotate;
        isFiring = false;
    }

    Vector2 GetMuzzleDirection()
    {
        Vector3 dir3 = gunPivot.right;
        if (invertDirection) dir3 = -dir3;
        return new Vector2(dir3.x, dir3.y).normalized;
    }

    void InitLine(LineRenderer lr, Color color, float width)
    {
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.gameObject.SetActive(false);
    }
}
