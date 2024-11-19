using UnityEngine;
using TMPro;

public class MoveManager : MonoBehaviour
{
    public int totalMoves = 20;
    public int remainingMoves;
    public TextMeshProUGUI movesText;

    private void Start(){
        ResetMoves();
    }

    public bool HasMoveLeft(){
        return remainingMoves > 0;
    }

    public void ResetMoves(){
        remainingMoves = totalMoves;
        UpdateMovesUI();
    }

    public bool UseMove(){
        if (remainingMoves > 0){
            remainingMoves--;
            UpdateMovesUI();
        }
        return !(remainingMoves <= 0);
    }

    private void UpdateMovesUI(){
        movesText.text = "Moves: " + remainingMoves;
    }
}
