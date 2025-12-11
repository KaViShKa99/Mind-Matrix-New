using UnityEngine;
using System.Collections;

public class TileSqueeze : MonoBehaviour
{
    public float squeezeAmount = 0.1f;   // How much it squeezes
    public float duration = 0.12f;       // Speed of squeeze

    private bool isAnimating = false;

    public void PlaySqueeze()
    {
        if (!isAnimating)
            StartCoroutine(SqueezeAnimation());
    }

    IEnumerator SqueezeAnimation()
    {
        isAnimating = true;

        Vector3 original = transform.localScale;
        Vector3 squeezed = new Vector3(original.x - squeezeAmount, original.y + squeezeAmount, original.z);

        float t = 0;

        // Squeeze phase
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(original, squeezed, t);
            yield return null;
        }

        t = 0;

        // Expand back
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(squeezed, original, t);
            yield return null;
        }

        transform.localScale = original;
        isAnimating = false;
    }
}
