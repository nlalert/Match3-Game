using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Candy[,] candies;
    public int width = 15;
    public int height = 20;

    private Candy selectedCandy = null;
    private bool isAnimating = false;

    public ScoreManager scoreManager;
    public MoveManager moveManager;
    public MatchManager matchManager;
    public AnimationManager animationManager;
    public CandySpawner candySpawner;

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

    public bool WillMatchIfAdd(int x, int y, int type) {
        // Check horizontally
        if (x >= 2) {
            if (candies[x - 1, y] != null && candies[x - 2, y] != null) {
                if (candies[x - 1, y].type == type && candies[x - 2, y].type == type) {
                    return true;
                }
            }
        }

        // Check vertically
        if (y >= 2) {
            if (candies[x, y - 1] != null && candies[x, y - 2] != null) {
                if (candies[x, y - 1].type == type && candies[x, y - 2].type == type) {
                    return true;
                }
            }
        }

        return false;
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
                if (AreAdjacent(selectedCandy, clickedCandy))
                {
                    StartCoroutine(HandleSwap(selectedCandy, clickedCandy));
                }
                selectedCandy = null;
            }
        }
    }

    private IEnumerator HandleSwap(Candy candy1, Candy candy2) {
        isAnimating = true;
        yield return StartCoroutine(animationManager.AnimateSwap(candy1, candy2));

        List<Candy> matches1 = matchManager.GetMatches(candy1);
        List<Candy> matches2 = matchManager.GetMatches(candy2);

        if ((matches1 != null && matches1.Count >= 3) || (matches2 != null && matches2.Count >= 3)) {
            Debug.Log("Match found!");
            if (!moveManager.UseMove()) {
                Debug.Log("No moves remaining!");
                GameOver();
            }
            
            if (matches1 != null) matchManager.DestroyMatches(matches1);
            if (matches2 != null) matchManager.DestroyMatches(matches2);

            yield return StartCoroutine(FillEmptySpots());

        }
        else {
            Debug.Log("No Match: Swapping back.");
            yield return StartCoroutine(animationManager.AnimateSwap(candy1, candy2));
        }

        isAnimating = false;
    }

    private void GameOver() {
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

    private bool AreAdjacent(Candy candy1, Candy candy2) {
        int deltaX = Mathf.Abs(candy1.x - candy2.x);
        int deltaY = Mathf.Abs(candy1.y - candy2.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    public void CompleteSwap(Candy candy1, Candy candy2) {
        candies[candy1.x, candy1.y] = candy2;
        candies[candy2.x, candy2.y] = candy1;

        int tempX = candy1.x;
        int tempY = candy1.y;
        candy1.UpdatePosition(candy2.x, candy2.y);
        candy2.UpdatePosition(tempX, tempY);
    }

    private IEnumerator FillEmptySpots() {
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
