using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapManager : MonoBehaviour {

    // https://docs.unity3d.com/ScriptReference/Coroutine.html

    public BoardManager board;
    public MatchManager matchManager;
    public MoveManager moveManager;
    public ScoreManager scoreManager;

    // Check if two fossils are adjacent and swap
    public void CheckAndSwap(Fossil fossil1, Fossil fossil2){
        if (AreAdjacent(fossil1, fossil2)){
            StartCoroutine(Swap(fossil1, fossil2));
        }
    }

    // Swap two fossils and play animation with coroutine and check for moves after swap
    public IEnumerator Swap(Fossil fossil1, Fossil fossil2){
        board.animationManager.isAnimating = true;

        yield return StartCoroutine(board.animationManager.AnimateSwap(fossil1, fossil2));

        // Check for matches after swapping
        if (TryHandleMatches(fossil1, fossil2)){
            moveManager.UseMove();
            yield return StartCoroutine(FillEmptySpots());
        }
        else{
            yield return StartCoroutine(board.animationManager.AnimateSwap(fossil1, fossil2));
        }

        board.animationManager.isAnimating = false;

        if (!moveManager.HasMoveLeft()){
            scoreManager.GameOver();
        }
    }

    // Check if two fossils are adjacent
    public bool AreAdjacent(Fossil fossil1, Fossil fossil2){
        int deltaX = Mathf.Abs(fossil1.x - fossil2.x);
        int deltaY = Mathf.Abs(fossil1.y - fossil2.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    // Swap two fossils in board array and in Fossil class
    public void CompleteSwap(Fossil fossil1, Fossil fossil2){
        // Swap the fossils in the board array
        board.fossils[fossil1.x, fossil1.y] = fossil2;
        board.fossils[fossil2.x, fossil2.y] = fossil1;

        // Update fossil position
        (fossil1.x, fossil1.y, fossil2.x, fossil2.y) = (fossil2.x, fossil2.y, fossil1.x, fossil1.y);
        fossil1.UpdatePosition(fossil1.x, fossil1.y);
        fossil2.UpdatePosition(fossil2.x, fossil2.y);
    }

    // Check for match of two fossil after swap
    private bool TryHandleMatches(Fossil fossil1, Fossil fossil2){
        if (HandleSpecialFossils(fossil1, fossil2)){
            return true;
        }

        List<Fossil> matches1 = matchManager.GetMatches(fossil1);
        List<Fossil> matches2 = matchManager.GetMatches(fossil2);

        bool hasMatches = matches1?.Count >= 3 || matches2?.Count >= 3;

        if (hasMatches){
            if (matches1 != null) matchManager.DestroyMatches(matches1);
            if (matches2 != null) matchManager.DestroyMatches(matches2);
        }

        return hasMatches;
    }

    // Handle swapping of 2 power-up fossils
    private bool HandleSpecialFossils(Fossil fossil1, Fossil fossil2){
        if (fossil1.type == FossilType.DNA){
            matchManager.powerUpManager.ActivateDNA(fossil2);
            board.DestroyFossil(fossil1);
            return true;
        }

        if (fossil2.type == FossilType.DNA){
            matchManager.powerUpManager.ActivateDNA(fossil1);
            board.DestroyFossil(fossil2);
            return true;
        }

        if (fossil1.powerUpType == PowerUpType.LineClear && fossil2.powerUpType == PowerUpType.LineClear){
            matchManager.powerUpManager.ActivateSuperLineClear(fossil1);
            return true;
        }

        if (fossil1.powerUpType == PowerUpType.Bomb && fossil2.powerUpType == PowerUpType.Bomb){
            matchManager.powerUpManager.ActivateSuperBomb(fossil1);
            return true;
        }

        if ((fossil1.powerUpType == PowerUpType.Bomb && fossil2.powerUpType == PowerUpType.LineClear) ||
            (fossil2.powerUpType == PowerUpType.Bomb && fossil1.powerUpType == PowerUpType.LineClear))
        {
            matchManager.powerUpManager.ActivateExplosiveLineClear(fossil1);
            return true;
        }

        return false;
    }

    // fill empty spot on board after match
    public IEnumerator FillEmptySpots(){
        while (TryShiftfossilsDown()){
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(SpawnNewfossils());
        yield return StartCoroutine(HandleChainReactions());
    }

    // Make fossil falling down in fill empty spot after match
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

    // Check for chain reaction (fossil matches automatically after new fossil falling down)
    private IEnumerator HandleChainReactions(){
        while (FindAndDestroyMatches()){
            yield return StartCoroutine(FillEmptySpots());
        }
    }

    // Find matches on board and destroy matches fossil
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

    // spawn new fossils to falling on top of board
    private IEnumerator SpawnNewfossils(){
        for (int x = 0; x < board.width; x++){
            for (int y = board.height + board.hiddenRow - 1; y >= 0; y--){
                if (board.fossils[x, y] == null){
                    board.fossilSpawner.SpawnFossil(x, y);
                }
            }
        }
        yield return null;
    }
}
