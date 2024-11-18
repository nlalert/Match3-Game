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

                        if (CheckMatches(selectedCandy) || CheckMatches(clickedCandy)) {
                            Debug.Log("Match");
                        } else {
                            // If no match, swap back
                            SwapCandies(selectedCandy, clickedCandy);
                            Debug.Log("No Match : Swap cancel.");
                        }
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

    private bool CheckMatches(Candy candy) {
        List<Candy> matchedCandies = new List<Candy>();

        // Check horizontal matches
        matchedCandies.Add(candy);
        for (int i = candy.x - 1; i >= 0; i--) {//left
            if (candies[i, candy.y] != null && candies[i, candy.y].type == candy.type) {
                matchedCandies.Add(candies[i, candy.y]);
            } else {
                break;
            }
        }
        for (int i = candy.x + 1; i < width; i++) {//right
            if (candies[i, candy.y] != null && candies[i, candy.y].type == candy.type) {
                matchedCandies.Add(candies[i, candy.y]);
            } else {
                break;
            }
        }
        if (matchedCandies.Count >= 3) {//match from left and right
            return true;
        }

        // Check vertical matches
        matchedCandies.Clear();
        matchedCandies.Add(candy);
        for (int i = candy.y - 1; i >= 0; i--) {//up
            if (candies[candy.x, i] != null && candies[candy.x, i].type == candy.type) {
                matchedCandies.Add(candies[candy.x, i]);
            } else {
                break;
            }
        }
        for (int i = candy.y + 1; i < height; i++) {//down
            if (candies[candy.x, i] != null && candies[candy.x, i].type == candy.type) {
                matchedCandies.Add(candies[candy.x, i]);
            } else {
                break;
            }
        }
        if (matchedCandies.Count >= 3) {//match from up and down
            return true;
        }

        return false;
    }
}
