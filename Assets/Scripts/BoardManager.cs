using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Candy[,] candies;
    public int width = 15;
    public int height = 20;

    private Candy selectedCandy = null;

    public MoveManager moveManager;
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
        FillBoardWithCandies();
    }

    private void FillBoardWithCandies() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                candySpawner.SpawnCandy(x, y);
            }
        }
    }

    public void HandleCandyClick(Vector3 mousePos){
        if (animationManager.isAnimating || !moveManager.HasMoveLeft()) return;

        Vector2Int gridPos = GetBoardGridPosition(mousePos);

        if (!IsInBoard(gridPos.x, gridPos.y)) return;
        
        Candy clickedCandy = candies[gridPos.x, gridPos.y];
        ProcessCandySelection(clickedCandy);
    }

    private void ProcessCandySelection(Candy clickedCandy) {
        if (selectedCandy == null) {
            selectedCandy = clickedCandy;
        } else {
            swapManager.CheckAndSwap(selectedCandy, clickedCandy);
            selectedCandy = null;
        }
    }
    
    private Vector2Int GetBoardGridPosition(Vector3 worldPosition) {
        int x = Mathf.RoundToInt(worldPosition.x + (width / 2));
        int y = Mathf.RoundToInt(worldPosition.y + (height / 2));
        return new Vector2Int(x, y);
    }

    private bool IsInBoard(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
