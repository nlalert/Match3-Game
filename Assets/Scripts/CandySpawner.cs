using UnityEngine;

public class CandySpawner : MonoBehaviour {
    public BoardManager board;
    public GameObject[] candyPrefabs;

    public void SpawnCandy(int x, int y) {
        int randomType = GetRandomCandyType(x, y);
        InstantiateCandyInScene(x, y, randomType);
    }

    private int GetRandomCandyType(int x, int y){
        int randomType;
        do {
            randomType = Random.Range(0, candyPrefabs.Length);
        } while (WillMatchIfAdd(x, y, randomType));

        return randomType;
    }

    private void InstantiateCandyInScene(int x, int y, int type){
        Vector3 position = new Vector3(
            x - (board.width / 2),
            y - (board.height / 2),
            0
        );

        GameObject newCandy = Instantiate(candyPrefabs[type], position, Quaternion.identity);
        newCandy.transform.SetParent(board.transform);

        AddCandyToBoard(newCandy, x, y, type);
    }

    private void AddCandyToBoard(GameObject newCandy, int x, int y, int type){
        Candy candy = newCandy.GetComponent<Candy>();
        
        candy.Initialize(x, y, type);
        board.candies[x, y] = candy;
    }

    private bool WillMatchIfAdd(int x, int y, int type) {
        return MatchesHorizontally(x, y, type) || MatchesVertically(x, y, type);
    }

    private bool MatchesHorizontally(int x, int y, int type) {
        if (x < 2) return false;

        if (board.candies[x - 1, y] != null && board.candies[x - 2, y] != null) {
            if (board.candies[x - 1, y].type == type && board.candies[x - 2, y].type == type) {
                return true;
            }
        }
        
        return false;
    }
    
    private bool MatchesVertically(int x, int y, int type) {
        if (y < 2) return false;

        if (board.candies[x, y - 1] != null && board.candies[x, y - 2] != null) {
            if (board.candies[x, y - 1].type == type && board.candies[x, y - 2].type == type) {
                return true;
            }
        }

        return false;
    }
}
