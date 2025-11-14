using UnityEngine;

public class PopupBoxUI : MonoBehaviour
{
    public GameObject popup;
    public Animator popupAnimator;

    public void ShowPopup()
    {
        if (popup == null) return;
        popup.SetActive(true);
        if (popupAnimator != null) popupAnimator.SetTrigger("PopupOpen");
    }
    public void ClosedPopup()
    {
        if (popup == null) return;
        popup.SetActive(false);
    }

}
