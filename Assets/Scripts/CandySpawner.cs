using UnityEngine;

public class CandySpawner : MonoBehaviour {
    public BoardManager board;
    public GameObject[] candyPrefabs;

    public void SpawnCandy(int x, int y) {
        int candyType = GetValidCandyType(x, y);
        GameObject newCandy = InstantiateCandy(x, y, candyType);
        RegisterCandyToBoard(newCandy, x, y, candyType);
    }

    private int GetValidCandyType(int x, int y) {
        int randomType;
        do {
            randomType = Random.Range(0, candyPrefabs.Length);
        } while (WillMatchIfAdd(x, y, randomType));

        return randomType;
    }

    private GameObject InstantiateCandy(int x, int y, int type) {
        Vector3 position = CalculateCandyWorldPosition(x, y);
        GameObject newCandy = Instantiate(candyPrefabs[type], position, Quaternion.identity);
        newCandy.transform.SetParent(board.transform);
        return newCandy;
    }

    private void RegisterCandyToBoard(GameObject newCandy, int x, int y, int type) {
        Candy candy = newCandy.GetComponent<Candy>();
        candy.Initialize(x, y, type);
        board.candies[x, y] = candy;
    }

    private bool WillMatchIfAdd(int x, int y, int type) {
        return MatchesHorizontally(x, y, type) || MatchesVertically(x, y, type);
    }

    private bool MatchesHorizontally(int x, int y, int type) {
        if (x < 2) return false;

        return board.candies[x - 1, y]?.type == type &&
               board.candies[x - 2, y]?.type == type;
    }

    private bool MatchesVertically(int x, int y, int type) {
        if (y < 2) return false;

        return board.candies[x, y - 1]?.type == type &&
               board.candies[x, y - 2]?.type == type;
    }

    private Vector3 CalculateCandyWorldPosition(int x, int y) {
        float offsetX = x - (board.width / 2f);
        float offsetY = y - (board.height / 2f);
        return new Vector3(offsetX, offsetY, 0f);
    }
}
