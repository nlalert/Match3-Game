using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Candy[,] candies;
    public int width = 15;
    public int height = 20;

    private Candy selectedCandy = null;
    public bool isAnimating = false;

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
        if (isAnimating) return;
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
        if (isAnimating) return;
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

    public void GameOver() {
        AudioManager.Instance.PlaySound(AudioManager.Instance.gameOverSound); // Play game over sound
        Debug.Log("Game Over! No moves remaining.");
    }

    private Vector3Int GetBoardGridPosition(Vector3 worldPosition) {
        int x = Mathf.RoundToInt(worldPosition.x + (width / 2));
        int y = Mathf.RoundToInt(worldPosition.y + (height / 2));
        return new Vector3Int(x, y, 0);
    }

    private bool IsInBoard(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public IEnumerator FillEmptySpots() {
        bool hasEmptySpots = true;

        while (hasEmptySpots) {
            hasEmptySpots = false;

            for (int x = 0; x < width; x++) {
                for (int y = 1; y < height; y++) { // Start from y=1 (skip the bottom row)
                    if (candies[x, y] != null && candies[x, y - 1] == null) {
                        // Move candy down
                        candies[x, y - 1] = candies[x, y];
                        candies[x, y] = null;
                        candies[x, y - 1].UpdatePosition(x, y - 1);

                        StartCoroutine(animationManager.AnimateCandyFall(candies[x, y - 1]));
                        hasEmptySpots = true;
                    }
                }
            }

            // Wait for candies to fall before continuing
            yield return new WaitForSeconds(0.1f);
        }

        // Spawn new candies at the top
        yield return StartCoroutine(SpawnNewCandies());

        // Check for new matches and handle chain reactions
        yield return StartCoroutine(HandleChainReactions());
    }

    private IEnumerator HandleChainReactions() {
        bool foundNewMatches = false;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (candies[x, y] != null) {
                    List<Candy> matches = matchManager.GetMatches(candies[x, y]);
                    if (matches != null && matches.Count >= 3) {
                        matchManager.DestroyMatches(matches);
                        foundNewMatches = true;
                    }
                }
            }
        }

        if (foundNewMatches) {
            yield return new WaitForSeconds(0.2f); // Wait briefly before filling
            yield return StartCoroutine(FillEmptySpots());
        }
    }

    private IEnumerator SpawnNewCandies() {
        for (int x = 0; x < width; x++) {
            for (int y = height - 1; y >= 0; y--) {
                if (candies[x, y] == null) {
                    candySpawner.SpawnCandy(x, y);
                    yield return new WaitForSeconds(0.05f); // Slight delay for spawning effect
                }
            }
        }
    }
}
