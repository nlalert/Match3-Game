using UnityEngine;

public enum PowerUpType {
    None,
    LineClear,
    Bomb,
    DNA
}

public enum FossilType {
    Amber,
    Gem,
    Leaf,
    Shell,
    TRex,
    Trilobite,
    DNA
}

public class Fossil : MonoBehaviour {
    public int x, y;
    public FossilType type;
    public PowerUpType powerUpType = PowerUpType.None;

    public void Initialize(int x, int y, FossilType type) {
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
