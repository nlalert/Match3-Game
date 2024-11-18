using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Candy[,] candies;
    public GameObject[] candyPrefabs;
    public int width = 15;
    public int height = 20;
    private float candySpacing = 1.2f;

    private Candy selectedCandy = null;
    private bool isAnimating = false;

    private void Start() {
        InitializeBoard();
    }

    private void Update() {
        if (isAnimating) return;
        HandleInput();
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
                        StartCoroutine(HandleSwap(selectedCandy, clickedCandy));
                    }
                    selectedCandy = null;
                }
            }
        }
    }

    private IEnumerator HandleSwap(Candy candy1, Candy candy2) {
        isAnimating = true;
        yield return StartCoroutine(AnimateSwap(candy1, candy2));

        List<Candy> matches1 = GetMatches(candy1);
        List<Candy> matches2 = GetMatches(candy2);

        if ((matches1 != null && matches1.Count >= 3) || (matches2 != null && matches2.Count >= 3)) {
            Debug.Log("Match found!");
            if (matches1 != null) DestroyMatches(matches1);
            if (matches2 != null) DestroyMatches(matches2);

            yield return StartCoroutine(FillEmptySpots());
        } else {
            Debug.Log("No Match: Swapping back.");
            yield return StartCoroutine(AnimateSwap(candy1, candy2));
        }

        isAnimating = false;
    }

    private Vector3Int GetBoardGridPosition(Vector3 worldPosition) {
        int x = Mathf.RoundToInt((worldPosition.x + (width * candySpacing) / 2) / candySpacing);
        int y = Mathf.RoundToInt((worldPosition.y + (height * candySpacing) / 2) / candySpacing);
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

    private IEnumerator AnimateSwap(Candy candy1, Candy candy2, float duration = 0.2f) {
        Vector3 startPos1 = candy1.transform.position;
        Vector3 startPos2 = candy2.transform.position;

        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            candy1.transform.position = Vector3.Lerp(startPos1, startPos2, t);
            candy2.transform.position = Vector3.Lerp(startPos2, startPos1, t);
            yield return null;
        }

        candy1.transform.position = startPos2;
        candy2.transform.position = startPos1;

        CompleteSwap(candy1, candy2);
    }

    private void CompleteSwap(Candy candy1, Candy candy2) {
        candies[candy1.x, candy1.y] = candy2;
        candies[candy2.x, candy2.y] = candy1;

        int tempX = candy1.x;
        int tempY = candy1.y;
        candy1.UpdatePosition(candy2.x, candy2.y);
        candy2.UpdatePosition(tempX, tempY);
    }

    private List<Candy> GetMatches(Candy candy) {
        List<Candy> matchedCandies = new List<Candy>();

        // Check horizontal matches
        matchedCandies.Add(candy);
        for (int i = candy.x - 1; i >= 0; i--) {
            if (candies[i, candy.y] != null && candies[i, candy.y].type == candy.type) {
                matchedCandies.Add(candies[i, candy.y]);
            } else {
                break;
            }
        }
        for (int i = candy.x + 1; i < width; i++) {
            if (candies[i, candy.y] != null && candies[i, candy.y].type == candy.type) {
                matchedCandies.Add(candies[i, candy.y]);
            } else {
                break;
            }
        }
        if (matchedCandies.Count >= 3) return matchedCandies;

        // Check vertical matches
        matchedCandies.Clear();
        matchedCandies.Add(candy);
        for (int i = candy.y - 1; i >= 0; i--) {
            if (candies[candy.x, i] != null && candies[candy.x, i].type == candy.type) {
                matchedCandies.Add(candies[candy.x, i]);
            } else {
                break;
            }
        }
        for (int i = candy.y + 1; i < height; i++) {
            if (candies[candy.x, i] != null && candies[candy.x, i].type == candy.type) {
                matchedCandies.Add(candies[candy.x, i]);
            } else {
                break;
            }
        }
        if (matchedCandies.Count >= 3) return matchedCandies;

        return null;
    }



    private void DestroyMatches(List<Candy> matches) {
        foreach (Candy candy in matches) {
            candies[candy.x, candy.y] = null; // Remove from the board array
            Destroy(candy.gameObject);       // Destroy the game object
        }
    }

    private IEnumerator FillEmptySpots() {
        bool hasEmptySpots = true;

        while (hasEmptySpots) {
            hasEmptySpots = false;

            for (int x = 0; x < width; x++) {
                for (int y = 1; y < height; y++) { // Start from y=1 (skip the bottom row)
                    if (candies[x, y] != null && candies[x, y - 1] == null) {
                        // Move candy down
                        candies[x, y - 1] = candies[x, y];
                        candies[x, y] = null;
                        candies[x, y - 1].UpdatePosition(x, y - 1);

                        StartCoroutine(AnimateCandyFall(candies[x, y - 1]));
                        hasEmptySpots = true;
                    }
                }
            }

            // Wait for candies to fall before continuing
            yield return new WaitForSeconds(0.1f);
        }

        // Spawn new candies at the top
        yield return StartCoroutine(SpawnNewCandies());

        // Check for new matches and handle chain reactions
        yield return StartCoroutine(HandleChainReactions());
    }

    private IEnumerator HandleChainReactions() {
        bool foundNewMatches = false;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (candies[x, y] != null) {
                    List<Candy> matches = GetMatches(candies[x, y]);
                    if (matches != null && matches.Count >= 3) {
                        DestroyMatches(matches);
                        foundNewMatches = true;
                    }
                }
            }
        }

        if (foundNewMatches) {
            yield return new WaitForSeconds(0.2f); // Wait briefly before filling
            yield return StartCoroutine(FillEmptySpots());
        }
    }

    private IEnumerator AnimateCandyFall(Candy candy, float duration = 0.2f) {
        Vector3 targetPosition = new Vector3(
            candy.x * candySpacing - (width * candySpacing) / 2,
            candy.y * candySpacing - (height * candySpacing) / 2,
            0
        );

        Vector3 startPosition = candy.transform.position;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            candy.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        candy.transform.position = targetPosition;
    }

    private IEnumerator SpawnNewCandies() {
        for (int x = 0; x < width; x++) {
            for (int y = height - 1; y >= 0; y--) {
                if (candies[x, y] == null) {
                    SpawnCandy(x, y);
                    yield return new WaitForSeconds(0.05f); // Slight delay for spawning effect
                }
            }
        }
    }

}
