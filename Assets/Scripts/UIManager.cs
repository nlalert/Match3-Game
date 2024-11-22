using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public bool isPaused = false;

    public void Resume() {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        AudioManager.Instance.ResumeAudio();
    }

    public void Pause() {
        isPaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);

        AudioManager.Instance.PauseAudio();
    }

    public void TogglePause() {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeToMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings() {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings() {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
