
// using UnityEngine;
// using System.Collections;

// public class ButtonShineController : MonoBehaviour
// {
//     public Material buttonMaterial;       // Assign your DiagonalShiny material
//     public float shineDuration = 0.8f;    // Time it takes to sweep
//     public float waitTime = 2f;           // Time to wait between shines

//     private void Start()
//     {
//         if (buttonMaterial == null)
//         {
//             Debug.LogError("Assign your DiagonalShiny material!");
//             return;
//         }

//         StartCoroutine(ShineRoutine());
//     }

//     private IEnumerator ShineRoutine()
//     {
//         while (true)
//         {
//             // Animate shine from start (-1) to end (2)
//             float elapsed = 0f;
//             while (elapsed < shineDuration)
//             {
//                 float offset = Mathf.Lerp(-1f, 2f, elapsed / shineDuration);
//                 buttonMaterial.SetFloat("_ShineOffset", offset);
//                 elapsed += Time.deltaTime;
//                 yield return null;
//             }

//             // Ensure shine ends exactly
//             buttonMaterial.SetFloat("_ShineOffset", 2f);

//             // Wait before next shine
//             yield return new WaitForSeconds(waitTime);
//         }
//     }
// }
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonShineController : MonoBehaviour
{
    public Material originalMaterial;       // Assign your DiagonalShiny material
    private Material runtimeMaterial;       // Unique material instance

    public float shineDuration = 0.8f;
    public float waitTime = 2f;

    private void Start()
    {
        if (originalMaterial == null)
        {
            Debug.LogError("Assign your DiagonalShiny material!");
            return;
        }

        // Create a UNIQUE material for this button
        runtimeMaterial = new Material(originalMaterial);

        // Assign the unique material to UI Image
        GetComponent<Image>().material = runtimeMaterial;

        StartCoroutine(ShineRoutine());
    }

    private IEnumerator ShineRoutine()
    {
        while (true)
        {
            float elapsed = 0f;

            while (elapsed < shineDuration)
            {
                float offset = Mathf.Lerp(-1f, 2f, elapsed / shineDuration);
                runtimeMaterial.SetFloat("_ShineOffset", offset);
                elapsed += Time.deltaTime;
                yield return null;
            }

            runtimeMaterial.SetFloat("_ShineOffset", 2f);

            yield return new WaitForSeconds(waitTime);
        }
    }
}
