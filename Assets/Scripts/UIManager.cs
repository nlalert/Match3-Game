using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;
    
    public Slider masterVolumeSlider;
    public TextMeshProUGUI masterVolumeText;
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeText;
    public Slider sfxVolumeSlider;
    public TextMeshProUGUI sfxVolumeText;

    public bool isPaused = false;

    void Start(){
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        int masterVolume = Mathf.RoundToInt(AudioManager.Instance.masterVolume * 100);
        int musicVolume = Mathf.RoundToInt(AudioManager.Instance.musicVolume * 100);
        int sfxVolume = Mathf.RoundToInt(AudioManager.Instance.sfxVolume * 100);
        
        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;

        masterVolumeText.text = masterVolume.ToString();
        musicVolumeText.text = musicVolume.ToString();
        sfxVolumeText.text = sfxVolume.ToString();
    }

    public void Resume(){
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        AudioManager.Instance.ResumeAudio();
    }

    public void Pause(){
        isPaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);

        AudioManager.Instance.PauseAudio();
    }

    public void ChangeToMainMenu(){
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void TogglePause(){
        isPaused = !isPaused;
        if(isPaused)
            Pause();
        else
            Resume();
    }

    public void Restart(){
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void OpenSettings(){
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings(){
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ChangeMasterVolume(float value){
        AudioManager.Instance.SetMasterVolume(value / 100f);
        masterVolumeText.text = value.ToString();
    }

    public void ChangeMusicVolume(float value){
        AudioManager.Instance.SetMusicVolume(value / 100f);
        musicVolumeText.text = value.ToString();
    }

    public void ChangeSFXVolume(float value){
        AudioManager.Instance.SetSFXVolume(value / 100f);
        sfxVolumeText.text = value.ToString();
    }
}
