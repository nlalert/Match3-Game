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
    public AudioClip gamePass;

    private AudioSource musicSource; // for background music
    private AudioSource sfxSource;  // for sound effects

    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;

    // make audio stay for every scene
    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    // setup audio sources and sets initial volume levels
    private void Start(){
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        UpdateVolumes();
    }

    // Plays music, stop current music first
    public void PlayMusic(AudioClip clip){
        StopMusic();
        musicSource.clip = clip;
        musicSource.Play();
    }

    // stop current music
    public void StopMusic(){
        musicSource.Stop();
    }

    // Plays sound effect once
    public void PlaySound(AudioClip clip){
        if (clip != null){
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }
    }

    // Set master volume
    public void SetMasterVolume(float value){
        masterVolume = Mathf.Clamp01(value);
        UpdateVolumes();
    }

    // Set music volume
    public void SetMusicVolume(float value){
        musicVolume = Mathf.Clamp01(value);
        UpdateVolumes();
    }

    // Set SFX volume and update the SFX source
    public void SetSFXVolume(float value){
        sfxVolume = Mathf.Clamp01(value);
        UpdateVolumes();
    }

    // Update volume of the music and SFX audio source
    private void UpdateVolumes(){
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;
    }

    // Pause all audio
    public void PauseAudio(){
        musicSource.Pause();
        sfxSource.Pause();
    }

    // Resume all paused audio
    public void ResumeAudio(){
        musicSource.UnPause();
        sfxSource.UnPause();
    }
}
