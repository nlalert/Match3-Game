using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapManager : MonoBehaviour {
    public BoardManager board;

    public void CheckAndSwap(Candy candy1, Candy candy2){
        if (AreAdjacent(candy1, candy2)){
            StartCoroutine(Swap(candy1, candy2));
        }
    }
    
    public IEnumerator Swap(Candy candy1, Candy candy2) {
        board.isAnimating = true;
        yield return StartCoroutine(board.animationManager.AnimateSwap(candy1, candy2));

        List<Candy> matches1 = board.matchManager.GetMatches(candy1);
        List<Candy> matches2 = board.matchManager.GetMatches(candy2);

        if ((matches1 != null && matches1.Count >= 3) || (matches2 != null && matches2.Count >= 3)) {
            Debug.Log("Match found!");
            if (!board.moveManager.UseMove()) {
                Debug.Log("No moves remaining!");
                board.GameOver();
            }
            
            if (matches1 != null) board.matchManager.DestroyMatches(matches1);
            if (matches2 != null) board.matchManager.DestroyMatches(matches2);

            yield return StartCoroutine(board.FillEmptySpots());

        }
        else {
            Debug.Log("No Match: Swapping back.");
            yield return StartCoroutine(board.animationManager.AnimateSwap(candy1, candy2));
        }

        board.isAnimating = false;
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
}
