using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour{

    public BoardManager board;
    public ScoreManager scoreManager;
    public PowerUpManager powerUpManager;

    public List<Fossil> GetMatches(Fossil fossil) {
        HashSet<Fossil> matchedFossils = new HashSet<Fossil>();

        // Get horizontal matches
        List<Fossil> horizontalMatches = GetHorizontalMatches(fossil);
        if (horizontalMatches.Count >= 3) {
            foreach (Fossil c in horizontalMatches) {
                matchedFossils.Add(c);
            }
        }

        // Get vertical matches
        List<Fossil> verticalMatches = GetVerticalMatches(fossil);
        if (verticalMatches.Count >= 3) {
            foreach (Fossil c in verticalMatches) {
                matchedFossils.Add(c);
            }
        }

        // Return the combined list of matches
        return matchedFossils.Count >= 3 ? new List<Fossil>(matchedFossils) : null;
    }

    private List<Fossil> GetHorizontalMatches(Fossil fossil) {
        List<Fossil> matches = new List<Fossil> { fossil };

        // Check left
        for (int i = fossil.x - 1; i >= 0; i--) {
            if (board.fossils[i, fossil.y] != null && board.fossils[i, fossil.y].type == fossil.type) {
                matches.Add(board.fossils[i, fossil.y]);
            } else {
                break;
            }
        }

        // Check right
        for (int i = fossil.x + 1; i < board.width; i++) {
            if (board.fossils[i, fossil.y] != null && board.fossils[i, fossil.y].type == fossil.type) {
                matches.Add(board.fossils[i, fossil.y]);
            } else {
                break;
            }
        }

        return matches;
    }

    private List<Fossil> GetVerticalMatches(Fossil fossil) {
        List<Fossil> matches = new List<Fossil> { fossil };

        // Check down
        for (int i = fossil.y - 1; i >= 0; i--) {
            if (board.fossils[fossil.x, i] != null && board.fossils[fossil.x, i].type == fossil.type) {
                matches.Add(board.fossils[fossil.x, i]);
            } else {
                break;
            }
        }

        // Check up
        for (int i = fossil.y + 1; i < board.height; i++) {
            if (board.fossils[fossil.x, i] != null && board.fossils[fossil.x, i].type == fossil.type) {
                matches.Add(board.fossils[fossil.x, i]);
            } else {
                break;
            }
        }

        return matches;
    }

    public void DestroyMatches(List<Fossil> matches) {
        if (matches == null || matches.Count < 3) return;

        AudioManager.Instance.PlaySound(AudioManager.Instance.matchSound); // Play match sound
        scoreManager.CalculateScore(matches);

        Fossil powerUpFossil = null;

        if (matches.Count >= 4) {
            powerUpFossil = powerUpManager.HandlePowerUpCreation(matches); // Create a LineClear power-up
        }

        foreach (Fossil fossil in matches) {
            if (fossil == powerUpFossil) {
                continue; // Skip destroying the central fossil
            }

            if (fossil.powerUpType != PowerUpType.None) {
                powerUpManager.ActivatePowerUp(fossil); // Activate any other power-ups
            }
            board.DestroyFossil(fossil);
        }
    }
}
