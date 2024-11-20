using UnityEngine;

public class CandySpawner : MonoBehaviour {
    public BoardManager board;
    public GameObject[] candyPrefabs;

    public void SpawnCandy(int x, int y) {
        int randomType;

        do {
            randomType = Random.Range(0, candyPrefabs.Length);
        } while (board.WillMatchIfAdd(x, y, randomType));

        Vector3 position = new Vector3(
            x - (board.width / 2),
            y - (board.height / 2),
            0
        );

        GameObject newCandy = Instantiate(candyPrefabs[randomType], position, Quaternion.identity);
        Candy candy = newCandy.GetComponent<Candy>();
        candy.Initialize(x, y, randomType);
        board.candies[x, y] = candy;
    }

}
