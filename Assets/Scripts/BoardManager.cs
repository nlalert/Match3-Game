using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Fossil[,] fossils;
    public int width = 15;
    public int height = 20;
    public int hiddenRow = 5;
    public Fossil selectedFossil = null;
    public Fossil[] swapPair = new Fossil[2];

    public MoveManager moveManager;
    public AnimationManager animationManager;
    public FossilSpawner fossilSpawner;
    public SwapManager swapManager;
    public TimerManager timer;

    private void Start() {
        hiddenRow = height;
        InitializeBoard();
        timer.StartTimer();
    }

    private void InitializeBoard() {
        fossils = new Fossil[width, height + hiddenRow];
        FillBoardWithFossils();
    }

    private void FillBoardWithFossils() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height + hiddenRow; y++) {
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
            // Select the first fossil
            selectedFossil = clickedFossil;
            ScaleFossil(selectedFossil, 1.2f); // Scale up the selected fossil
        } else {
            // Deselect the previously selected fossil by resetting its scale
            ScaleFossil(selectedFossil, 1.0f);

            // Swap with the second fossil
            swapPair[0] = selectedFossil;
            swapPair[1] = clickedFossil;
            swapManager.CheckAndSwap(swapPair[0], swapPair[1]);

            // Reset selection
            selectedFossil = null;
        }
    }

    private void ScaleFossil(Fossil fossil, float scale) {
        if (fossil != null) {
            fossil.transform.localScale = new Vector3(scale, scale, scale);
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

    public void DestroyFossil(Fossil fossil){
        fossils[fossil.x, fossil.y] = null;
        Destroy(fossil.gameObject);
    }
}
