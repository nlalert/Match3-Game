using UnityEngine;

public class TimerManager : MonoBehaviour {
    public UIManager uiManager;
    private float elapsedTime = 0f;
    private bool isTimerRunning = false;

    private void Start(){
        ResetTimer();
    }

    private void Update(){
        if (!isTimerRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateUITimer();
    }

    // Start the timer
    public void StartTimer(){
        isTimerRunning = true;
    }

    // Pause the timer
    public void PauseTimer(){
        isTimerRunning = false;
    }

    // Reset the timer to zero and update UI
    public void ResetTimer(){
        elapsedTime = 0f;
        UpdateUITimer();
    }

    // Stop the timer
    public void StopTimer(){
        isTimerRunning = false;
    }

    // Update the timer text on UI
    private void UpdateUITimer(){
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        uiManager.UpdateTimerText(minutes, seconds);
    }
}
