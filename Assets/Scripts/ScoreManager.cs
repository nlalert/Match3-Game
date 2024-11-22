using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {
    public int score = 0;
    public int targetScore = 10000;

    public Slider scoreSlider; 
    public TextMeshProUGUI scoreText; 

    public AnimationManager animationManager;
    public UIManager uiManager;

    private void Start(){
        InitializeScoreUI();
    }


    // Calculate the score based on matches and power-up
    public void CalculateScore(List<Fossil> matches, PowerUpType powerUpType = PowerUpType.None){
        if (matches == null) return;

        int baseScore = matches.Count * 10;

        baseScore += GetPowerUpBonus(powerUpType);

        AddScore(baseScore);
    }

    // Add score based on power-up activation and the number of cleared fossils
    public void AddScoreForPowerUpActivation(PowerUpType powerUpType, int fossilsCleared){
        int baseScorePerFossil = GetBaseScorePerFossil(powerUpType);
        AddScore(fossilsCleared * baseScorePerFossil);
    }

    // Update ui
    private void UpdateScoreUI(){
        scoreSlider.value = score;
        scoreText.text = $"Score: {score} / {targetScore}";
    }

    private void WinGame(){
        if (uiManager.isPaused) return;

        uiManager.Win();
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound(AudioManager.Instance.gamePass);
    }

    public void GameOver(){
        if (animationManager.isAnimating) return;

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound(AudioManager.Instance.gameOver);
        uiManager.GameOver();
    }

    // Initialize score UI
    private void InitializeScoreUI(){
        scoreSlider.maxValue = targetScore;
        scoreSlider.value = score;
        UpdateScoreUI();
    }

    // Add score and check for win
    private void AddScore(int points){
        score += points;
        UpdateScoreUI();

        if (score >= targetScore){
            WinGame();
        }
    }

    // Get score bonus
    private int GetPowerUpBonus(PowerUpType powerUpType){
        switch (powerUpType){
            case PowerUpType.LineClear: 
                return 50;
            case PowerUpType.Bomb: 
                return 100;
            case PowerUpType.DNA: 
                return 150;
            default:
                return 0;
        }
    }

    /// Get base score per fossil
    private int GetBaseScorePerFossil(PowerUpType powerUpType){
        switch (powerUpType){
            case PowerUpType.LineClear: 
                return 15;
            case PowerUpType.Bomb: 
                return 15;
            case PowerUpType.DNA: 
                return 20;
            default:
                return 0;
        }
    }
}
