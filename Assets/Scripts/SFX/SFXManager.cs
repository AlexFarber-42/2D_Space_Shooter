using UnityEngine;

/// <summary>
/// Handles the sound effects from various unique sources and game events.
/// </summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    // TODO ---> Make this be more inclusive of multiple audio sources for different types of SFX (e.g., UI sounds, game sounds, ambient sounds, etc.)
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
