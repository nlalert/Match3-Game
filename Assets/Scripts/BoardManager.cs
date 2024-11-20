using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Fossil[,] fossils;
    public int width = 15;
    public int height = 20;

    public Fossil selectedFossil = null;
    public Fossil[] swapPair = new Fossil[2];

    public MoveManager moveManager;
    public AnimationManager animationManager;
    public FossilSpawner fossilSpawner;
    public SwapManager swapManager;

    private void Start() {
        InitializeBoard();
    }

    private void Update() {
        if (animationManager.isAnimating) return;
    }

    private void InitializeBoard() {
        fossils = new Fossil[width, height];
        FillBoardWithFossils();
    }

    private void FillBoardWithFossils() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                fossilSpawner.SpawnFossil(x, y);
            }
        }
    }

    public void HandleFossilClick(Vector3 mousePos) {
        if (animationManager.isAnimating || !moveManager.HasMoveLeft()) return;

        Vector2Int gridPos = GetBoardGridPosition(mousePos);

        if (!IsInBoard(gridPos.x, gridPos.y)) return;

        Fossil clickedFossil = fossils[gridPos.x, gridPos.y];
        ProcessFossilSelection(clickedFossil);
    }

    private void ProcessFossilSelection(Fossil clickedFossil) {
        if (selectedFossil == null) {
            selectedFossil = clickedFossil;  // Select the first fossil
        } else {
            swapPair[0] = selectedFossil;
            swapPair[1] = clickedFossil;
            swapManager.CheckAndSwap(swapPair[0], swapPair[1]);
            selectedFossil = null;  // Reset selection after swap
        }
    }

    private Vector2Int GetBoardGridPosition(Vector3 worldPosition) {
        int x = Mathf.RoundToInt(worldPosition.x + (width / 2));
        int y = Mathf.RoundToInt(worldPosition.y + (height / 2));
        return new Vector2Int(x, y);
    }

    public bool IsInBoard(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
