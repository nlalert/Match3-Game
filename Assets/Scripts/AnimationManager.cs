using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour{

    // https://docs.unity3d.com/ScriptReference/Coroutine.html
    
    public BoardManager board;
    public bool isAnimating = false;
    public float swapDuration = 0.15f;
    public float fallDuration = 0.15f;

    // Animates the swapping of two fossils on the board
    public IEnumerator AnimateSwap(Fossil fossil1, Fossil fossil2) {
        // Play swap sound effect
        AudioManager.Instance.PlaySound(AudioManager.Instance.swap);

        // start positions of both fossils
        Vector3 startPos1 = fossil1.transform.position;
        Vector3 startPos2 = fossil2.transform.position;
        float elapsed = 0f;

        // Animate the swapping
        while (elapsed < swapDuration){
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / swapDuration);

            if (fossil1 != null)
                fossil1.transform.position = Vector3.Lerp(startPos1, startPos2, t);
            if (fossil2 != null)
                fossil2.transform.position = Vector3.Lerp(startPos2, startPos1, t);

            yield return null;
        }

        // Set fossils to final position
        if (fossil1 != null)
            fossil1.transform.position = startPos2;
        if (fossil2 != null)
            fossil2.transform.position = startPos1;

        board.swapManager.CompleteSwap(fossil1, fossil2);
    }

    // Animates a fossil falling to position on the board
    public IEnumerator AnimateFossilFall(Fossil fossil) {
        if (fossil == null)
            yield break;

        // Calculate the target position for the fossil
        Vector3 targetPosition = new Vector3(
            fossil.x - (board.width / 2),
            fossil.y - (board.height / 2),
            0
        );

        Vector3 startPosition = fossil.transform.position;
        float elapsed = 0f;

        // Animate the falling
        while (elapsed < fallDuration){
            elapsed += Time.deltaTime;
            if (fossil != null)
                fossil.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / fallDuration);
            yield return null;
        }

        // Set fossils to final position
        if (fossil != null)
            fossil.transform.position = targetPosition;
    }
}
