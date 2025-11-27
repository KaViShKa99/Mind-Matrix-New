using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform rect;
    private Rect lastSafeArea = new Rect(0,0,0,0);

    // Add extra padding if needed
    public Vector2 extraTopBottomPadding = new Vector2(0,0); // x = top, y = bottom

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        if(Screen.safeArea != lastSafeArea)
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        // Apply extra padding for tricky notches/punch-holes
        safeArea.y += extraTopBottomPadding.y;
        safeArea.height -= extraTopBottomPadding.y + extraTopBottomPadding.x;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
    }
}
