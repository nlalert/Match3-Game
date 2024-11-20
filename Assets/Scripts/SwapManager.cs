using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapManager : MonoBehaviour {
    public BoardManager board;
    public MatchManager matchManager;

    public void CheckAndSwap(Candy candy1, Candy candy2){
        if (AreAdjacent(candy1, candy2)){
            StartCoroutine(Swap(candy1, candy2));
        }
    }
    
    public IEnumerator Swap(Candy candy1, Candy candy2) {
        board.animationManager.isAnimating = true;
        yield return StartCoroutine(board.animationManager.AnimateSwap(candy1, candy2));

        List<Candy> matches1 = matchManager.GetMatches(candy1);
        List<Candy> matches2 = matchManager.GetMatches(candy2);

        if ((matches1 != null && matches1.Count >= 3) || (matches2 != null && matches2.Count >= 3)) {
            Debug.Log("Match found!");
            if (!board.moveManager.UseMove()) {
                Debug.Log("No moves remaining!");
                board.moveManager.GameOver();
            }
            
            if (matches1 != null) matchManager.DestroyMatches(matches1);
            if (matches2 != null) matchManager.DestroyMatches(matches2);

            yield return StartCoroutine(FillEmptySpots());

        }
        else {
            Debug.Log("No Match: Swapping back.");
            yield return StartCoroutine(board.animationManager.AnimateSwap(candy1, candy2));
        }

        board.animationManager.isAnimating = false;
    }

    public bool AreAdjacent(Candy candy1, Candy candy2) {
        int deltaX = Mathf.Abs(candy1.x - candy2.x);
        int deltaY = Mathf.Abs(candy1.y - candy2.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    public void CompleteSwap(Candy candy1, Candy candy2) {
        board.candies[candy1.x, candy1.y] = candy2;
        board.candies[candy2.x, candy2.y] = candy1;

        int tempX = candy1.x;
        int tempY = candy1.y;
        candy1.UpdatePosition(candy2.x, candy2.y);
        candy2.UpdatePosition(tempX, tempY);
    }

    
    public IEnumerator FillEmptySpots() {
        bool hasEmptySpots = true;

        while (hasEmptySpots) {
            hasEmptySpots = false;

            for (int x = 0; x < board.width; x++) {
                for (int y = 1; y < board.height; y++) { // Start from y=1 (skip the bottom row)
                    if (board.candies[x, y] != null && board.candies[x, y - 1] == null) {
                        // Move candy down
                        board.candies[x, y - 1] = board.candies[x, y];
                        board.candies[x, y] = null;
                        board.candies[x, y - 1].UpdatePosition(x, y - 1);

                        StartCoroutine(board.animationManager.AnimateCandyFall(board.candies[x, y - 1]));
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

        for (int x = 0; x < board.width; x++) {
            for (int y = 0; y < board.height; y++) {
                if (board.candies[x, y] != null) {
                    List<Candy> matches = matchManager.GetMatches(board.candies[x, y]);
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
        for (int x = 0; x < board.width; x++) {
            for (int y = board.height - 1; y >= 0; y--) {
                if (board.candies[x, y] == null) {
                    board.candySpawner.SpawnCandy(x, y);
                    yield return new WaitForSeconds(0.05f); // Slight delay for spawning effect
                }
            }
        }
    }
}
