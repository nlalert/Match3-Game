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

    public void CalculateScore(List<Candy> matches, PowerUpType powerUpType = PowerUpType.None){
        int matchSize = matches.Count;
        int baseScore = 0;
        
        // Base score for matches
        if (powerUpType == PowerUpType.None){
            baseScore += matchSize * 10;
        } else{
            int powerUpBonus = powerUpType == PowerUpType.LineClear ? 50 : 100;
            baseScore += powerUpBonus;
        }

        // Update total score
        score += baseScore;

        Debug.Log($"Match of {matchSize} candies! Base Score: {baseScore}, Total Score: {score}");
        UpdateScoreUI();
    }

    public void AddScoreForPowerUpActivation(PowerUpType powerUpType, int candiesCleared){
        int baseScorePerCandy = powerUpType == PowerUpType.LineClear ? 20 : 30;
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
