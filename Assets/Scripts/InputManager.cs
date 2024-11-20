using UnityEngine;

public class InputManager : MonoBehaviour
{
    public BoardManager board;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            board.HandleFossilClick(mousePos);
        }
    }
}
