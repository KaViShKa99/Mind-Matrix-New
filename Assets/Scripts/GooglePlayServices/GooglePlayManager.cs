using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayManager : MonoBehaviour
{
    public static GooglePlayManager Instance;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log("GPGS Login Success!");
        }
        else
        {
            Debug.Log("GPGS Login Failed!");
        }
    }

    // ============================
    //     ACHIEVEMENT METHODS
    // ============================

    /// Unlock an achievement instantly
    public void UnlockAchievement(string achievementID)
    {
        PlayGamesPlatform.Instance.ReportProgress(achievementID, 100f, (bool success) => {
            Debug.Log("Achievement Unlock: " + success);
        });
    }

    /// Increment an incremental achievement
    public void IncrementAchievement(string achievementID, int steps)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(achievementID, steps, (bool success) => {
            Debug.Log("Increment Achievement: " + success);
        });
    }

    /// Show the achievements UI
    public void ShowAchievementsUI()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }
}
// using UnityEngine;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using UnityEngine.SocialPlatforms;

// public class GooglePlayManager : MonoBehaviour
// {
//     public static GooglePlayManager Instance;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);

//             // Initialize Google Play Games (v2)
//             InitializeGPGS();
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     private void InitializeGPGS()
//     {
//         // Create configuration (add optional features if needed)
//         PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
//             .RequestServerAuthCode(false) // Example: request auth code if needed
//             .RequestEmail()
//             .Build();

//         PlayGamesPlatform.InitializeInstance(config);
//         PlayGamesPlatform.Activate();

//         // Authenticate the user
//         Social.localUser.Authenticate(success =>
//         {
//             if (success)
//             {
//                 Debug.Log("GPGS Login Success!");
//             }
//             else
//             {
//                 Debug.Log("GPGS Login Failed!");
//             }
//         });
//     }

//     // ============================
//     //     ACHIEVEMENT METHODS
//     // ============================

//     /// Unlock an achievement instantly
//     public void UnlockAchievement(string achievementID)
//     {
//         Social.ReportProgress(achievementID, 100f, success =>
//         {
//             Debug.Log("Achievement Unlock: " + success);
//         });
//     }

//     /// Increment an incremental achievement
//     public void IncrementAchievement(string achievementID, int steps)
//     {
//         PlayGamesPlatform.Instance.IncrementAchievement(achievementID, steps, success =>
//         {
//             Debug.Log("Increment Achievement: " + success);
//         });
//     }

//     /// Show the achievements UI
//     public void ShowAchievementsUI()
//     {
//         Social.ShowAchievementsUI();
//     }
// }
