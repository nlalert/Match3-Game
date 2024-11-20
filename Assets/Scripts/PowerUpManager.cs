using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {
    public BoardManager board;
    public MatchManager matchManager;

    public Candy HandlePowerUpCreation(List<Candy> matches) {
        if (matches == null || matches.Count < 4) return null;

        // Determine the central candy of the match
        Candy centralCandy = matches[matches.Count / 2];

        // Create a LineClear power-up for matches of exactly 4 candies
        if (matches.Count == 4) {
            CreatePowerUp(centralCandy, PowerUpType.LineClear);
            Debug.Log("Power-Up Created: Line Clear");
            return centralCandy;
        } 

        // Create a Bomb power-up for matches of 5 or more candies
        if (matches.Count >= 5) {
            CreatePowerUp(centralCandy, PowerUpType.Bomb);
            Debug.Log("Power-Up Created: Bomb");
            return centralCandy;
        }

        return null;
    }

    private void CreatePowerUp(Candy candy, PowerUpType powerUpType) {
        // Assign the power-up type to the candy
        candy.SetPowerUp(powerUpType);

        // Visual feedback for power-up
        // Replace this with a unique appearance for the power-up (e.g., special sprite or animation)
        candy.GetComponent<SpriteRenderer>().color = powerUpType == PowerUpType.LineClear ? Color.black : Color.white;
    }

    public void ActivatePowerUp(Candy candy) {
        if (candy.powerUpType == PowerUpType.None) return;

        switch (candy.powerUpType) {
            case PowerUpType.LineClear:
                //ClearLine(candy);
                break;
            case PowerUpType.Bomb:
                //ExplodeAround(candy);
                break;
        }
    }
}
