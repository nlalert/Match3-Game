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
                baseScorePerFossil = 40; // Higher score per fossil cleared by DNA
                break;
        }

        int activationScore = fossilsCleared * baseScorePerFossil;

        // Update total score
        score += activationScore;

        Debug.Log($"Power-Up Activated ({powerUpType})! Cleared {fossilsCleared} fossils, Score: {activationScore}, Total Score: {score}");
        UpdateScoreUI();
    }

    private void UpdateScoreUI(){
        scoreText.text = "Score: " + score;
    }
}
