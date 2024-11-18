using UnityEngine;

public class Candy : MonoBehaviour {
    public int x;
    public int y;
    public int type;

    public void Initialize(int x, int y, int type) {
        this.x = x;
        this.y = y;
        this.type = type;
    }

    public void UpdatePosition(int x, int y) {
        this.x = x;
        this.y = y;
    }
}
