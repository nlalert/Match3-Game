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

    public IEnumerator Swap(Candy candy1, Candy candy2){
        board.animationManager.isAnimating = true;

        // Perform the initial swap animation
        yield return StartCoroutine(board.animationManager.AnimateSwap(candy1, candy2));

        // Check for matches after swapping
        if (TryHandleMatches(candy1, candy2)){
            if (!board.moveManager.UseMove()){
                Debug.Log("No moves remaining!");
                board.moveManager.GameOver();
            }

            yield return StartCoroutine(FillEmptySpots());
        }
        else{
            Debug.Log("No Match: Swapping back.");
            yield return StartCoroutine(board.animationManager.AnimateSwap(candy1, candy2));
        }

        board.animationManager.isAnimating = false;
    }

    public bool AreAdjacent(Candy candy1, Candy candy2){
        int deltaX = Mathf.Abs(candy1.x - candy2.x);
        int deltaY = Mathf.Abs(candy1.y - candy2.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    public void CompleteSwap(Candy candy1, Candy candy2){
        // Swap the candies in the board array
        board.candies[candy1.x, candy1.y] = candy2;
        board.candies[candy2.x, candy2.y] = candy1;

        // Update candy positions
        (candy1.x, candy1.y, candy2.x, candy2.y) = (candy2.x, candy2.y, candy1.x, candy1.y);
        candy1.UpdatePosition(candy1.x, candy1.y);
        candy2.UpdatePosition(candy2.x, candy2.y);
    }

    private bool TryHandleMatches(Candy candy1, Candy candy2){
        List<Candy> matches1 = matchManager.GetMatches(candy1);
        List<Candy> matches2 = matchManager.GetMatches(candy2);

        bool hasMatches = (matches1 != null && matches1.Count >= 3) || (matches2 != null && matches2.Count >= 3);

        if (hasMatches){
            Debug.Log("Match found!");
            if (matches1 != null) matchManager.DestroyMatches(matches1);
            if (matches2 != null) matchManager.DestroyMatches(matches2);
        }

        return hasMatches;
    }

    public IEnumerator FillEmptySpots(){
        while (TryShiftCandiesDown()){
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(SpawnNewCandies());
        yield return StartCoroutine(HandleChainReactions());
    }

    private bool TryShiftCandiesDown(){
        bool hasEmptySpots = false;

        for (int x = 0; x < board.width; x++){
            for (int y = 1; y < board.height; y++){
                if (board.candies[x, y] != null && board.candies[x, y - 1] == null){
                    board.candies[x, y - 1] = board.candies[x, y];
                    board.candies[x, y] = null;
                    board.candies[x, y - 1].UpdatePosition(x, y - 1);

                    StartCoroutine(board.animationManager.AnimateCandyFall(board.candies[x, y - 1]));
                    hasEmptySpots = true;
                }
            }
        }

        return hasEmptySpots;
    }

    private IEnumerator HandleChainReactions(){
        while (FindAndDestroyMatches()){
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(FillEmptySpots());
        }
    }

    private bool FindAndDestroyMatches(){
        bool foundNewMatches = false;

        for (int x = 0; x < board.width; x++){
            for (int y = 0; y < board.height; y++){
                if (board.candies[x, y] != null){
                    List<Candy> matches = matchManager.GetMatches(board.candies[x, y]);
                    if (matches != null && matches.Count >= 3){
                        matchManager.DestroyMatches(matches);
                        foundNewMatches = true;
                    }
                }
            }
        }

        return foundNewMatches;
    }

    private IEnumerator SpawnNewCandies(){
        for (int x = 0; x < board.width; x++){
            for (int y = board.height - 1; y >= 0; y--){
                if (board.candies[x, y] == null){
                    board.candySpawner.SpawnCandy(x, y);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
    }
}
