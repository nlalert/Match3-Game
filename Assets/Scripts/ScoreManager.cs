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

    public void CalculateScore(List<Candy> matches){
        int points = matches.Count * 10;
        score += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI(){
        scoreText.text = "Score: " + score;
    }
}