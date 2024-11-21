using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {
    public BoardManager board;
    public bool isAnimating = false;

    public IEnumerator AnimateSwap(Fossil fossil1, Fossil fossil2, float duration = 0.2f) {
        AudioManager.Instance.PlaySound(AudioManager.Instance.swapSound); // Play swap sound
        Vector3 startPos1 = fossil1.transform.position;
        Vector3 startPos2 = fossil2.transform.position;

        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            fossil1.transform.position = Vector3.Lerp(startPos1, startPos2, t);
            fossil2.transform.position = Vector3.Lerp(startPos2, startPos1, t);
            yield return null;
        }

        fossil1.transform.position = startPos2;
        fossil2.transform.position = startPos1;

        board.swapManager.CompleteSwap(fossil1, fossil2);
    }

    public IEnumerator AnimateFossilFall(Fossil fossil, float duration = 0.15f) {
        AudioManager.Instance.PlaySound(AudioManager.Instance.fallSound); // Play fall sound

        Vector3 targetPosition = new Vector3(
            fossil.x - (board.width / 2),
            fossil.y - (board.height / 2),
            0
        );

        Vector3 startPosition = fossil.transform.position;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            if(fossil != null)
                fossil.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        if(fossil != null)
            fossil.transform.position = targetPosition;
    }
}
