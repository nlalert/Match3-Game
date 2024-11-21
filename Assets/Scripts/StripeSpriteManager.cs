using System.Collections.Generic;
using UnityEngine;

public class StripeSpriteManager : MonoBehaviour {
    [SerializeField]
    public List<Sprite> stripeSprites;

    public Sprite GetSprite(FossilType type) {
        return stripeSprites[(int)type];
    }
}
