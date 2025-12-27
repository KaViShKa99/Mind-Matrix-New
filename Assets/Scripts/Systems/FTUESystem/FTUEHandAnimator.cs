using UnityEngine;
using System.Collections;

public class FTUEHandAnimator : MonoBehaviour
{
    public RectTransform hand;
    public float dragDuration = 0.6f;
    public float pauseDuration = 0.3f;

    private Coroutine routine;

    // Play hand animation along one move
    public void Play(Vector3 startPos, Vector3 endPos)
    {
        Stop();
        routine = StartCoroutine(AnimateLoop(startPos, endPos));
    }

    public void Stop()
    {
        if (routine != null) StopCoroutine(routine);
        hand.gameObject.SetActive(false);
    }

    private IEnumerator AnimateLoop(Vector3 startPos, Vector3 endPos)
    {
        hand.gameObject.SetActive(true);

        while (true)
        {
            hand.position = startPos;
            yield return new WaitForSeconds(pauseDuration);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / dragDuration;
                hand.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            yield return new WaitForSeconds(pauseDuration);
        }
    }
}
