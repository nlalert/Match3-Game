using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {
    public BoardManager board;
    public bool isAnimating = false;

    public IEnumerator AnimateSwap(Candy candy1, Candy candy2, float duration = 0.2f) {
        AudioManager.Instance.PlaySound(AudioManager.Instance.swapSound); // Play swap sound
        Vector3 startPos1 = candy1.transform.position;
        Vector3 startPos2 = candy2.transform.position;

        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            candy1.transform.position = Vector3.Lerp(startPos1, startPos2, t);
            candy2.transform.position = Vector3.Lerp(startPos2, startPos1, t);
            yield return null;
        }

        candy1.transform.position = startPos2;
        candy2.transform.position = startPos1;

        board.swapManager.CompleteSwap(candy1, candy2);
    }

    public IEnumerator AnimateCandyFall(Candy candy, float duration = 0.2f) {
        AudioManager.Instance.PlaySound(AudioManager.Instance.fallSound); // Play fall sound

        Vector3 targetPosition = new Vector3(
            candy.x - (board.width / 2),
            candy.y - (board.height / 2),
            0
        );

        Vector3 startPosition = candy.transform.position;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            candy.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        candy.transform.position = targetPosition;
    }
}
