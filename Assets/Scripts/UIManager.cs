using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public TextMeshProUGUI gameOverTimerText;
    public TextMeshProUGUI winTimerText;
    public bool isPaused = false;

    //Resume the game and hiding pause and settings panel
    public void Resume() {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        AudioManager.Instance.ResumeAudio();
    }
        
    // Pause the game and display pause panel
    public void Pause() {
        isPaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);

        AudioManager.Instance.PauseAudio();
    }

    // Display win panel and pause the game
    public void Win() {
        isPaused = true;
        Time.timeScale = 0;
        winPanel.SetActive(true);
    }

    // Display game over panel and pause the game
    public void GameOver() {
        isPaused = true;
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
    }

    // Toggle between pause and resume
    public void TogglePause() {
        if(IsGameEnd()) return;

        if (isPaused)
            Resume();
        else
            Pause();
    }

    // Check if the game has ended
    private bool IsGameEnd(){
        return winPanel.activeSelf || gameOverPanel.activeSelf;
    }

    // Restart the game scene
    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Chang to the main menu scene
    public void ChangeToMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    // Open the settings panel and hide the pause panel
    public void OpenSettings() {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Close the settings panel and show to the pause panel
    public void CloseSettings() {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // Update the timer text on both the win and game over panels
    public void UpdateTimerText(int minutes, int seconds){
        gameOverTimerText.text = $"Played Time: {minutes:00}:{seconds:00}";
        winTimerText.text = $"Played Time: {minutes:00}:{seconds:00}";
    }
}
