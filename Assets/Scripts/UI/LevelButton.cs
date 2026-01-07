// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using DG.Tweening;


// public class LevelButton : MonoBehaviour
// {
//     [Header("Star UI (Optional)")]
//     public Image[] starImages;           // Assign star images (0–3)
//     public Sprite filledStar;            // Yellow star sprite
//     public Sprite emptyStar; 
//     private int pendingStars = -1; // Will store stars to animate
  

//     public TextMeshProUGUI levelText;
//     public GameObject lockIcon; // assign lock icon in prefab
//     private int levelNumber;
//     private Button button;
//     private Color normalColor;



//     public void Setup(int number, bool isUnlocked)
//     {
//         levelNumber = number;
//         levelText.text = number.ToString();

//         if (button == null)
//             button = GetComponent<Button>();

//         // Save the button’s normal color
//         normalColor = button.colors.normalColor;

//         // Keep button looking normal, even if not interactable
//         var colors = button.colors;
//         colors.disabledColor = normalColor;
//         button.colors = colors;

//         // Apply unlock state
//         button.interactable = isUnlocked;
//         if (lockIcon != null)
//             lockIcon.SetActive(!isUnlocked);

//         AnimateStars();
//     }

//     private void AnimateStars()
//     {
//         pendingStars = PlayerPrefs.GetInt("Stars_Level_" + levelNumber, 0);

//         if (starImages == null || starImages.Length == 0)
//             return; // No stars in this popup → skip

//         for (int i = 0; i < starImages.Length; i++)
//         {
//             bool active = i < pendingStars;

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

//     public void OnClick()
//     {
//         if (!GetComponent<Button>().interactable)
//             return;

//         LevelStageManager.Instance.SetSelectedLevel(levelNumber);
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelButton : MonoBehaviour
{
    [Header("Star UI (Optional)")]
    public Image[] starImages;           
    public Sprite filledStar;            
    public Sprite emptyStar; 
    private int pendingStars = -1; 

    public TextMeshProUGUI levelText;
    public GameObject lockIcon; 
    private int levelNumber;
    private Button button;
    private Color normalColor;

    /// <summary>
    /// FIX: Added OnDestroy to stop all star animations if the level 
    /// select menu is closed or the scene changes.
    /// </summary>
    private void OnDestroy()
    {
        if (starImages != null)
        {
            foreach (var star in starImages)
            {
                if (star != null) star.transform.DOKill();
            }
        }
    }

    public void Setup(int number, bool isUnlocked)
    {
        levelNumber = number;
        levelText.text = number.ToString();

        if (button == null)
            button = GetComponent<Button>();

        normalColor = button.colors.normalColor;

        var colors = button.colors;
        colors.disabledColor = normalColor;
        button.colors = colors;

        button.interactable = isUnlocked;
        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);

        AnimateStars();
    }

    private void AnimateStars()
    {
        pendingStars = PlayerPrefs.GetInt("Stars_Level_" + levelNumber, 0);

        if (starImages == null || starImages.Length == 0)
            return; 

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null) continue; // Safety check

            // FIX: Kill any existing tween on this star before starting a new one
            starImages[i].transform.DOKill();

            bool active = i < pendingStars;
            starImages[i].sprite = active ? filledStar : emptyStar;
            starImages[i].color = active ? Color.white : new Color(0, 0, 0, 0.25f);

            if (active)
            {
                starImages[i].transform.localScale = Vector3.zero;
                // FIX: Added .SetTarget(this) so DOTween handles the lifecycle better
                starImages[i].transform.DOScale(1f, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.15f * i)
                    .SetTarget(this);
            }
            else
            {
                starImages[i].transform.localScale = Vector3.one;
            }
        }
    }

    public void OnClick()
    {
        // Added null check for the button component
        if (button == null) button = GetComponent<Button>();
        
        if (!button.interactable)
            return;

        LevelStageManager.Instance.SetSelectedLevel(levelNumber);
    }
}