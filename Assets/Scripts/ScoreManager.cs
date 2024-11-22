using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public Slider scoreSlider; // Slider to represent the score
    public TextMeshProUGUI scoreText; // Text to display the numeric score
    public int targetScore = 10000; // Score required to win

    public AnimationManager animationManager;
    public UIManager uiManager;

    private void Start() {
        scoreSlider.maxValue = targetScore;
        scoreSlider.value = score;

        UpdateScoreUI();
    }

    public void CalculateScore(List<Fossil> matches, PowerUpType powerUpType = PowerUpType.None) {
        int matchSize = matches.Count;
        int baseScore = 0;

        // Base score for matches
        if (powerUpType == PowerUpType.None) {
            baseScore += matchSize * 10;
        } else {
            int powerUpBonus = 0;
            switch (powerUpType) {
                case PowerUpType.LineClear:
                    powerUpBonus = 50;
                    break;
                case PowerUpType.Bomb:
                    powerUpBonus = 100;
                    break;
                case PowerUpType.DNA:
                    powerUpBonus = 150; // Higher bonus for DNA
                    break;
            }
            baseScore += powerUpBonus;
        }

        // Update total score
        score += baseScore;

        UpdateScoreUI();

        // Check if the target score is reached
        if (score >= targetScore) {
            WinGame();
        }
    }

    public void AddScoreForPowerUpActivation(PowerUpType powerUpType, int fossilsCleared) {
        int baseScorePerFossil = 0;

        switch (powerUpType) {
            case PowerUpType.LineClear:
                baseScorePerFossil = 15;
                break;
            case PowerUpType.Bomb:
                baseScorePerFossil = 15;
                break;
            case PowerUpType.DNA:
                baseScorePerFossil = 20;
                break;
        }

        int activationScore = fossilsCleared * baseScorePerFossil;

        // Update total score
        score += activationScore;

        UpdateScoreUI();

        // Check if the target score is reached
        if (score >= targetScore) {
            WinGame();
        }
    }

    private void UpdateScoreUI() {
        // Update the slider value
        scoreSlider.value = score;

        // Update the score text
        scoreText.text = $"Score: {score} / {targetScore}";
    }

    private void WinGame() {
        if(uiManager.isPaused) return;

        uiManager.Win();
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound(AudioManager.Instance.gamePass);
    }

    public void GameOver() {
        if(animationManager.isAnimating) return;

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound(AudioManager.Instance.gameOver); // Play game over sound
        uiManager.GameOver();
    }
}
