using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    public AudioClip stageMusic;

    public AudioClip swapSound;
    public AudioClip matchSound1;
    public AudioClip matchSound2;
    public AudioClip matchSound3;
    public AudioClip matchSound4;
    public AudioClip fallSound;
    public AudioClip stripeCreatedSound;
    public AudioClip wrapCreatedSound;
    public AudioClip colorBombCreatedSound;
    public AudioClip gameOverSound;

    private AudioSource audioSource;

    [Range(0f, 1f)] // Slider in the Inspector for volume
    public float volume = 1f; // Default to maximum volume

    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure persistence
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start(){
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set the initial volume of the AudioSource
        audioSource.volume = volume;
    }

    public void PlaySound(AudioClip clip){
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume); // Use the current volume level
        }
    }

    public void SetVolume(float newVolume){
        volume = Mathf.Clamp01(newVolume); // Ensure the volume is between 0 and 1
        audioSource.volume = volume; // Update the AudioSource volume
    }
}
