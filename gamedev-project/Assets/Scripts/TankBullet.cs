using UnityEngine;

public class TankBullet : MonoBehaviour
{
    public LayerMask hitMask;
    public float damage = 2f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitMask) != 0)
        {
            GameTimer.Instance.RemoveTime(damage);
            Destroy(gameObject);
        }
    }
}