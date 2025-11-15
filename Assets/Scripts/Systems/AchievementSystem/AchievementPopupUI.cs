using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementPopupUI : MonoBehaviour
{
    [Header("Popup References")]
    public GameObject popup;
    public Animator popupAnimator;
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;


    public void ShowPopup(AchievementData data)
    {
        if (popup == null) return;

        popup.SetActive(true);

        icon.sprite = data.icon;
        title.text = data.title;
        description.text = data.description;


        if (popupAnimator != null)
            popupAnimator.SetTrigger("PopupOpen");
    }

    public void ClosedPopup()
    {
        if (popup == null) return;
        popup.SetActive(false);
    }

    public void OnPopupOpenComplete()
    {
        Debug.Log("Popup animation finished!");
        // You can add any logic here
    }



}
