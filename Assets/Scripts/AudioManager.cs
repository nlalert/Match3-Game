using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    public AudioClip stageMusic;

    public AudioClip swap;
    public AudioClip match1;
    public AudioClip match2;
    public AudioClip match3;
    public AudioClip match4;
    public AudioClip stripeCreated;
    public AudioClip stripeBlast;
    public AudioClip wrapCreated;
    public AudioClip wrapBlast;
    public AudioClip colorBombCreated;
    public AudioClip colorBombBlast;
    public AudioClip gameOver;

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
