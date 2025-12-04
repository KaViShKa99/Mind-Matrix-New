// using UnityEngine;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using UnityEngine.SocialPlatforms;

// public class GooglePlayManager : MonoBehaviour
// {
//     public static GooglePlayManager Instance;

//     private void Awake()
//     {
//         if (Instance == null) {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else {
//             Destroy(gameObject);
//         }
//     }

//     private void Start()
//     {
//         PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
//     }

//     internal void ProcessAuthentication(SignInStatus status)
//     {
//         if (status == SignInStatus.Success)
//         {
//             Debug.Log("GPGS Login Success!");
//         }
//         else
//         {
//             Debug.Log("GPGS Login Failed!");
//         }
//     }

//     // ============================
//     //     ACHIEVEMENT METHODS
//     // ============================

//     /// Unlock an achievement instantly
//     public void UnlockAchievement(string achievementID)
//     {
//         PlayGamesPlatform.Instance.ReportProgress(achievementID, 100f, (bool success) => {
//             Debug.Log("Achievement Unlock: " + success);
//         });
//     }

//     /// Increment an incremental achievement
//     public void IncrementAchievement(string achievementID, int steps)
//     {
//         PlayGamesPlatform.Instance.IncrementAchievement(achievementID, steps, (bool success) => {
//             Debug.Log("Increment Achievement: " + success);
//         });
//     }

//     /// Show the achievements UI
//     public void ShowAchievementsUI()
//     {
//         PlayGamesPlatform.Instance.ShowAchievementsUI();
//     }
// }

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
                    PlayerPrefs.SetInt("Coins", playerData.coins);
                    PlayerPrefs.SetInt("UnlockedLevel", playerData.unlockedLevel);
                    PlayerPrefs.SetInt("SelectedLevel", playerData.currentLevel);

                    PlayerPrefs.SetInt("LevelReward_" + playerData.currentLevel, 1);


                    // PlayerPrefs.SetInt("Lives", playerData.lives);
                    // PlayerPrefs.SetString("LastLifeUnix", playerData.lastLifeUnix.ToString());


                    PlayerPrefs.Save();
                    Debug.Log("Loaded player data from cloud "+playerData);
                    OnCloudDataLoaded?.Invoke(playerData);
                }
                else
                {
                    playerData.coins = PlayerPrefs.GetInt("Coins", 0);
                    playerData.currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
                    playerData.unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
                    playerData.levelReward = PlayerPrefs.GetInt("LevelReward_" + playerData.currentLevel, 0);

                    // playerData.lives = PlayerPrefs.GetInt("Lives", 5);
                    // playerData.lastLifeUnix = PlayerPrefs.HasKey("LastLifeUnix") ? long.Parse(PlayerPrefs.GetString("LastLifeUnix")) : 0;


                    Debug.Log("No cloud save found, using default player data" +PlayerPrefs.GetInt("Coins", 0));
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


    // ============================
    //     CLOUD SAVE METHODS
    // ============================

    public void SaveGame(PlayerData data)
    {

        if (PlayGamesPlatform.Instance == null)
        {
            Debug.LogWarning("PlayGamesPlatform not initialized!");
            return;
        }

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (savedGameClient == null)
        {
            Debug.LogWarning("SavedGameClient not ready yet!");
            return;
        }

        string json = JsonUtility.ToJson(data);
        Debug.Log("JSON saving: " + json);

        byte[] savedData = Encoding.UTF8.GetBytes(json);

        // ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

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

                    savedGameClient.CommitUpdate(game, update, savedData,
                    (commitStatus, savedGame) =>
                    {
                        Debug.Log("Game Saved Result: " + commitStatus +
                                " | Coins Saved: " + data.coins);
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
                            Debug.Log("Loaded JSON: " + json);

                            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
                            onLoaded?.Invoke(loadedData);
                        }
                        else
                        {
                            Debug.LogError("Failed to read save: " + readStatus);
                            onLoaded?.Invoke(null);
                        }
                    });
                }
                else
                {
                    Debug.LogError("Failed to open save: " + status);
                    onLoaded?.Invoke(null);
                }
            });
    }

    // ============================
    //     DELETE CLOUD SAVE (FOR TESTING)
    // ============================

    public void DeleteCloudSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        if (PlayGamesPlatform.Instance == null)
        {
            Debug.LogError("PlayGamesPlatform not initialized!");
            return;
        }

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        if (savedGameClient == null)
        {
            Debug.LogError("SavedGame client not ready yet. Make sure user is logged in.");
            return;
        }

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
