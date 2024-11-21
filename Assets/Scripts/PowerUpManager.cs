using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {
    public BoardManager board;
    public MatchManager matchManager;
    public ScoreManager scoreManager;

    public StripeSpriteManager stripeSpriteManager;
    public BombSpriteManager bombSpriteManager;
    public DNASpriteManager dnaSpriteManager;

    public Fossil HandlePowerUpCreation(List<Fossil> matches) {
        if (matches == null || matches.Count < 4) return null;

        // Determine the central fossil of the match based on swap pair or the center of the match list
        Fossil centralFossil = GetCentralFossil(matches);

        // Create appropriate power-ups based on the match
        if (IsDNA(matches)) {
            CreatePowerUp(centralFossil, PowerUpType.DNA);
            AudioManager.Instance.PlaySound(AudioManager.Instance.colorBombCreatedSound);
            scoreManager.CalculateScore(matches, PowerUpType.DNA);
            Debug.Log("Power-Up Created: DNA");
            return centralFossil;
        }

        if (matches.Count == 4) {
            CreatePowerUp(centralFossil, PowerUpType.LineClear);
            AudioManager.Instance.PlaySound(AudioManager.Instance.stripeCreatedSound);
            scoreManager.CalculateScore(matches, PowerUpType.LineClear);
            Debug.Log("Power-Up Created: Line Clear");
            return centralFossil;
        } 

        if (matches.Count >= 5) {
            CreatePowerUp(centralFossil, PowerUpType.Bomb);
            AudioManager.Instance.PlaySound(AudioManager.Instance.wrapCreatedSound);
            scoreManager.CalculateScore(matches, PowerUpType.Bomb);
            Debug.Log("Power-Up Created: Bomb");
            return centralFossil;
        }

        return null;
    }

    private Fossil GetCentralFossil(List<Fossil> matches) {
        Fossil centralFossil = null;

        // Check if the swapped fossils are part of the matches, if so use one of them as central fossil
        if (board.swapPair[0] != null && matches.Contains(board.swapPair[0])) {
            centralFossil = board.swapPair[0];
            board.swapPair[0] = null;
            Debug.Log("Central Fossil from Swap: " + centralFossil.name);
        }
        else if (board.swapPair[1] != null && matches.Contains(board.swapPair[1])) {
            centralFossil = board.swapPair[1];
            board.swapPair[1] = null;
            Debug.Log("Central Fossil from Swap: " + centralFossil.name);
        }
        else {
            // If no swapped fossil is the central fossil, pick the middle one from the match
            centralFossil = matches[matches.Count / 2];
            Debug.Log("Center Fossil: " + centralFossil.name);
        }

        return centralFossil;
    }

    private bool IsDNA(List<Fossil> matches) {
        if(matches.Count != 5) return false;

        int startX = matches[0].x;
        int startY = matches[0].y;

        bool sameRow = matches.TrueForAll(c => c.y == startY);

        bool sameColumn = matches.TrueForAll(c => c.x == startX);

        return sameRow || sameColumn;
    }

    private void CreatePowerUp(Fossil fossil, PowerUpType powerUpType) {
        // Assign the power-up type to the fossil
        fossil.SetPowerUp(powerUpType);

        // Change the sprite based on the power-up type
        SpriteRenderer spriteRenderer = fossil.GetComponent<SpriteRenderer>();
        switch (powerUpType) {
            case PowerUpType.LineClear:
                spriteRenderer.sprite = GetStripeSprite(fossil.type);
                break;
            case PowerUpType.Bomb:
                spriteRenderer.sprite = GetBombSprite(fossil.type);
                break;
            case PowerUpType.DNA:
                spriteRenderer.sprite = dnaSpriteManager.GetSprite();
                fossil.type = FossilType.DNA;
                break;
            default:
                Debug.LogWarning("Unknown PowerUpType. No sprite assigned.");
                break;
        }
    }

    private Sprite GetStripeSprite(FossilType type) {
        return stripeSpriteManager.GetSprite(type);
    }

    private Sprite GetBombSprite(FossilType type) {
        return bombSpriteManager.GetSprite(type);
    }

    public void ActivatePowerUp(Fossil fossil) {
        if (fossil.powerUpType == PowerUpType.None) return;
        switch (fossil.powerUpType) {
            case PowerUpType.LineClear:
                ClearLine(fossil);
                break;
            case PowerUpType.Bomb:
                ExplodeAround(fossil);
                break;
            case PowerUpType.DNA:
                ActivateDNA(fossil);
                break;
        }
    }

    private void ClearLine(Fossil fossil) {
        int x = fossil.x;
        int y = fossil.y;

        List<Fossil> clearedFossils = new List<Fossil>();

        // Clear the row
        for (int i = 0; i < board.width; i++) {
            clearedFossils.Add(board.fossils[i, y]);
            ClearFossil(board.fossils[i, y]);
        }

        // Clear the column
        for (int j = 0; j < board.height; j++) {
            clearedFossils.Add(board.fossils[x, j]);
            ClearFossil(board.fossils[x, j]);
        }

        scoreManager.AddScoreForPowerUpActivation(PowerUpType.LineClear, clearedFossils.Count);
        Debug.Log($"LineClear activated at ({x}, {y})");
    }

    private void ExplodeAround(Fossil fossil) {
        int x = fossil.x;
        int y = fossil.y;

        List<Fossil> clearedFossils = new List<Fossil>();

        for (int i = x - 2; i <= x + 2; i++) {
            for (int j = y - 2; j <= y + 2; j++) {
                if (board.IsInBoard(i, j)) {
                    clearedFossils.Add(board.fossils[i, j]);
                    ClearFossil(board.fossils[i, j]);
                }
            }
        }

        scoreManager.AddScoreForPowerUpActivation(PowerUpType.Bomb, clearedFossils.Count);
        Debug.Log($"Bomb activated at ({x}, {y})");
    }
    
    public void ActivateDNA(Fossil swappedFossil) {
        List<Fossil> clearedFossils = new List<Fossil>();

        if (swappedFossil.powerUpType != PowerUpType.None) {
            if (swappedFossil.powerUpType == PowerUpType.DNA) {
                clearedFossils = ClearEntireBoard();
            } else {
                SpreadPowerUpType(swappedFossil);
            }
        } else {
            clearedFossils = ClearMatchingFossils(swappedFossil.type);
        }

        scoreManager.AddScoreForPowerUpActivation(PowerUpType.DNA, clearedFossils.Count);
        Debug.Log($"DNA activated: Cleared {clearedFossils.Count} fossils of type {swappedFossil.type}");
    }

    private List<Fossil> ClearEntireBoard() {
        List<Fossil> clearedFossils = new List<Fossil>();

        for (int x = 0; x < board.width; x++) {
            for (int y = 0; y < board.height; y++) {
                if (board.fossils[x, y] != null) {
                    clearedFossils.Add(board.fossils[x, y]);
                    ClearFossil(board.fossils[x, y]);
                }
            }
        }

        Debug.Log("Cleared entire board.");
        return clearedFossils;
    }

    private void SpreadPowerUpType(Fossil swappedFossil) {
        PowerUpType targetPowerUp = swappedFossil.powerUpType;

        for (int x = 0; x < board.width; x++) {
            for (int y = 0; y < board.height; y++) {
                Fossil currentFossil = board.fossils[x, y];
                if (currentFossil != null && currentFossil.type == swappedFossil.type) {
                    CreatePowerUp(currentFossil, targetPowerUp);
                    Debug.Log($"Changed fossil at ({x},{y}) to power-up type {targetPowerUp}");
                }
            }
        }

        Debug.Log($"Propagated power-up type {targetPowerUp} to all fossils of type {swappedFossil.type}.");
    }

    private List<Fossil> ClearMatchingFossils(FossilType targetType) {
        List<Fossil> clearedFossils = new List<Fossil>();

        for (int x = 0; x < board.width; x++) {
            for (int y = 0; y < board.height; y++) {
                if (board.fossils[x, y] != null && board.fossils[x, y].type == targetType) {
                    clearedFossils.Add(board.fossils[x, y]);
                    ClearFossil(board.fossils[x, y]);
                }
            }
        }

        Debug.Log($"Cleared fossils of type {targetType}.");
        return clearedFossils;
    }


    private void ClearFossil(Fossil targetFossil) {
        if (targetFossil != null) {
            board.DestroyFossil(targetFossil);
            Debug.Log($"Cleared fossil at ({targetFossil.x}, {targetFossil.y})");
        }
    }
}
