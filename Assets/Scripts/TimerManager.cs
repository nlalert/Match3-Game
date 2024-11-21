using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime;
    private bool isRunning = false;

    void Start(){
        ResetTimer();
    }

    void Update(){
        if (isRunning){
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
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
        UpdateTimerUI();
    }

    public void StopTimer(){
        isRunning = false;
    }

    public float GetElapsedTime(){
        return elapsedTime;
    }

    private void UpdateTimerUI(){
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }
}
