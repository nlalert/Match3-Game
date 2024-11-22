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
    private GameState state = GameState.Playing;

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

        Debug.Log($"Match of {matchSize} fossils! Power-Up: {powerUpType}, Base Score: {baseScore}, Total Score: {score}");
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
                baseScorePerFossil = 20;
                break;
            case PowerUpType.Bomb:
                baseScorePerFossil = 30;
                break;
            case PowerUpType.DNA:
                baseScorePerFossil = 10;
                break;
        }

        int activationScore = fossilsCleared * baseScorePerFossil;

        // Update total score
        score += activationScore;

        Debug.Log($"Power-Up Activated ({powerUpType})! Cleared {fossilsCleared} fossils, Score: {activationScore}, Total Score: {score}");
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
        if(state == GameState.Playing){
            Debug.Log("Congratulations! You've reached the target score and won the game!");
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySound(AudioManager.Instance.gamePass);
            state = GameState.Won;
        }
    }

    public void GameOver() {
        if(state == GameState.Playing){
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySound(AudioManager.Instance.gameOver); // Play game over sound
            Debug.Log("Game Over! No moves remaining.");
            state = GameState.Lose;
        }
    }
}

public enum GameState {
    Playing,
    Won,
    Lose
}
