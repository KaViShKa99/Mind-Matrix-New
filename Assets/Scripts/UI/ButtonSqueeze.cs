using UnityEngine;
using DG.Tweening;

public class ButtonSqueeze : MonoBehaviour
{
    public float squeezeAmount = 0.05f; // how much to stretch (0.05–0.2 is good)
    public float duration = 0.5f;       // speed of squeeze

    private RectTransform rect;
    private Sequence seq;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        PlaySqueeze();
    }

    void OnDisable()
    {
        // Safely kill the tween when the object is disabled
        if (seq != null && seq.IsActive())
        {
            seq.Kill();
            rect.localScale = Vector3.one; // reset scale
        }
    }

    void PlaySqueeze()
    {
        if (rect == null) return;

        // Kill any existing tweens on this rect to avoid overlapping
        rect.DOKill();

        // Create a looping sequence
        seq = DOTween.Sequence();

        seq.Append(rect.DOScale(new Vector3(1 + squeezeAmount, 1 - squeezeAmount, 1), duration).SetEase(Ease.OutQuad))
           .Append(rect.DOScale(Vector3.one, duration).SetEase(Ease.InQuad))
           .SetLoops(-1, LoopType.Restart);
    }
}
// using UnityEngine;
// using DG.Tweening;

// public class ButtonPulse : MonoBehaviour
// {
//     public float scaleAmount = 1.1f; // how much to scale up
//     public float duration = 0.5f;    // speed of scale up/down

//     private RectTransform rect;
//     private Sequence seq;

//     void Awake()
//     {
//         rect = GetComponent<RectTransform>();
//     }

//     void OnEnable()
//     {
//         PlayPulse();
//     }

//     void OnDisable()
//     {
//         if (seq != null && seq.IsActive())
//         {
//             seq.Kill();
//             rect.localScale = Vector3.one; // reset scale
//         }
//     }

//     void PlayPulse()
//     {
//         if (rect == null) return;

//         rect.DOKill(); // kill any previous tweens

//         seq = DOTween.Sequence();
//         seq.Append(rect.DOScale(scaleAmount * Vector3.one, duration).SetEase(Ease.OutQuad)) // scale up
//            .Append(rect.DOScale(Vector3.one, duration).SetEase(Ease.InQuad))              // scale back to normal
//            .SetLoops(-1, LoopType.Restart);                                               // loop infinitely
//     }
// }
