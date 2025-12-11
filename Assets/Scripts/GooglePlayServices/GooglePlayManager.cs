using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using System;
using System.Text;

public class GooglePlayManager : MonoBehaviour
{
    public static GooglePlayManager Instance;
    public PlayerData playerData;
    public event Action<PlayerData> OnCloudDataLoaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            playerData = new PlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    private void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            LoadGame((loadedData) =>
            {
                if (loadedData != null)
                {
                    playerData = loadedData;
                    SyncCloudToPlayerPrefs();
                    OnCloudDataLoaded?.Invoke(playerData);
                }
                else
                {
                    LoadPlayerPrefsToPlayerData();
                    SaveGame(playerData);
                    OnCloudDataLoaded?.Invoke(playerData);
                }
            });
            Debug.Log("GPGS Login Success!");
        }
        else
        {
            Debug.LogError("GPGS Login Failed: " + status);
        }
    }
    // ============================
    //     ACHIEVEMENT METHODS
    // ============================

    public void UnlockAchievement(string achievementID)
    {
        PlayGamesPlatform.Instance.ReportProgress(achievementID, 100f, (bool success) =>
        {
            Debug.Log("Achievement Unlock: " + success);
        });
    }

    public void IncrementAchievement(string achievementID, int steps)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(achievementID, steps, (bool success) =>
        {
            Debug.Log("Increment Achievement: " + success);
        });
    }


    public void ShowAchievementsUI()
    {
        if (!Social.localUser.authenticated)
        {
            Debug.Log("Not authenticated, retrying login...");
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
            return;
        }

        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    // ===============================
    // ===== CLOUD SAVE METHODS ======
    // ===============================

    public void SaveGame(PlayerData data)
    {
        if (PlayGamesPlatform.Instance == null) return;
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        if (savedGameClient == null) return;

        string json = JsonUtility.ToJson(data);
        byte[] savedData = Encoding.UTF8.GetBytes(json);

        savedGameClient.OpenWithAutomaticConflictResolution(
            "player_data",
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    var update = new SavedGameMetadataUpdate.Builder()
                        .WithUpdatedDescription("Saved at " + DateTime.Now)
                        .Build();

                    savedGameClient.CommitUpdate(game, update, savedData, (commitStatus, savedGame) =>
                    {
                        Debug.Log("Game Saved Result: " + commitStatus + " | Coins Saved: " + data.coins);
                    });
                }
                else
                {
                    Debug.LogError("Failed to open save: " + status);
                }
            });
    }

    public void LoadGame(Action<PlayerData> onLoaded)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            "player_data",
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    savedGameClient.ReadBinaryData(game, (readStatus, data) =>
                    {
                        if (readStatus == SavedGameRequestStatus.Success)
                        {
                            string json = Encoding.UTF8.GetString(data);
                            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
                            onLoaded?.Invoke(loadedData);
                        }
                        else
                        {
                            onLoaded?.Invoke(null);
                        }
                    });
                }
                else
                {
                    onLoaded?.Invoke(null);
                }
            });
    }

    // ===============================
    // ===== SYNC CLOUD & LOCAL ======
    // ===============================

    private void SyncCloudToPlayerPrefs()
    {
        PlayerPrefs.SetInt("Coins", playerData.coins);
        PlayerPrefs.SetInt("UnlockedLevel", playerData.unlockedLevel);
        PlayerPrefs.SetInt("SelectedLevel", playerData.currentLevel);
        PlayerPrefs.SetInt("LevelReward_" + playerData.currentLevel, playerData.levelReward);

        // Sync stars
        foreach (var starData in playerData.starRatings)
        {
            PlayerPrefs.SetInt("Stars_Level_" + starData.levelIndex, starData.stars);
        }
        PlayerPrefs.Save();
    }

    private void LoadPlayerPrefsToPlayerData()
    {
        playerData.coins = PlayerPrefs.GetInt("Coins", 0);
        playerData.currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        playerData.unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        playerData.levelReward = PlayerPrefs.GetInt("LevelReward_" + playerData.currentLevel, 0);

        playerData.starRatings.Clear();
        for (int i = 1; i <= 200; i++)
        {
            if (PlayerPrefs.HasKey("Stars_Level_" + i))
            {
                int s = PlayerPrefs.GetInt("Stars_Level_" + i);
                playerData.starRatings.Add(new LevelStarData(i, s));
            }
        }
    }

    // ===============================
    // ===== DELETE CLOUD SAVE =======
    // ===============================

    public void DeleteCloudSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        if (savedGameClient == null) return;

        savedGameClient.OpenWithAutomaticConflictResolution(
            "player_data",
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    savedGameClient.Delete(game);
                    Debug.Log("Cloud save deleted!");
                }
                else
                {
                    Debug.LogError("Failed to open save for deletion: " + status);
                }
            });
    }
}
