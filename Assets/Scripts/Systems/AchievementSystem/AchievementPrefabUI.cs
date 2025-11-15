using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AchievementPrefabUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI titleText;
    public Image completeIcon;
    public Button infoBtn;

    private AchievementData data;
    private Action<AchievementData> onInfoClicked;

    public void Init(AchievementData achievement, Action<AchievementData> infoCallback)
    {
        data = achievement;
        onInfoClicked = infoCallback;

        titleText.text = achievement.title;
        icon.sprite = achievement.icon;

        completeIcon.gameObject.SetActive(!achievement.isUnlocked);

        infoBtn.onClick.AddListener(InfoBtnClick);
    }

    private void InfoBtnClick()
    {
        Debug.Log("[Achievement] Info clicked → " + data.title);
        onInfoClicked?.Invoke(data);
    }
}
