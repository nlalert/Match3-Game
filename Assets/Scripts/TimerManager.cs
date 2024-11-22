using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public UIManager uiManager;
    private float elapsedTime;
    private bool isRunning = false;

    void Start(){
        ResetTimer();
    }

    void Update(){
        if (isRunning){
            elapsedTime += Time.deltaTime;
            UpdateUI();
        }
    }

    public void StartTimer(){
        isRunning = true;
    }

    public void PauseTimer(){
        isRunning = false;
    }

    public void ResetTimer(){
        elapsedTime = 0f;
        UpdateUI();
    }

    public void StopTimer(){
        isRunning = false;
    }

    public float GetElapsedTime(){
        return elapsedTime;
    }

    private void UpdateUI(){
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        uiManager.UpdateTimerText(minutes, seconds);
    }
}
