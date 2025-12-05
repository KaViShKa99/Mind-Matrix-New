// using UnityEngine;
// using UnityEngine.UI;
// using DG.Tweening;

// public class PopupBoxUI : MonoBehaviour
// {
//     [Header("Popup Elements")]
//     public GameObject popupBox;
//     public CanvasGroup popupCanvasGroup;
//     public Image popupBackground;

//     [Header("Animation Settings")]
//     public float popupOpenTime = 0.5f;
//     public float popupCloseTime = 0.4f;

//     private Sequence openSequence;
//     private Sequence closeSequence;

//     private void Awake()
//     {
//         if (popupCanvasGroup == null)
//             popupCanvasGroup = popupBox.GetComponent<CanvasGroup>();
//     }

//     public void ShowPopup()
//     {
//         // Kill any running animations (VERY IMPORTANT)
//         openSequence?.Kill();
//         closeSequence?.Kill();

//         // Enable UI
//         popupBox.SetActive(true);
//         popupBackground.gameObject.SetActive(true);

//         // Reset values
//         popupCanvasGroup.alpha = 0f;
//         popupBox.transform.localScale = Vector3.one * 0.6f;
//         popupBackground.color = new Color(0, 0, 0, 0);

//         // Create open animation
//         openSequence = DOTween.Sequence();

//         openSequence
//             // Background fade
//             .Append(popupBackground.DOFade(0.85f, popupOpenTime * 0.8f))

//             // Popup scale bounce
//             .Join(popupBox.transform.DOScale(1.15f, popupOpenTime * 0.6f).SetEase(Ease.OutBack))
//             .Append(popupBox.transform.DOScale(1f, popupOpenTime * 0.4f).SetEase(Ease.OutBack))

//             // Fade popup content
//             .Join(popupCanvasGroup.DOFade(1f, popupOpenTime))
//             ;
//     }

//     public Tween ClosedPopup()
//     {
//         // Kill other tweens
//         openSequence?.Kill();
//         closeSequence?.Kill();

//         closeSequence = DOTween.Sequence();

//         closeSequence
//             // Fade out popup
//             .Append(popupCanvasGroup.DOFade(0f, popupCloseTime))
//             .Join(popupBox.transform.DOScale(0.6f, popupCloseTime).SetEase(Ease.InBack))

//             // Fade out background
//             .Join(popupBackground.DOFade(0f, popupCloseTime))

//             // Disable after animation
//             .OnComplete(() =>
//             {
//                 popupBox.SetActive(false);
//                 popupBackground.gameObject.SetActive(false);
//             });

//         return closeSequence;
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

    [Header("Star UI (Optional)")]
    public Image[] starImages;           // Assign star images (0–3)
    public Sprite filledStar;            // Yellow star sprite
    public Sprite emptyStar;             // Grey/empty star sprite

    [Header("Animation Settings")]
    public float popupOpenTime = 0.5f;
    public float popupCloseTime = 0.4f;

    private Sequence openSequence;
    private Sequence closeSequence;

    private int pendingStars = -1; // Will store stars to animate

    private void Awake()
    {
        if (popupCanvasGroup == null)
            popupCanvasGroup = popupBox.GetComponent<CanvasGroup>();
    }

    // ==========================================================
    // 🔵 SHOW POPUP (Normal Popup)
    // ==========================================================
    public void ShowPopup()
    {
        // Stop any old animations
        openSequence?.Kill();
        closeSequence?.Kill();

        popupBox.SetActive(true);
        popupBackground.gameObject.SetActive(true);

        popupCanvasGroup.alpha = 0f;
        popupBox.transform.localScale = Vector3.one * 0.6f;
        popupBackground.color = new Color(0, 0, 0, 0);

        openSequence = DOTween.Sequence();

        openSequence
            .Append(popupBackground.DOFade(0.85f, popupOpenTime * 0.8f))
            .Join(popupBox.transform.DOScale(1.15f, popupOpenTime * 0.6f).SetEase(Ease.OutBack))
            .Append(popupBox.transform.DOScale(1f, popupOpenTime * 0.4f).SetEase(Ease.OutBack))
            .Join(popupCanvasGroup.DOFade(1f, popupOpenTime))
            .OnComplete(() =>
            {
                // Animate stars only if needed
                if (pendingStars >= 0)
                {
                    AnimateStars(pendingStars);
                    pendingStars = -1;
                }
            });
    }

    // ==========================================================
    // 🔵 SHOW POPUP WITH STARS (Reads PlayerPrefs)
    // ==========================================================
    public void ShowPopupForLevel(int levelIndex)
    {
        pendingStars = PlayerPrefs.GetInt("Stars_Level_" + levelIndex, 0);
        Debug.Log("lvlstarsssssss: "+levelIndex);
        Debug.Log("starsssssss: "+pendingStars);

        ShowPopup();
    }

    // ==========================================================
    // ⭐ STAR ANIMATION SYSTEM
    // ==========================================================
    private void AnimateStars(int stars)
    {
        if (starImages == null || starImages.Length == 0)
            return; // No stars in this popup → skip

        for (int i = 0; i < starImages.Length; i++)
        {
            bool active = i < stars;

            starImages[i].sprite = active ? filledStar : emptyStar;
            starImages[i].color = active ? Color.white : new Color(1, 1, 1, 0.25f);

            starImages[i].transform.localScale = Vector3.zero;

            if (active)
            {
                starImages[i].transform.DOScale(1f, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.15f * i);
            }
            else
            {
                starImages[i].transform.localScale = Vector3.one;
            }
        }
    }

    

    


    // ==========================================================
    // 🔴 CLOSE POPUP
    // ==========================================================
    public Tween ClosedPopup()
    {
        openSequence?.Kill();
        closeSequence?.Kill();

        closeSequence = DOTween.Sequence();

        closeSequence
            .Append(popupCanvasGroup.DOFade(0f, popupCloseTime))
            .Join(popupBox.transform.DOScale(0.6f, popupCloseTime).SetEase(Ease.InBack))
            .Join(popupBackground.DOFade(0f, popupCloseTime))
            .OnComplete(() =>
            {
                popupBox.SetActive(false);
                popupBackground.gameObject.SetActive(false);
            });

        return closeSequence;
    }
}
