using UnityEngine;

public class FossilSpawner : MonoBehaviour {
    public BoardManager board;
    public GameObject[] fossilPrefabs;

    public void SpawnFossil(int x, int y) {
        int randomType = GetRandomFossilType(x, y);
        InstantiateFossilInScene(x, y, randomType);
    }

    private int GetRandomFossilType(int x, int y){
        int randomType;
        do {
            randomType = Random.Range(0, fossilPrefabs.Length);
        } while (WillMatchIfAdd(x, y, randomType));

        return randomType;
    }

    private void InstantiateFossilInScene(int x, int y, int type){
        Vector3 position = new Vector3(
            x - (board.width / 2),
            y - (board.height / 2),
            0
        );

        GameObject newFossil = Instantiate(fossilPrefabs[type], position, Quaternion.identity);
        newFossil.transform.SetParent(board.transform);

        AddFossilToBoard(newFossil, x, y, type);
    }

    private void AddFossilToBoard(GameObject newFossil, int x, int y, int type){
        Fossil fossil = newFossil.GetComponent<Fossil>();
        
        fossil.Initialize(x, y, type);
        board.fossils[x, y] = fossil;
    }

    private bool WillMatchIfAdd(int x, int y, int type) {
        return MatchesHorizontally(x, y, type) || MatchesVertically(x, y, type);
    }

    private bool MatchesHorizontally(int x, int y, int type) {
        if (x < 2) return false;

        if (board.fossils[x - 1, y] != null && board.fossils[x - 2, y] != null) {
            if (board.fossils[x - 1, y].type == type && board.fossils[x - 2, y].type == type) {
                return true;
            }
        }
        
        return false;
    }
    
    private bool MatchesVertically(int x, int y, int type) {
        if (y < 2) return false;

        if (board.fossils[x, y - 1] != null && board.fossils[x, y - 2] != null) {
            if (board.fossils[x, y - 1].type == type && board.fossils[x, y - 2].type == type) {
                return true;
            }
        }

        return false;
    }
}
