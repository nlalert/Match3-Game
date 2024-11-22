using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {
    public BoardManager board;
    public bool isAnimating = false;
    public float swapDuration = 0.15f;
    public float fallDuration = 0.15f;
    public IEnumerator AnimateSwap(Fossil fossil1, Fossil fossil2) {
        AudioManager.Instance.PlaySound(AudioManager.Instance.swap); // Play swap sound
        Vector3 startPos1 = fossil1.transform.position;
        Vector3 startPos2 = fossil2.transform.position;

        float elapsed = 0f;

        while (elapsed < swapDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / swapDuration);
            //if(fossil1 != null)
                fossil1.transform.position = Vector3.Lerp(startPos1, startPos2, t);
            //if(fossil2 != null)
                fossil2.transform.position = Vector3.Lerp(startPos2, startPos1, t);
            yield return null;
        }
        //if(fossil1 != null)
            fossil1.transform.position = startPos2;
        //if(fossil2 != null)
            fossil2.transform.position = startPos1;
        //if(fossil1 != null && fossil2 != null)
            board.swapManager.CompleteSwap(fossil1, fossil2);
    }

    public IEnumerator AnimateFossilFall(Fossil fossil) {
        Vector3 targetPosition = new Vector3(
            fossil.x - (board.width / 2),
            fossil.y - (board.height / 2),
            0
        );

        Vector3 startPosition = fossil.transform.position;
        float elapsed = 0f;

        while (elapsed < fallDuration) {
            elapsed += Time.deltaTime;
            if(fossil != null)
                fossil.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / fallDuration);
            yield return null;
        }

        if(fossil != null)
            fossil.transform.position = targetPosition;
    }
}
