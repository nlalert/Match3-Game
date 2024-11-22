using System;
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

    private int bombRadius = 2;

    // Handle creation of a power-up based on how fossil match
    public Fossil HandlePowerUpCreation(List<Fossil> matches) {
        if (matches == null || matches.Count < 4) return null;

        Fossil centralFossil = GetCentralFossil(matches);

        if (IsDNA(matches)) {
            CreatePowerUp(centralFossil, PowerUpType.DNA);
            AudioManager.Instance.PlaySound(AudioManager.Instance.colorBombCreated);
            scoreManager.CalculateScore(matches, PowerUpType.DNA);

            return centralFossil;
        }
        else if (matches.Count == 4) {
            CreatePowerUp(centralFossil, PowerUpType.LineClear);
            AudioManager.Instance.PlaySound(AudioManager.Instance.stripeCreated);
            scoreManager.CalculateScore(matches, PowerUpType.LineClear);

            return centralFossil;
        } 
        else if (matches.Count >= 5) {
            CreatePowerUp(centralFossil, PowerUpType.Bomb);
            AudioManager.Instance.PlaySound(AudioManager.Instance.wrapCreated);
            scoreManager.CalculateScore(matches, PowerUpType.Bomb);
            
            return centralFossil;
        }

        return null;
    }

    // Choose central fossil for power-up creation
    private Fossil GetCentralFossil(List<Fossil> matches) {
        Fossil centralFossil = null;

        if (board.swapPair[0] != null && matches.Contains(board.swapPair[0])) {
            centralFossil = board.swapPair[0];
            board.swapPair[0] = null;
        }
        else if (board.swapPair[1] != null && matches.Contains(board.swapPair[1])) {
            centralFossil = board.swapPair[1];
            board.swapPair[1] = null;
        }
        else {
            centralFossil = matches[matches.Count / 2];
        }

        return centralFossil;
    }

    // Check if the matches will create as DNA power-up
    private bool IsDNA(List<Fossil> matches) {
        if(matches.Count != 5) return false;

        int startX = matches[0].x;
        int startY = matches[0].y;

        bool sameRow = matches.TrueForAll(c => c.y == startY);

        bool sameColumn = matches.TrueForAll(c => c.x == startX);

        return sameRow || sameColumn;
    }

    // Creates a power-up
    private void CreatePowerUp(Fossil fossil, PowerUpType powerUpType) {
        fossil.SetPowerUp(powerUpType);

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
        }
    }

    private Sprite GetStripeSprite(FossilType type) {
        return stripeSpriteManager.GetSprite(type);
    }

    private Sprite GetBombSprite(FossilType type) {
        return bombSpriteManager.GetSprite(type);
    }

    // Activate power-up and to use its effect
    public void ActivatePowerUp(Fossil fossil) {
        if (fossil == null || fossil.powerUpType == PowerUpType.None) return;
        switch (fossil.powerUpType) {
            case PowerUpType.LineClear:
                fossil.powerUpType = PowerUpType.None;
                ClearLine(fossil);
                break;
            case PowerUpType.Bomb:
                fossil.powerUpType = PowerUpType.None;
                ExplodeAround(fossil);
                break;
            case PowerUpType.DNA:
                fossil.powerUpType = PowerUpType.None;
                ActivateDNA(fossil);
                break;
        }
    }

    // Clear Cross line
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

        AudioManager.Instance.PlaySound(AudioManager.Instance.stripeBlast);
        scoreManager.AddScoreForPowerUpActivation(PowerUpType.LineClear, clearedFossils.Count);
    }

    // Explode with radius of bombRadius
    private void ExplodeAround(Fossil fossil) {
        int x = fossil.x;
        int y = fossil.y;

        List<Fossil> clearedFossils = new List<Fossil>();

        for (int i = x - bombRadius; i <= x + bombRadius; i++) {
            for (int j = y - bombRadius; j <= y + bombRadius; j++) {
                if (board.IsInBoard(i, j)) {
                    clearedFossils.Add(board.fossils[i, j]);
                    ClearFossil(board.fossils[i, j]);
                }
            }
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.wrapBlast);
        scoreManager.AddScoreForPowerUpActivation(PowerUpType.Bomb, clearedFossils.Count);
    }

    // Activate effect of DNA
    public void ActivateDNA(Fossil swappedFossil) {
        List<Fossil> clearedFossils = new List<Fossil>();

        if (swappedFossil.powerUpType != PowerUpType.None) {
            if (swappedFossil.powerUpType == PowerUpType.DNA) {
                clearedFossils = ClearEntireBoard();
            } 
            else {
                SpreadPowerUpType(swappedFossil);
            }
        } 
        else {
            clearedFossils = ClearMatchingFossils(swappedFossil.type);
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.colorBombBlast);
        scoreManager.AddScoreForPowerUpActivation(PowerUpType.DNA, clearedFossils.Count);
    }

    // Activate combination effect of stripe and stripe
    public void ActivateSuperLineClear(Fossil fossil){
        int x = fossil.x;
        int y = fossil.y;
        
        List<Fossil> clearedFossils = new List<Fossil>();

        // Clear entire row and column
        for (int i = 0; i < board.width; i++) {
            if (board.fossils[i, y] != null) {
                clearedFossils.Add(board.fossils[i, y]);
                ClearFossil(board.fossils[i, y]);
            }
        }
        for (int j = 0; j < board.height; j++) {
            if (board.fossils[x, j] != null) {
                clearedFossils.Add(board.fossils[x, j]);
                ClearFossil(board.fossils[x, j]);
            }
        }

        // Clear diagonals
        for (int i = -Mathf.Max(board.width, board.height); i <= Mathf.Max(board.width, board.height); i++) {
            if (board.IsInBoard(x + i, y + i) && board.fossils[x + i, y + i] != null) {
                clearedFossils.Add(board.fossils[x + i, y + i]);
                ClearFossil(board.fossils[x + i, y + i]);
            }
            if (board.IsInBoard(x + i, y - i) && board.fossils[x + i, y - i] != null) {
                clearedFossils.Add(board.fossils[x + i, y - i]);
                ClearFossil(board.fossils[x + i, y - i]);
            }
        }

        scoreManager.AddScoreForPowerUpActivation(PowerUpType.LineClear, clearedFossils.Count);
    }

    // Activate combination effect of bomb and bomb
    public void ActivateSuperBomb(Fossil centralFossil) {
        int x = centralFossil.x;
        int y = centralFossil.y;
        int radius = bombRadius * 2;

        List<Fossil> clearedFossils = new List<Fossil>();

        for (int i = x - radius; i <= x + radius; i++) {
            for (int j = y - radius; j <= y + radius; j++) {
                if (board.IsInBoard(i, j) && board.fossils[i, j] != null) {
                    clearedFossils.Add(board.fossils[i, j]);
                    ClearFossil(board.fossils[i, j]);
                }
            }
        }

        scoreManager.AddScoreForPowerUpActivation(PowerUpType.Bomb, clearedFossils.Count);
    }

    // Activate combination effect of bomb and stripe
    public void ActivateExplosiveLineClear(Fossil fossil) {
        int x = fossil.x;
        int y = fossil.y;

        List<Fossil> clearedFossils = new List<Fossil>();
        
        // Clear the row within the bomb radius
        for (int i = 0; i < board.width; i++) {
            for (int j = y - bombRadius; j <= y + bombRadius; j++) {
                if (board.IsInBoard(i, j) && board.fossils[i, j] != null) {
                    if(!clearedFossils.Contains(board.fossils[i, j])){
                        clearedFossils.Add(board.fossils[i, j]);
                    }
                }
            }
        }

        // Clear the column within the bomb radius
        for (int i = x - bombRadius; i <= x + bombRadius; i++) {
            for (int j = 0; j < board.height; j++) {
                if (board.IsInBoard(i, j) && board.fossils[i, j] != null) {
                    if(!clearedFossils.Contains(board.fossils[i, j])){
                        clearedFossils.Add(board.fossils[i, j]);
                    }
                }
            }
        }

        foreach (Fossil fossilElement in clearedFossils){
            ClearFossil(fossilElement);
        }

        scoreManager.AddScoreForPowerUpActivation(PowerUpType.LineClear, clearedFossils.Count);
    }

    // Clear entire board from effect of DNA
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

        return clearedFossils;
    }

    // Spread power-up of fossil that swap with DNA to other fossils with same type
    private void SpreadPowerUpType(Fossil swappedFossil) {
        PowerUpType targetPowerUp = swappedFossil.powerUpType;

        for (int x = 0; x < board.width; x++) {
            for (int y = 0; y < board.height; y++) {
                Fossil currentFossil = board.fossils[x, y];
                if (currentFossil != null && currentFossil.type == swappedFossil.type) {
                    CreatePowerUp(currentFossil, targetPowerUp);
                }
            }
        }
    }

    // Clear all fossil of same type of fossil that swap with DNA
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

        return clearedFossils;
    }

    // Clear fossils that will get destroy by power-up effect and also use its power-up
    private void ClearFossil(Fossil targetFossil) {
        if (targetFossil != null) {
            if (targetFossil.powerUpType != PowerUpType.None) {
                if(targetFossil.powerUpType != PowerUpType.DNA){
                    ActivatePowerUp(targetFossil);
                }
                else{
                    targetFossil.type = (FossilType) UnityEngine.Random.Range(0, 6);
                    ActivatePowerUp(targetFossil);
                }
            }

            board.DestroyFossil(targetFossil);
        }
    }
}
