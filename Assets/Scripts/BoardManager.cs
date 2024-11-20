using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public Candy[,] candies;
    public int width = 15;
    public int height = 20;

    private Candy selectedCandy = null;

    public ScoreManager scoreManager;
    public MoveManager moveManager;
    public MatchManager matchManager;
    public AnimationManager animationManager;
    public CandySpawner candySpawner;
    public SwapManager swapManager;

    private void Start() {
        InitializeBoard();
    }

    private void Update() {
        if (animationManager.isAnimating) return;
    }

    private void InitializeBoard() {
        candies = new Candy[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                candySpawner.SpawnCandy(x, y);
            }
        }
    }

    public void HandleCandyClick(Vector3 mousePos){
        if (animationManager.isAnimating) return;
        if (!moveManager.HasMoveLeft()) return;

        Vector3Int gridPos = GetBoardGridPosition(mousePos);

        if (IsInBoard(gridPos.x, gridPos.y)) {
            Candy clickedCandy = candies[gridPos.x, gridPos.y];

            if (selectedCandy == null)
            {
                selectedCandy = clickedCandy;
            }
            else
            {
                swapManager.CheckAndSwap(selectedCandy, clickedCandy);
                selectedCandy = null;
            }
        }
    }

    private Vector3Int GetBoardGridPosition(Vector3 worldPosition) {
        int x = Mathf.RoundToInt(worldPosition.x + (width / 2));
        int y = Mathf.RoundToInt(worldPosition.y + (height / 2));
        return new Vector3Int(x, y, 0);
    }

    private bool IsInBoard(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
