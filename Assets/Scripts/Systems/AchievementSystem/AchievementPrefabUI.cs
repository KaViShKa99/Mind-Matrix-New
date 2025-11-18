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
    public TextMeshProUGUI status;

    private AchievementData data;
    private Action<AchievementData> onInfoClicked;

    public void Init(AchievementData achievement, Action<AchievementData> infoCallback)
    {
        data = achievement;
        onInfoClicked = infoCallback;

        titleText.text = achievement.title;
        icon.sprite = achievement.icon;

        infoBtn.onClick.RemoveAllListeners();
        infoBtn.onClick.AddListener(InfoBtnClick);

        bool unlocked = false;
        if (!string.IsNullOrEmpty(achievement.id) && AchievementManager.Instance != null)
        {
            unlocked = AchievementManager.Instance.IsUnlocked(achievement.id);

            AchievementManager.Instance.OnAchievementUnlocked -= OnAchievementUnlockedHandler;
            AchievementManager.Instance.OnAchievementUnlocked += OnAchievementUnlockedHandler;
        }

        achievement.isUnlocked = unlocked;
        completeIcon.gameObject.SetActive(unlocked);
        if (unlocked)
        {
            status.text = "Completed";
            status.color = new Color32(38, 116, 69, 255); // #267445ff (green)
        }
        else
        {
            status.text = "Incomplete";
            status.color = new Color32(176, 49, 49, 255); // #B03131 (dark red)
        }
    }

    private void OnAchievementUnlockedHandler(string id)
    {
        if (data == null) return;
        if (id != data.id) return;

        data.isUnlocked = true;
        completeIcon.gameObject.SetActive(true);
        status.text = "Completed";
    }

    private void InfoBtnClick()
    {
        Debug.Log("[Achievement] Info clicked → " + data.title);
        onInfoClicked?.Invoke(data);
    }

    private void OnDestroy()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementUnlocked -= OnAchievementUnlockedHandler;
    }
}
