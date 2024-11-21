using UnityEngine;

public class InputManager : MonoBehaviour
{
    public BoardManager board;
    public UIManager uiManager;

    private void Update(){
        if (Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            board.HandleFossilClick(mousePos);
        }
        if (Input.GetKeyDown(KeyCode.Escape)){
            uiManager.TogglePause();
        }
    }
}
