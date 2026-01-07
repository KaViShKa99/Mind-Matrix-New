// using UnityEngine;
// using UnityEngine.UI;
// using DG.Tweening;

// public class PopupBoxUI : MonoBehaviour
// {
//     [Header("Popup Elements")]
//     public GameObject popupBox;
//     public CanvasGroup popupCanvasGroup;
//     public Image popupBackground;

//     [Header("Star UI (Optional)")]
//     public Image[] starImages;           // Assign star images (0–3)
//     public Sprite filledStar;            // Yellow star sprite
//     public Sprite emptyStar;             // Grey/empty star sprite

//     [Header("Animation Settings")]
//     public float popupOpenTime = 0.5f;
//     public float popupCloseTime = 0.4f;

//     private Sequence openSequence;
//     private Sequence closeSequence;

//     private int pendingStars = -1; // Will store stars to animate

//     private void Awake()
//     {
//         if (popupCanvasGroup == null)
//             popupCanvasGroup = popupBox.GetComponent<CanvasGroup>();
//     }

//     // ==========================================================
//     // 🔵 SHOW POPUP (Normal Popup)
//     // ==========================================================
//     public void ShowPopup()
//     {
//         // Stop any old animations
//         openSequence?.Kill();
//         closeSequence?.Kill();

//         popupBox.SetActive(true);
//         popupBackground.gameObject.SetActive(true);

//         popupCanvasGroup.alpha = 0f;
//         popupBox.transform.localScale = Vector3.one * 0.6f;
//         popupBackground.color = new Color(0, 0, 0, 0);

//         openSequence = DOTween.Sequence();

//         openSequence
//             .Append(popupBackground.DOFade(0.85f, popupOpenTime * 0.8f))
//             .Join(popupBox.transform.DOScale(1.15f, popupOpenTime * 0.6f).SetEase(Ease.OutBack))
//             .Append(popupBox.transform.DOScale(1f, popupOpenTime * 0.4f).SetEase(Ease.OutBack))
//             .Join(popupCanvasGroup.DOFade(1f, popupOpenTime))
//             .OnComplete(() =>
//             {
//                 // Animate stars only if needed
//                 if (pendingStars >= 0)
//                 {
//                     AnimateStars(pendingStars);
//                     pendingStars = -1;
//                 }
//             });
//     }

//     // ==========================================================
//     // 🔵 SHOW POPUP WITH STARS (Reads PlayerPrefs)
//     // ==========================================================
//     public void ShowPopupForLevel(int levelIndex)
//     {
//         pendingStars = PlayerPrefs.GetInt("Stars_Level_" + levelIndex, 0);
//         Debug.Log("lvlstarsssssss: "+levelIndex);
//         Debug.Log("starsssssss: "+pendingStars);

//         ShowPopup();
//     }

//     // ==========================================================
//     // ⭐ STAR ANIMATION SYSTEM
//     // ==========================================================
//     private void AnimateStars(int stars)
//     {
//         if (starImages == null || starImages.Length == 0)
//             return; // No stars in this popup → skip

//         for (int i = 0; i < starImages.Length; i++)
//         {
//             bool active = i < stars;

//             starImages[i].sprite = active ? filledStar : emptyStar;
//             starImages[i].color = active ? Color.white : new Color(0, 0, 0, 0.25f);

//             starImages[i].transform.localScale = Vector3.zero;

//             if (active)
//             {
//                 starImages[i].transform.DOScale(1f, 0.4f)
//                     .SetEase(Ease.OutBack)
//                     .SetDelay(0.15f * i);
//             }
//             else
//             {
//                 starImages[i].transform.localScale = Vector3.one;
//             }
//         }
//     }

    

    


//     // ==========================================================
//     // 🔴 CLOSE POPUP
//     // ==========================================================
//     public Tween ClosedPopup()
//     {
//         openSequence?.Kill();
//         closeSequence?.Kill();

//         closeSequence = DOTween.Sequence();

//         closeSequence
//             .Append(popupCanvasGroup.DOFade(0f, popupCloseTime))
//             .Join(popupBox.transform.DOScale(0.6f, popupCloseTime).SetEase(Ease.InBack))
//             .Join(popupBackground.DOFade(0f, popupCloseTime))
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

    [Header("Buttons to Reveal")]
    public CanvasGroup actionButtonsCG; // Put your Next and Home buttons in a group
    public GameObject nextButton;
    public float buttonFadeTime = 0.3f;

    [Header("Star UI (Optional)")]
    public Image[] starImages;           // Assign star images (0–3)
    public Sprite filledStar;            // Yellow star sprite
    public Sprite emptyStar;             // Grey/empty star sprite

    [Header("Animation Settings")]
    public float popupOpenTime = 0.5f;
    public float popupCloseTime = 0.4f;

    private Sequence openSequence;
    private Sequence closeSequence;

    private int pendingStars = -1; 

    private void Awake()
    {
        if (popupCanvasGroup == null && popupBox != null)
            popupCanvasGroup = popupBox.GetComponent<CanvasGroup>();
        
        // Ensure buttons are hidden at the very start
        if (actionButtonsCG != null) actionButtonsCG.alpha = 0f;
    }

    /// <summary>
    /// Critical Fix: Ensures no orphaned tweens run after the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        KillAllActiveTweens();
    }

    private void KillAllActiveTweens()
    {
        // Kill sequences
        openSequence?.Kill();
        closeSequence?.Kill();
        actionButtonsCG?.DOKill(); // Kill button animations

        // Kill specific transform tweens to prevent NullReferenceException on localScale
        if (popupBox != null) popupBox.transform.DOKill();
        if (popupBackground != null) popupBackground.transform.DOKill();
        if (popupCanvasGroup != null) popupCanvasGroup.transform.DOKill();

        // Kill star tweens
        if (starImages != null)
        {
            foreach (var star in starImages)
            {
                if (star != null) star.transform.DOKill();
            }
        }
    }

    public void ShowPopup()
    {
        // Stop any old animations before starting new ones
        KillAllActiveTweens();

        // 1. Hide buttons immediately when opening
        if (actionButtonsCG != null)
        {
            actionButtonsCG.alpha = 0f;
            actionButtonsCG.interactable = false;
            actionButtonsCG.blocksRaycasts = false;
        }

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
                if (pendingStars >= 0)
                {
                    AnimateStars(pendingStars);
                    pendingStars = -1;
                }else
                {
                    // If there are no stars to animate, show buttons immediately
                    ShowButtons();
                }
            });
    }
    private void ShowButtons()
    {
        if (actionButtonsCG == null) return;

        actionButtonsCG.DOFade(1f, buttonFadeTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                actionButtonsCG.interactable = true;
                actionButtonsCG.blocksRaycasts = true;
            });

        int currentLevel = LevelStageManager.Instance.SelectedLevel;

        if(nextButton != null && currentLevel == 112)
        {
            nextButton.SetActive(false);
        }
    }

    public void ShowPopupForLevel(int levelIndex)
    {
        pendingStars = PlayerPrefs.GetInt("Stars_Level_" + levelIndex, 0);
        ShowPopup();
    }

    private void AnimateStars(int stars)
    {
        if (starImages == null || starImages.Length == 0)
            return; 

        int starsAnimatedCount = 0;
        int totalStarsToAnimate = 0;

        // Calculate how many stars will actually perform a scale animation
        for (int i = 0; i < starImages.Length; i++) 
            if (i < stars) totalStarsToAnimate++;

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null) continue;

            // Kill any previous tween on this specific star before starting a new one
            starImages[i].transform.DOKill();

            bool active = i < stars;
            starImages[i].sprite = active ? filledStar : emptyStar;
            starImages[i].color = active ? Color.white : new Color(0, 0, 0, 0.25f);

            if (active)
            {
                starImages[i].transform.localScale = Vector3.zero;
                // SetTarget(this) links the tween to this object's lifecycle
                starImages[i].transform.DOScale(1f, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.15f * i)
                    .SetTarget(this) 
                    .OnComplete(() => {
                        starsAnimatedCount++;
                        // 2. CHECK: If this was the last star to finish, show the buttons
                        if (starsAnimatedCount >= totalStarsToAnimate)
                        {
                            ShowButtons();
                        }
                    });
            }
            else
            {
                starImages[i].transform.localScale = Vector3.one;
            }
        }
    }

    public Tween ClosedPopup()
    {
        // Stop opening/star animations if user closes early
        KillAllActiveTweens();

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