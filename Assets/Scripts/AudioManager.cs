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

    private AudioSource musicSource;
    private AudioSource sfxSource;

    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        UpdateVolumes();
    }

    public void PlayMusic(AudioClip clip) {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip) {
        if (clip != null) {
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }
    }

    public void SetMasterVolume(float value) {
        masterVolume = Mathf.Clamp01(value);
        UpdateVolumes();
    }

    public void SetMusicVolume(float value) {
        musicVolume = Mathf.Clamp01(value);
        UpdateVolumes();
    }

    public void SetSFXVolume(float value) {
        sfxVolume = Mathf.Clamp01(value);
        UpdateVolumes();
    }

    private void UpdateVolumes() {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;
    }

    public void PauseAudio() {
        musicSource.Pause();
        sfxSource.Pause();
    }

    public void ResumeAudio() {
        musicSource.UnPause();
        sfxSource.UnPause();
    }
}
