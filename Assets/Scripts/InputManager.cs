using UnityEngine;

public class InputManager : MonoBehaviour{
    public BoardManager board;
    public UIManager uiManager;

    private void Update(){
        // Toggle pause if the ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape)){
            uiManager.TogglePause();
        }

        // Process fossil click when left mouse button is clicked
        if (!uiManager.isPaused && Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            board.HandleFossilClick(mousePos);
        }
    }
}
