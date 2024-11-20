using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public int score { get; private set; }
    public TextMeshProUGUI scoreText;

    private void Start(){
        score = 0;
        UpdateScoreUI();
    }

    public void CalculateScore(List<Candy> matches, PowerUpType powerUpType = PowerUpType.None) {
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

        Debug.Log($"Match of {matchSize} candies! Power-Up: {powerUpType}, Base Score: {baseScore}, Total Score: {score}");
        UpdateScoreUI();
    }

    public void AddScoreForPowerUpActivation(PowerUpType powerUpType, int candiesCleared) {
        int baseScorePerCandy = 0;

        switch (powerUpType) {
            case PowerUpType.LineClear:
                baseScorePerCandy = 20;
                break;
            case PowerUpType.Bomb:
                baseScorePerCandy = 30;
                break;
            case PowerUpType.DNA:
                baseScorePerCandy = 40; // Higher score per candy cleared by DNA
                break;
        }

        int activationScore = candiesCleared * baseScorePerCandy;

        // Update total score
        score += activationScore;

        Debug.Log($"Power-Up Activated ({powerUpType})! Cleared {candiesCleared} candies, Score: {activationScore}, Total Score: {score}");
        UpdateScoreUI();
    }

    private void UpdateScoreUI(){
        scoreText.text = "Score: " + score;
    }
}
