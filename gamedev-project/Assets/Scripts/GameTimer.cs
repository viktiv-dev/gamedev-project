using UnityEngine;
using System;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    public float timeLeft = 10f;

    public event Action OnTimeAdded;
    public event Action OnTimeRemoved;
    public event Action OnGameOver;

    [Header("Audio")]
    public AudioSource audioSource;          // single audio source to play beeps and damage
    public AudioClip timeAddedClip;
    public AudioClip timeRemovedClip;
    public float pitchIncrease = 0.2f;       // how much pitch increases for fast additions
    public float fastThreshold = 3f;  
    public float pitchThreshold = 2;

    private float lastTimeAdded = -10f;      // track last time a time bonus occurred
    private bool gameOverTriggered = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (gameOverTriggered) return;

        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                TriggerGameOver();
            }
        }
    }

    public void AddTime(float amount)
    {
        if (gameOverTriggered) return;

        float currentTime = Time.time;
        timeLeft += amount;

        // Handle fast-add pitch
        if (audioSource != null && timeAddedClip != null)
        {
            float pitch = audioSource.pitch;

            if (currentTime - lastTimeAdded < fastThreshold && pitch <= pitchThreshold) 
            {
                float factor = (fastThreshold - (currentTime - lastTimeAdded)) / fastThreshold;
                pitch += factor * pitchIncrease;
            }

            audioSource.pitch = pitch;
            audioSource.PlayOneShot(timeAddedClip);
        }

        lastTimeAdded = currentTime;
        OnTimeAdded?.Invoke();
    }

    public void RemoveTime(float amount)
    {
        if (gameOverTriggered) return;

        timeLeft -= amount;

        if (audioSource != null && timeRemovedClip != null)
        {
            audioSource.pitch = 1f; // reset pitch
            audioSource.PlayOneShot(timeRemovedClip);
        }

        OnTimeRemoved?.Invoke();

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        gameOverTriggered = true;
        OnGameOver?.Invoke();
        Debug.Log("Game Over!");
        // You can also call a GameManager method here to show Game Over UI
    }
}
