using System.Collections.Generic;
using UnityEngine;

public class BombSpriteManager : MonoBehaviour {
    [SerializeField]
    public List<Sprite> bombSprites;

    public Sprite GetSprite(FossilType type) {
        return bombSprites[(int)type];
    }
}
