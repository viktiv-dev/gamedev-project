using UnityEngine;

public class DestroyAnchorPoint : MonoBehaviour
{
    public float timeToDestroy = 2f;
    private AnchorPoint anchorPoint;
    public AudioSource audioSource;
    private bool isDestroying = false;
    public AudioClip destroySound;

    void Awake()
    {
        anchorPoint = GetComponent<AnchorPoint>();
    }
    
    public void StartDestroyCountdown()
    {
        if (isDestroying) return; 
        isDestroying = true;
        audioSource.PlayOneShot(destroySound);
        Invoke(nameof(DestroySelf), timeToDestroy);
    }

    private void DestroySelf()
    {
        if (transform.parent != null)
            Destroy(transform.parent.gameObject);
        else
            Destroy(gameObject);
    }
}