using UnityEngine;
using CrazyGames;

public static class CrazyGamesAchievementSystem
{
    public static void Unlock(string achievementId, PlayerData data)
    {
        if (data.achievements.Contains(achievementId))
            return; // already unlocked

        data.achievements.Add(achievementId);
        PlayerData.Save(data);

        // Send analytics event
        // CrazySDK.Ad().GameplayEvent("achievement_unlocked_" + achievementId);

        Debug.Log("Achievement unlocked: " + achievementId);

        // Show popup
        // AchievementPopupUI.Instance.ShowPopup(
        //     "Achievement Unlocked",
        //     achievementId,
        //     null
        // );
    }
}
