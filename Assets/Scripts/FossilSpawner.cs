using UnityEngine;

public class FossilSpawner : MonoBehaviour
{
    public BoardManager board;
    public GameObject[] fossilPrefabs; // fossil prefabs to spawn

    // Spawn a fossil at grid position with a random type so it won't match when start
    public void SpawnFossil(int x, int y){
        FossilType randomType = GetRandomFossilType(x, y);
        InstantiateFossilInScene(x, y, randomType);
    }

    // Select a random fossil type that won't match with other fossils when spawn
    private FossilType GetRandomFossilType(int x, int y){
        FossilType randomType;

        do{
            randomType = (FossilType)Random.Range(0, fossilPrefabs.Length);
        } while (WillMatchIfAdd(x, y, randomType));

        return randomType;
    }

    // Instantiate a fossil in the game scene at grid position and set fossil type
    private void InstantiateFossilInScene(int x, int y, FossilType type){
        Vector3 position = new Vector3(
            x - (board.width / 2),
            y - (board.height / 2),
            0
        );

        GameObject newFossil = Instantiate(fossilPrefabs[(int)type], position, Quaternion.identity);
        newFossil.transform.SetParent(board.transform);

        AddFossilToBoard(newFossil, x, y, type);
    }

    // Add the fossil GameObject to the board fossils array and initialize;
    private void AddFossilToBoard(GameObject newFossil, int x, int y, FossilType type){
        Fossil fossil = newFossil.GetComponent<Fossil>();

        fossil.Initialize(x, y, type);
        board.fossils[x, y] = fossil;
    }

    // Check if adding a fossil at the position will create a match
    private bool WillMatchIfAdd(int x, int y, FossilType type){
        return MatchesHorizontally(x, y, type) || MatchesVertically(x, y, type);
    }

    // Check for a horizontal match of three fossils
    private bool MatchesHorizontally(int x, int y, FossilType type){
        if (x < 2) return false;

        if (board.fossils[x - 1, y] != null && board.fossils[x - 2, y] != null){
            if (board.fossils[x - 1, y].type == type && board.fossils[x - 2, y].type == type){
                return true;
            }
        }

        return false;
    }

    // Checks for a vertical match of three fossils
    private bool MatchesVertically(int x, int y, FossilType type){
        if (y < 2) return false;

        if (board.fossils[x, y - 1] != null && board.fossils[x, y - 2] != null){
            if (board.fossils[x, y - 1].type == type && board.fossils[x, y - 2].type == type){
                return true;
            }
        }

        return false;
    }
}
