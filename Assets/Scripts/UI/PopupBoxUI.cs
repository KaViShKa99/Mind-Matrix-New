// using UnityEngine;

// public class PopupBoxUI : MonoBehaviour
// {
//     public GameObject popup;
//     public Animator popupAnimator;

//     public void ShowPopup()
//     {
//         if (popup == null) return;
//         popup.SetActive(true);
//         if (popupAnimator != null) popupAnimator.SetTrigger("PopupOpen");
//     }
//     public void ClosedPopup()
//     {
//         if (popup == null) return;
//         popup.SetActive(false);
//     }


// }

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupBoxUI : MonoBehaviour
{
    [Header("Popup Elements")]
    public GameObject popupBox;
    public CanvasGroup popupCanvasGroup;
    public Image popupBackground;

    [Header("Animation Settings")]
    public float popupOpenTime = 0.5f;
    public float popupCloseTime = 0.4f;

    private Sequence openSequence;
    private Sequence closeSequence;

    private void Awake()
    {
        if (popupCanvasGroup == null)
            popupCanvasGroup = popupBox.GetComponent<CanvasGroup>();
    }

    public void ShowPopup()
    {
        // Kill any running animations (VERY IMPORTANT)
        openSequence?.Kill();
        closeSequence?.Kill();

        // Enable UI
        popupBox.SetActive(true);
        popupBackground.gameObject.SetActive(true);

        // Reset values
        popupCanvasGroup.alpha = 0f;
        popupBox.transform.localScale = Vector3.one * 0.6f;
        popupBackground.color = new Color(0, 0, 0, 0);

        // Create open animation
        openSequence = DOTween.Sequence();

        openSequence
            // Background fade
            .Append(popupBackground.DOFade(0.85f, popupOpenTime * 0.8f))

            // Popup scale bounce
            .Join(popupBox.transform.DOScale(1.15f, popupOpenTime * 0.6f).SetEase(Ease.OutBack))
            .Append(popupBox.transform.DOScale(1f, popupOpenTime * 0.4f).SetEase(Ease.OutBack))

            // Fade popup content
            .Join(popupCanvasGroup.DOFade(1f, popupOpenTime))
            ;
    }

    public Tween ClosedPopup()
    {
        // Kill other tweens
        openSequence?.Kill();
        closeSequence?.Kill();

        closeSequence = DOTween.Sequence();

        closeSequence
            // Fade out popup
            .Append(popupCanvasGroup.DOFade(0f, popupCloseTime))
            .Join(popupBox.transform.DOScale(0.6f, popupCloseTime).SetEase(Ease.InBack))

            // Fade out background
            .Join(popupBackground.DOFade(0f, popupCloseTime))

            // Disable after animation
            .OnComplete(() =>
            {
                popupBox.SetActive(false);
                popupBackground.gameObject.SetActive(false);
            });

        return closeSequence;
    }
}
