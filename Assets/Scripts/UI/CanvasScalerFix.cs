using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerFix : MonoBehaviour
{
    void Start()
    {
        var scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

    #if UNITY_ANDROID
            scaler.matchWidthOrHeight = 0f;   // Android = match width
    #elif UNITY_WEBGL
            scaler.matchWidthOrHeight = 1f;   // WebGL = match height
    #else
            scaler.matchWidthOrHeight = 0.5f; // Editor or other = neutral
    #endif
    }
}
