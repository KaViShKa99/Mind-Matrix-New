// using UnityEngine;
// using DG.Tweening;

// public class ButtonSqueeze : MonoBehaviour
// {
//     public float squeezeAmount = 0.05f; // how much to stretch (0.05–0.2 is good)
//     public float duration = 0.5f;       // speed of squeeze

//     private RectTransform rect;
//     private Sequence seq;

//     void Awake()
//     {
//         rect = GetComponent<RectTransform>();
//     }

//     void OnEnable()
//     {
//         PlaySqueeze();
//     }

//     void OnDisable()
//     {
//         // Safely kill the tween when the object is disabled
//         if (seq != null && seq.IsActive())
//         {
//             seq.Kill();
//             rect.localScale = Vector3.one; // reset scale
//         }
//     }

//     void PlaySqueeze()
//     {
//         if (rect == null) return;

//         // Kill any existing tweens on this rect to avoid overlapping
//         rect.DOKill();

//         // Create a looping sequence
//         seq = DOTween.Sequence();

//         seq.Append(rect.DOScale(new Vector3(1 + squeezeAmount, 1 - squeezeAmount, 1), duration).SetEase(Ease.OutQuad))
//            .Append(rect.DOScale(Vector3.one, duration).SetEase(Ease.InQuad))
//            .SetLoops(-1, LoopType.Restart);
//     }
// }
using UnityEngine;
using DG.Tweening;

public class ButtonSqueeze : MonoBehaviour
{
    public float squeezeAmount = 0.05f; 
    public float duration = 0.5f;       

    private RectTransform rect;
    private Sequence seq;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        // Safety: Ensure rect is assigned if script was added dynamically
        if (rect == null) rect = GetComponent<RectTransform>();
        
        PlaySqueeze();
    }

    void OnDisable()
    {
        KillTween();
    }

    /// <summary>
    /// CRITICAL: OnDestroy is essential. If the object is destroyed, 
    /// OnDisable runs, but DOTween might still have a reference in its 
    /// static manager until the end of the frame.
    /// </summary>
    void OnDestroy()
    {
        KillTween();
    }

    private void KillTween()
    {
        // 1. Kill the sequence specifically
        if (seq != null)
        {
            seq.Kill();
            seq = null;
        }

        // 2. Force kill all tweens on this specific Transform/RectTransform
        if (rect != null)
        {
            rect.DOKill();
            rect.localScale = Vector3.one; 
        }
    }

    void PlaySqueeze()
    {
        if (rect == null) return;

        // Ensure we aren't stacking tweens
        rect.DOKill();

        seq = DOTween.Sequence();

        // Added .SetTarget(this) so DOTween knows this tween belongs to this object lifecycle
        seq.Append(rect.DOScale(new Vector3(1 + squeezeAmount, 1 - squeezeAmount, 1), duration).SetEase(Ease.OutQuad))
           .Append(rect.DOScale(Vector3.one, duration).SetEase(Ease.InQuad))
           .SetLoops(-1, LoopType.Yoyo) // Yoyo is usually smoother for "squeezing" than Restart
           .SetTarget(this); 
    }
}