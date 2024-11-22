using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {
    public Slider masterVolumeSlider;
    public TextMeshProUGUI masterVolumeText;
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeText;
    public Slider sfxVolumeSlider;
    public TextMeshProUGUI sfxVolumeText;

    void Start() {
        int masterVolume = Mathf.RoundToInt(AudioManager.Instance.masterVolume * 100);
        int musicVolume = Mathf.RoundToInt(AudioManager.Instance.musicVolume * 100);
        int sfxVolume = Mathf.RoundToInt(AudioManager.Instance.sfxVolume * 100);

        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;

        masterVolumeText.text = masterVolume.ToString();
        musicVolumeText.text = musicVolume.ToString();
        sfxVolumeText.text = sfxVolume.ToString();

        masterVolumeSlider.onValueChanged.AddListener(ChangeMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(ChangeSFXVolume);
    }

    public void ChangeMasterVolume(float value) {
        AudioManager.Instance.SetMasterVolume(value / 100f);
        masterVolumeText.text = Mathf.RoundToInt(value).ToString();
    }

    public void ChangeMusicVolume(float value) {
        AudioManager.Instance.SetMusicVolume(value / 100f);
        musicVolumeText.text = Mathf.RoundToInt(value).ToString();
    }

    public void ChangeSFXVolume(float value) {
        AudioManager.Instance.SetSFXVolume(value / 100f);
        sfxVolumeText.text = Mathf.RoundToInt(value).ToString();
    }
}
