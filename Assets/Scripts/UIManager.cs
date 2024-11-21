using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    void Start(){
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OnResumeButtonPressed(){
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OnSettingsButtonPressed(){
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnMainMenuButtonPressed(){
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void TogglePause(){
        isPaused = !isPaused;
        if(isPaused)
            PauseGame();
        else
            OnResumeButtonPressed();
    }

    public void PauseGame(){
        isPaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void CloseSettings(){
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
