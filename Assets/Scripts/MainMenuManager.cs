using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    public string gameSceneName = "GameStage";
    public GameObject settingsPanel;
    public Slider masterVolumeSlider;
    public TextMeshProUGUI masterVolumeText;
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeText;
    public Slider sfxVolumeSlider;
    public TextMeshProUGUI sfxVolumeText;
    void Start(){
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

    public void StartGame(){
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings(){
        settingsPanel.SetActive(true);
    }

    public void CloseSettings(){
        settingsPanel.SetActive(false);
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
    
    public void ExitGame(){
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
