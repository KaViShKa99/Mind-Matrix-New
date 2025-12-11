// using UnityEngine;
// using System.Collections;

// public class CoinFlyEffect : MonoBehaviour
// {
//     public static CoinFlyEffect Instance;

//     [Header("Coin Prefab (UI Image)")]
//     public GameObject coinPrefab;

//     [Header("Parent Canvas")]
//     public Transform uiParent;

//     [Header("Explosion Settings")]
//     public float explosionMinRadius = 80f;
//     public float explosionMaxRadius = 180f;
//     public float explosionMinTime = 0.3f;
//     public float explosionMaxTime = 0.6f;

//     [Header("Fly Settings")]
//     public float flyMinDuration = 2.0f;
//     public float flyMaxDuration = 3.0f;

//     [Header("Spin Settings")]
//     public float spinMinSpeed = 200f;
//     public float spinMaxSpeed = 600f;

//     private void Awake()
//     {
//         Instance = this;
//     }

//     public void SpawnCoinsRandomExplosion(RectTransform popupStart, RectTransform coinUIEnd, int amount = 12)
//     {
//         StartCoroutine(SpawnEffectCoroutine(popupStart, coinUIEnd, amount));
//     }

//     private IEnumerator SpawnEffectCoroutine(RectTransform popupStart, RectTransform coinUIEnd, int amount)
//     {
//         GameObject[] coins = new GameObject[amount];
//         RectTransform[] rects = new RectTransform[amount];
//         Vector3[] explosionTargets = new Vector3[amount];
//         float[] explosionTimes = new float[amount];
//         float[] flyDurations = new float[amount];
//         float[] spinSpeeds = new float[amount];

//         // 1️⃣ Create coins
//         for (int i = 0; i < amount; i++)
//         {
//             GameObject coin = Instantiate(coinPrefab, uiParent);
//             RectTransform rect = coin.GetComponent<RectTransform>();
//             rect.position = popupStart.position;
//             rect.localScale = Vector3.one * 0.8f;

//             coins[i] = coin;
//             rects[i] = rect;

//             // Random explosion direction & distance
//             Vector2 dir = Random.insideUnitCircle.normalized;
//             float distance = Random.Range(explosionMinRadius, explosionMaxRadius);
//             explosionTargets[i] = popupStart.position + (Vector3)(dir * distance);

//             // Random explosion time
//             explosionTimes[i] = Random.Range(explosionMinTime, explosionMaxTime);

//             // Random fly duration
//             flyDurations[i] = Random.Range(flyMinDuration, flyMaxDuration);

//             // Random spin speed
//             spinSpeeds[i] = Random.Range(spinMinSpeed, spinMaxSpeed);
//         }

//         // 2️⃣ Explosion burst
//         float elapsed = 0f;
//         bool done = false;
//         while (!done)
//         {
//             done = true;
//             for (int i = 0; i < amount; i++)
//             {
//                 if (elapsed < explosionTimes[i])
//                 {
//                     done = false;
//                     float t = elapsed / explosionTimes[i];
//                     t = Mathf.SmoothStep(0, 1, t);
//                     rects[i].position = Vector3.Lerp(popupStart.position, explosionTargets[i], t);
//                     rects[i].Rotate(0, 0, spinSpeeds[i] * Time.deltaTime * 0.8f);
//                 }
//             }
//             elapsed += Time.deltaTime;
//             yield return null;
//         }

//         yield return new WaitForSeconds(0.1f); // pause before fly

//         // 3️⃣ Fly in S-curve
//         for (int i = 0; i < amount; i++)
//         {
//             StartCoroutine(FlySCurveRandom(rects[i], rects[i].position, coinUIEnd.position, flyDurations[i], spinSpeeds[i]));
//         }
//     }

//     private IEnumerator FlySCurveRandom(RectTransform coin, Vector3 start, Vector3 end, float duration, float spinSpeed)
//     {
//         float t = 0f;

//         // Random S-curve control points
//         Vector3 control1 = start + new Vector3(Random.Range(-100f, -50f), Random.Range(100f, 200f), 0);
//         Vector3 control2 = end   + new Vector3(Random.Range(50f, 100f),  Random.Range(100f, 200f), 0);

//         Vector3 startScale = Vector3.one * 0.8f;
//         Vector3 endScale   = Vector3.one * 0.5f;

//         while (t < 1f)
//         {
//             t += Time.deltaTime / duration;
//             float ease = Mathf.SmoothStep(0, 1, t);

//             // Cubic Bezier position
//             Vector3 pos =
//                 Mathf.Pow(1 - ease, 3) * start +
//                 3 * Mathf.Pow(1 - ease, 2) * ease * control1 +
//                 3 * (1 - ease) * Mathf.Pow(ease, 2) * control2 +
//                 Mathf.Pow(ease, 3) * end;

//             coin.position = pos;
//             coin.localScale = Vector3.Lerp(startScale, endScale, ease);

//             // Spin while flying
//             coin.Rotate(0, 0, spinSpeed * Time.deltaTime);

//             yield return null;
//         }

//         Destroy(coin.gameObject);
//     }
// }
using UnityEngine;
using System.Collections;

public class CoinFlyEffect : MonoBehaviour
{
    public static CoinFlyEffect Instance;

    [Header("Coin Prefab (UI Image)")]
    public GameObject coinPrefab;

    [Header("Parent Canvas")]
    public Transform uiParent;

    [Header("Explosion Settings")]
    public float explosionMinRadius = 80f;
    public float explosionMaxRadius = 180f;
    public float explosionMinTime = 0.3f;
    public float explosionMaxTime = 0.6f;

    [Header("Fly Settings")]
    public float flyMinDuration = 2f;
    public float flyMaxDuration = 3f;

    [Header("Spin Settings")]
    public float spinMinSpeed = 200f;
    public float spinMaxSpeed = 600f;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnCoinsRandomExplosion(RectTransform popupStart, RectTransform coinUIEnd, int amount = 12)
    {
        StartCoroutine(SpawnEffectCoroutine(popupStart, coinUIEnd, amount));
    }

    private IEnumerator SpawnEffectCoroutine(RectTransform popupStart, RectTransform coinUIEnd, int amount)
    {
        GameObject[] coins = new GameObject[amount];
        RectTransform[] rects = new RectTransform[amount];
        Vector3[] explosionTargets = new Vector3[amount];
        float[] explosionTimes = new float[amount];
        float[] flyDurations = new float[amount];
        float[] spinSpeeds = new float[amount];

        Vector3 startLocal = uiParent.InverseTransformPoint(popupStart.position);
        Vector3 endLocal = uiParent.InverseTransformPoint(coinUIEnd.position);

        // 1️⃣ Create coins
        for (int i = 0; i < amount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, uiParent);
            RectTransform rect = coin.GetComponent<RectTransform>();
            rect.localPosition = startLocal;
            rect.localScale = Vector3.one * 0.8f;

            coins[i] = coin;
            rects[i] = rect;

            // Random explosion
            Vector2 dir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(explosionMinRadius, explosionMaxRadius);
            explosionTargets[i] = startLocal + (Vector3)(dir * dist);

            explosionTimes[i] = Random.Range(explosionMinTime, explosionMaxTime);
            flyDurations[i] = Random.Range(flyMinDuration, flyMaxDuration);
            spinSpeeds[i] = Random.Range(spinMinSpeed, spinMaxSpeed);
        }

        // 2️⃣ Explosion
        float elapsed = 0f;
        bool done = false;
        while (!done)
        {
            done = true;
            for (int i = 0; i < amount; i++)
            {
                if (elapsed < explosionTimes[i])
                {
                    done = false;
                    float t = Mathf.SmoothStep(0, 1, elapsed / explosionTimes[i]);
                    rects[i].localPosition = Vector3.Lerp(startLocal, explosionTargets[i], t);
                    rects[i].Rotate(0, 0, spinSpeeds[i] * Time.deltaTime * 0.8f);
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        // 3️⃣ Fly to target (S-curve)
        for (int i = 0; i < amount; i++)
        {
            StartCoroutine(FlySCurve(rects[i], rects[i].localPosition, endLocal, flyDurations[i], spinSpeeds[i]));
        }
    }

    private IEnumerator FlySCurve(RectTransform coin, Vector3 start, Vector3 end, float duration, float spinSpeed)
    {
        float t = 0f;

        Vector3 control1 = start + new Vector3(Random.Range(-100f, -50f), Random.Range(100f, 200f), 0f);
        Vector3 control2 = end + new Vector3(Random.Range(50f, 100f), Random.Range(100f, 200f), 0f);

        Vector3 startScale = Vector3.one * 0.8f;
        Vector3 endScale = Vector3.one * 0.4f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float ease = Mathf.SmoothStep(0, 1, t);

            Vector3 pos =
                Mathf.Pow(1 - ease, 3) * start +
                3 * Mathf.Pow(1 - ease, 2) * ease * control1 +
                3 * (1 - ease) * Mathf.Pow(ease, 2) * control2 +
                Mathf.Pow(ease, 3) * end;

            coin.localPosition = pos;
            coin.localScale = Vector3.Lerp(startScale, endScale, ease);
            coin.Rotate(0, 0, spinSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(coin.gameObject);
    }
}
