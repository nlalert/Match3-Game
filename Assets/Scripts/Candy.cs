using UnityEngine;

public enum PowerUpType {
    None,
    LineClear,
    Bomb,
    DNA
}

public class Candy : MonoBehaviour {
    public int x, y;
    public int type;
    public PowerUpType powerUpType = PowerUpType.None;

    public void Initialize(int x, int y, int type) {
        this.x = x;
        this.y = y;
        this.type = type;
        this.powerUpType = PowerUpType.None;
    }

    public void UpdatePosition(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetPowerUp(PowerUpType powerUpType) {
        this.powerUpType = powerUpType;
    }
}
