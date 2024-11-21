using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapManager : MonoBehaviour {
    public BoardManager board;
    public MatchManager matchManager;
    public MoveManager moveManager;

    public void CheckAndSwap(Fossil fossil1, Fossil fossil2){
        if (AreAdjacent(fossil1, fossil2)){
            StartCoroutine(Swap(fossil1, fossil2));
        }
    }

    public IEnumerator Swap(Fossil fossil1, Fossil fossil2){
        board.animationManager.isAnimating = true;

        // Perform the initial swap animation
        yield return StartCoroutine(board.animationManager.AnimateSwap(fossil1, fossil2));

        // Check for matches after swapping
        if (TryHandleMatches(fossil1, fossil2)){
            if (!moveManager.UseMove()){
                Debug.Log("No moves remaining!");
                moveManager.GameOver();
            }

            yield return StartCoroutine(FillEmptySpots());
        }
        else{
            Debug.Log("No Match: Swapping back.");
            yield return StartCoroutine(board.animationManager.AnimateSwap(fossil1, fossil2));
        }

        board.animationManager.isAnimating = false;
    }

    public bool AreAdjacent(Fossil fossil1, Fossil fossil2){
        int deltaX = Mathf.Abs(fossil1.x - fossil2.x);
        int deltaY = Mathf.Abs(fossil1.y - fossil2.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    public void CompleteSwap(Fossil fossil1, Fossil fossil2){
        // Swap the fossils in the board array
        board.fossils[fossil1.x, fossil1.y] = fossil2;
        board.fossils[fossil2.x, fossil2.y] = fossil1;

        // Update fossil positions
        (fossil1.x, fossil1.y, fossil2.x, fossil2.y) = (fossil2.x, fossil2.y, fossil1.x, fossil1.y);
        fossil1.UpdatePosition(fossil1.x, fossil1.y);
        fossil2.UpdatePosition(fossil2.x, fossil2.y);
    }

    private bool TryHandleMatches(Fossil fossil1, Fossil fossil2){
        List<Fossil> matches1 = matchManager.GetMatches(fossil1);
        List<Fossil> matches2 = matchManager.GetMatches(fossil2);

        bool hasMatches = (matches1 != null && matches1.Count >= 3) || (matches2 != null && matches2.Count >= 3);

        if (hasMatches){
            Debug.Log("Match found!");
            if (matches1 != null) matchManager.DestroyMatches(matches1);
            if (matches2 != null) matchManager.DestroyMatches(matches2);
        }

        return hasMatches;
    }

    public IEnumerator FillEmptySpots(){
        while (TryShiftfossilsDown()){
            yield return new WaitForSeconds(0.15f);
        }

        yield return StartCoroutine(SpawnNewfossils());
        yield return StartCoroutine(HandleChainReactions());
    }

    private bool TryShiftfossilsDown(){
        bool hasEmptySpots = false;

        for (int x = 0; x < board.width; x++){
            for (int y = 1; y < board.height + board.hiddenRow; y++){
                if (board.fossils[x, y] != null && board.fossils[x, y - 1] == null){
                    board.fossils[x, y - 1] = board.fossils[x, y];
                    board.fossils[x, y] = null;
                    board.fossils[x, y - 1].UpdatePosition(x, y - 1);

                    StartCoroutine(board.animationManager.AnimateFossilFall(board.fossils[x, y - 1]));
                    hasEmptySpots = true;
                }
            }
        }

        return hasEmptySpots;
    }

    private IEnumerator HandleChainReactions(){
        while (FindAndDestroyMatches()){
            yield return StartCoroutine(FillEmptySpots());
        }
    }

    private bool FindAndDestroyMatches(){
        bool foundNewMatches = false;

        for (int x = 0; x < board.width; x++){
            for (int y = 0; y < board.height; y++){
                if (board.fossils[x, y] != null){
                    List<Fossil> matches = matchManager.GetMatches(board.fossils[x, y]);
                    if (matches != null && matches.Count >= 3){
                        matchManager.DestroyMatches(matches);
                        foundNewMatches = true;
                    }
                }
            }
        }

        return foundNewMatches;
    }

    private IEnumerator SpawnNewfossils(){
        for (int x = 0; x < board.width; x++){
            for (int y = board.height + board.hiddenRow - 1; y >= 0; y--){
                if (board.fossils[x, y] == null){
                    board.fossilSpawner.SpawnFossil(x, y);
                }
            }
        }
        yield return null; // Ensure the coroutine completes properly
    }
}
