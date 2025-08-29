using UnityEngine;
using System.Collections;
public class TankController : MonoBehaviour
{
    [Header("References")]
    public Transform muzzlePoint;  
    public GameObject bulletPrefab; 
    public LayerMask bulletHitMask; 

    [Header("Burst")]
    public int burstCount = 3;
    public float burstInterval = 0.18f;   
    public float burstCooldown = 2.5f;   
    [Header("Bullet")]
    public float bulletSpeed = 12f;
    public float bulletLife = 4f;
    public float fireJitterDegrees = 0f; 
    [Header("Direction (choose)")]
    public bool useAngle = true;
    public float fixedAngleDegrees = 0f; 
    public Vector2 fixedDirection = Vector2.right; 
    [Header("Spawn offset")]
    public Vector3 spawnOffset = Vector3.zero;

    float cooldown;
    public AudioSource audioSource;
    public AudioClip bulletShotSound;

    void Start()
    {
        cooldown = Random.Range(0f, burstCooldown * 0.5f);
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
        {
            StartCoroutine(FireBurst());
            cooldown = burstCooldown;
        }
    }

    IEnumerator FireBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            Vector2 baseDir = GetFixedDirection();
            float jitter = (fireJitterDegrees != 0f) ? Random.Range(-fireJitterDegrees, fireJitterDegrees) : 0f;
            float angle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg + jitter;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            Vector3 spawnPos = (muzzlePoint != null) ? muzzlePoint.position : transform.position;
            spawnPos += spawnOffset;

            SpawnBullet(spawnPos, rot);
            audioSource.PlayOneShot(bulletShotSound);
            yield return new WaitForSeconds(burstInterval);
        }
    }

    Vector2 GetFixedDirection()
    {
        if (useAngle)
        {
            float rad = fixedAngleDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
        }
        else
        {
            return fixedDirection.normalized;
        }
    }

    void SpawnBullet(Vector3 pos, Quaternion rot)
    {
        GameObject b = Instantiate(bulletPrefab, pos, rot);
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        TankBullet tb = b.GetComponent<TankBullet>();
        if (tb != null) tb.hitMask = bulletHitMask;

        if (rb != null)
        {
            Vector2 dir = rot * Vector3.right;
            rb.linearVelocity = dir.normalized * bulletSpeed;
        }
        Destroy(b, bulletLife);
    }
}
