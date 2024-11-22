using UnityEngine;
using TMPro;

public class MoveManager : MonoBehaviour {
    public int totalMoves = 20;
    public int remainingMoves;
    public TextMeshProUGUI movesText;

    // Initialize remaining moves when start the game
    private void Start(){
        ResetMoves();
    }

    // Check if there are moves left
    public bool HasMoveLeft(){
        return remainingMoves > 0;
    }

    // Reset the remaining moves to the total moves and update UI
    public void ResetMoves(){
        remainingMoves = totalMoves;
        UpdateMovesUI();
    }

    /// Decrease the remaining moves by one if have moves left and update UI
    public bool UseMove(){
        if (remainingMoves > 0){
            remainingMoves--;
            UpdateMovesUI();
        }
        return remainingMoves > 0;
    }

    // Update the moves left text in the UI to show remaining moves
    private void UpdateMovesUI(){
        movesText.text = "Moves Left: " + remainingMoves;
    }
}
