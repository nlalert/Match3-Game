using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour{
    public BoardManager board;
    public ScoreManager scoreManager;
    public PowerUpManager powerUpManager;

    // Find all match of a fossil. (DNA don't match with other DNA)
    public List<Fossil> GetMatches(Fossil fossil){
        if (fossil.type == FossilType.DNA){
            return null;
        }

        HashSet<Fossil> matchedFossils = new HashSet<Fossil>();

        // Get horizontal matches
        List<Fossil> horizontalMatches = GetHorizontalMatches(fossil);
        if (horizontalMatches.Count >= 3) {
            foreach (Fossil fossilElement in horizontalMatches) {
                matchedFossils.Add(fossilElement);
            }
        }

        // Get vertical matches
        List<Fossil> verticalMatches = GetVerticalMatches(fossil);
        if (verticalMatches.Count >= 3) {
            foreach (Fossil fossilElement in verticalMatches) {
                matchedFossils.Add(fossilElement);
            }
        }

        return matchedFossils.Count >= 3 ? new List<Fossil>(matchedFossils) : null;
    }

    // Find all fosiil that horizontal match of a fossil
    private List<Fossil> GetHorizontalMatches(Fossil fossil){
        List<Fossil> matches = new List<Fossil> { fossil };

        // Check left
        for (int i = fossil.x - 1; i >= 0; i--){
            if (board.fossils[i, fossil.y] != null && board.fossils[i, fossil.y].type == fossil.type){
                matches.Add(board.fossils[i, fossil.y]);
            }
            else{
                break;
            }
        }

        // Check right
        for (int i = fossil.x + 1; i < board.width; i++){
            if (board.fossils[i, fossil.y] != null && board.fossils[i, fossil.y].type == fossil.type){
                matches.Add(board.fossils[i, fossil.y]);
            }
            else{
                break;
            }
        }

        return matches;
    }

    // Find all fosiil that vertical match of a fossil
    private List<Fossil> GetVerticalMatches(Fossil fossil){
        List<Fossil> matches = new List<Fossil> { fossil };

        // Check down
        for (int i = fossil.y - 1; i >= 0; i--){
            if (board.fossils[fossil.x, i] != null && board.fossils[fossil.x, i].type == fossil.type){
                matches.Add(board.fossils[fossil.x, i]);
            }
            else{
                break;
            }
        }

        // Check up
        for (int i = fossil.y + 1; i < board.height; i++){
            if (board.fossils[fossil.x, i] != null && board.fossils[fossil.x, i].type == fossil.type){
                matches.Add(board.fossils[fossil.x, i]);
            }
            else{
                break;
            }
        }

        return matches;
    }

    // Destroy matched fossils, update score, and handle power-up
    public void DestroyMatches(List<Fossil> matches){
        if (matches == null || matches.Count < 3) return;

        PlayRandomMatchSound();
        scoreManager.CalculateScore(matches);

        Fossil powerUpFossil = null;

        // Handle power-up creation for larger matches
        if (matches.Count >= 4){
            powerUpFossil = powerUpManager.HandlePowerUpCreation(matches);
        }

        foreach (Fossil fossil in matches){
            // Skip destroying center fossil that use power-up
            if (fossil == powerUpFossil){
                continue;
            }

            // Activate and destroy other power-up
            if (fossil.powerUpType != PowerUpType.None){
                powerUpManager.ActivatePowerUp(fossil);
            }

            board.DestroyFossil(fossil);
        }
    }

    // Play random match sound
    private void PlayRandomMatchSound(){
        int randomSound = Random.Range(0, 4);
        switch (randomSound){
            case 0:
                AudioManager.Instance.PlaySound(AudioManager.Instance.match1);
                break;
            case 1:
                AudioManager.Instance.PlaySound(AudioManager.Instance.match2);
                break;
            case 2:
                AudioManager.Instance.PlaySound(AudioManager.Instance.match3);
                break;
            default:
                AudioManager.Instance.PlaySound(AudioManager.Instance.match4);
                break;
        }
    }
}
