using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {
    public BoardManager board;
    public MatchManager matchManager;
    public ScoreManager scoreManager;

    public Candy HandlePowerUpCreation(List<Candy> matches) {
        if (matches == null || matches.Count < 4) return null;

        // Determine the central candy of the match based on swap pair or the center of the match list
        Candy centralCandy = GetCentralCandy(matches);

        // Create appropriate power-ups based on the match
        if (IsDNA(matches)) {
            CreatePowerUp(centralCandy, PowerUpType.DNA);
            scoreManager.CalculateScore(matches, PowerUpType.DNA);
            Debug.Log("Power-Up Created: DNA");
            return centralCandy;
        }

        if (matches.Count == 4) {
            CreatePowerUp(centralCandy, PowerUpType.LineClear);
            scoreManager.CalculateScore(matches, PowerUpType.LineClear);
            Debug.Log("Power-Up Created: Line Clear");
            return centralCandy;
        } 

        if (matches.Count >= 5) {
            CreatePowerUp(centralCandy, PowerUpType.Bomb);
            scoreManager.CalculateScore(matches, PowerUpType.Bomb);
            Debug.Log("Power-Up Created: Bomb");
            return centralCandy;
        }

        return null;
    }

    private Candy GetCentralCandy(List<Candy> matches) {
        Candy centralCandy = null;

        // Check if the swapped candies are part of the matches, if so use one of them as central candy
        if (board.swapPair[0] != null && matches.Contains(board.swapPair[0])) {
            centralCandy = board.swapPair[0];
            board.swapPair[0] = null;
            Debug.Log("Central Candy from Swap: " + centralCandy.name);
        }
        else if (board.swapPair[1] != null && matches.Contains(board.swapPair[1])) {
            centralCandy = board.swapPair[1];
            board.swapPair[1] = null;
            Debug.Log("Central Candy from Swap: " + centralCandy.name);
        }
        else {
            // If no swapped candy is the central candy, pick the middle one from the match
            centralCandy = matches[matches.Count / 2];
            Debug.Log("Center Candy: " + centralCandy.name);
        }

        return centralCandy;
    }

    private bool IsDNA(List<Candy> matches) {
        if(matches.Count != 5) return false;

        int startX = matches[0].x;
        int startY = matches[0].y;

        bool sameRow = matches.TrueForAll(c => c.y == startY);

        bool sameColumn = matches.TrueForAll(c => c.x == startX);

        return sameRow || sameColumn;
    }

    private void CreatePowerUp(Candy candy, PowerUpType powerUpType) {
        // Assign the power-up type to the candy
        candy.SetPowerUp(powerUpType);

        // Provide visual feedback for the power-up (e.g., color change)
        candy.GetComponent<SpriteRenderer>().color = powerUpType == PowerUpType.LineClear ? Color.blue : Color.red;
    }

    public void ActivatePowerUp(Candy candy) {
        if (candy.powerUpType == PowerUpType.None) return;
        switch (candy.powerUpType) {
            case PowerUpType.LineClear:
                ClearLine(candy);
                break;
            case PowerUpType.Bomb:
                ExplodeAround(candy);
                break;
            case PowerUpType.DNA:
                ActivateDNA(candy);
                break;
        }
    }

    private void ClearLine(Candy candy) {
        int x = candy.x;
        int y = candy.y;

        List<Candy> clearedCandies = new List<Candy>();

        // Clear the row
        for (int i = 0; i < board.width; i++) {
            clearedCandies.Add(board.candies[i, y]);
            ClearCandy(i, y);
        }

        // Clear the column
        for (int j = 0; j < board.height; j++) {
            clearedCandies.Add(board.candies[x, j]);
            ClearCandy(x, j);
        }

        scoreManager.AddScoreForPowerUpActivation(PowerUpType.LineClear, clearedCandies.Count);
        Debug.Log($"LineClear activated at ({x}, {y})");
    }

    private void ExplodeAround(Candy candy) {
        int x = candy.x;
        int y = candy.y;

        List<Candy> clearedCandies = new List<Candy>();

        for (int i = x - 2; i <= x + 2; i++) {
            for (int j = y - 2; j <= y + 2; j++) {
                if (board.IsInBoard(i, j)) {
                    clearedCandies.Add(board.candies[i, j]);
                    ClearCandy(i, j);
                }
            }
        }

        scoreManager.AddScoreForPowerUpActivation(PowerUpType.Bomb, clearedCandies.Count);
        Debug.Log($"Bomb activated at ({x}, {y})");
    }

    private void ActivateDNA(Candy candy) {
        List<Candy> clearedCandies = new List<Candy>();

        // Clear all candies of the same type as the DNA candy
        foreach (Candy boardCandy in board.candies) {
            if (boardCandy != null && boardCandy.type == candy.type) {
                clearedCandies.Add(boardCandy);
                ClearCandy(boardCandy.x, boardCandy.y);
            }
        }

        scoreManager.AddScoreForPowerUpActivation(PowerUpType.DNA, clearedCandies.Count);
        Debug.Log($"DNA activated: Cleared {clearedCandies.Count} candies of type {candy.type}");
    }
    
    private void ClearCandy(int x, int y) {
        Candy targetCandy = board.candies[x, y];

        if (targetCandy != null) {
            board.candies[x, y] = null;  // Remove candy from the board
            Destroy(targetCandy.gameObject);  // Destroy the candy object
            Debug.Log($"Cleared candy at ({x}, {y})");
        }
    }
}
