using UnityEngine;

public class CandySpawner : MonoBehaviour {
    public BoardManager board;
    public GameObject[] candyPrefabs;

    public void SpawnCandy(int x, int y) {
        int randomType;

        do {
            randomType = Random.Range(0, candyPrefabs.Length);
        } while (WillMatchIfAdd(x, y, randomType));

        Vector3 position = new Vector3(
            x - (board.width / 2),
            y - (board.height / 2),
            0
        );

        GameObject newCandy = Instantiate(candyPrefabs[randomType], position, Quaternion.identity);
        newCandy.transform.SetParent(board.transform);

        Candy candy = newCandy.GetComponent<Candy>();
        candy.Initialize(x, y, randomType);
        board.candies[x, y] = candy;
    }

    private bool WillMatchIfAdd(int x, int y, int type) {
        // Check horizontally
        if (x >= 2) {
            if (board.candies[x - 1, y] != null && board.candies[x - 2, y] != null) {
                if (board.candies[x - 1, y].type == type && board.candies[x - 2, y].type == type) {
                    return true;
                }
            }
        }

        // Check vertically
        if (y >= 2) {
            if (board.candies[x, y - 1] != null && board.candies[x, y - 2] != null) {
                if (board.candies[x, y - 1].type == type && board.candies[x, y - 2].type == type) {
                    return true;
                }
            }
        }

        return false;
    }
}
