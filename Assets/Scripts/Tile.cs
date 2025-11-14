// using UnityEngine;
// using UnityEngine.EventSystems;

// public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
// {
//     public TileManager tileManager;
//     private Vector2 pointerDownPos;

//     void Start()
//     {
//         if (tileManager == null)
//             tileManager = Object.FindFirstObjectByType<TileManager>();
//     }

//     public void OnPointerDown(PointerEventData eventData)
//     {
//         pointerDownPos = eventData.position;
//     }

//     public void OnPointerUp(PointerEventData eventData)
//     {
//         Vector2 pointerUpPos = eventData.position;
//         Vector2 swipe = pointerUpPos - pointerDownPos;

//         if (swipe.magnitude < 30f)
//         {
//             // Tap
//             tileManager.TryMoveTile(transform);
//             return;
//         }
//         // tileManager.SwipeTile(transform, swipe);
//     }
// }
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TileManager tileManager;
    private Vector2 pointerDownPos;
    private Vector2 pointerUpPos;

    [Header("Swipe Settings")]
    public float swipeThreshold = 50f; // Minimum distance to register swipe

    void Start()
    {
        if (tileManager == null)
            tileManager = Object.FindFirstObjectByType<TileManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerUpPos = eventData.position;
        Vector2 swipe = pointerUpPos - pointerDownPos;

        // Small movement = tap
        if (swipe.magnitude < swipeThreshold)
        {
            tileManager.TryMoveTile(transform);
            return;
        }

        // Detect main swipe direction
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            // Horizontal swipe
            if (swipe.x > 0)
                tileManager.SwipeTile(transform, Vector2.right);  // → Swipe right
            else
                tileManager.SwipeTile(transform, Vector2.left);   // ← Swipe left
        }
        else
        {
            // Vertical swipe
            if (swipe.y > 0)
                tileManager.SwipeTile(transform, Vector2.up);     // ↑ Swipe up
            else
                tileManager.SwipeTile(transform, Vector2.down);   // ↓ Swipe down
        }
    }
}
