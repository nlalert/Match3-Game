using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Candy[,] candies;
    public GameObject[] candyPrefabs;
    public int width = 15;
    public int height = 20;
    private float candySpacing = 1.2f;

    private Candy selectedCandy = null;

    private void Start() {
        InitializeBoard();
    }

    private void Update() {
        HandleInput(); // Check for user input each frame
    }

    private void InitializeBoard() {
        candies = new Candy[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                SpawnCandy(x, y);
            }
        }
    }

    private void SpawnCandy(int x, int y) {
        int randomType;

        do {
            randomType = Random.Range(0, candyPrefabs.Length);
        } while (WillMatchIfAdd(x, y, randomType));

        // Apply spacing to position
        Vector3 position = new Vector3(
            x * candySpacing - (width * candySpacing) / 2,
            y * candySpacing - (height * candySpacing) / 2,
            0
        );

        GameObject newCandy = Instantiate(candyPrefabs[randomType], position, Quaternion.identity);
        Candy candy = newCandy.GetComponent<Candy>();
        candy.Initialize(x, y, randomType);
        candies[x, y] = candy;
    }

    private bool WillMatchIfAdd(int x, int y, int type) {
        // Check horizontally
        if (x >= 2) {
            if (candies[x - 1, y] != null && candies[x - 2, y] != null) {
                if (candies[x - 1, y].type == type && candies[x - 2, y].type == type) {
                    return true;
                }
            }
        }

        // Check vertically
        if (y >= 2) {
            if (candies[x, y - 1] != null && candies[x, y - 2] != null) {
                if (candies[x, y - 1].type == type && candies[x, y - 2].type == type) {
                    return true;
                }
            }
        }

        return false;
    }

    private void HandleInput() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = GetBoardGridPosition(mousePos);

            if (IsInBoard(gridPos.x, gridPos.y)) {
                Candy clickedCandy = candies[gridPos.x, gridPos.y];

                if (selectedCandy == null) {
                    selectedCandy = clickedCandy;
                } else {
                    if (AreAdjacent(selectedCandy, clickedCandy)) {
                        SwapCandies(selectedCandy, clickedCandy);
                    }
                    selectedCandy = null;
                }
            }
        }
    }

    private Vector3Int GetBoardGridPosition(Vector3 worldPosition) {
        int x = Mathf.RoundToInt((worldPosition.x + (width * candySpacing) / 2) / candySpacing);
        int y = Mathf.RoundToInt((worldPosition.y + (height * candySpacing) / 2) / candySpacing);
        Debug.Log("X :"+x);
        Debug.Log("Y :"+y);

        return new Vector3Int(x, y, 0);
    }

    private bool IsInBoard(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private bool AreAdjacent(Candy candy1, Candy candy2) {
        int deltaX = Mathf.Abs(candy1.x - candy2.x);
        int deltaY = Mathf.Abs(candy1.y - candy2.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    private void SwapCandies(Candy candy1, Candy candy2) {
        candies[candy1.x, candy1.y] = candy2;
        candies[candy2.x, candy2.y] = candy1;

        int tempX = candy1.x;
        int tempY = candy1.y;
        candy1.UpdatePosition(candy2.x, candy2.y);
        candy2.UpdatePosition(tempX, tempY);

        Vector3 tempPosition = candy1.transform.position;
        candy1.transform.position = candy2.transform.position;
        candy2.transform.position = tempPosition;
    }
}
