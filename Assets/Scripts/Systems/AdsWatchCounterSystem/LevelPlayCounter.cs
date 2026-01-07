using UnityEngine;

public class LevelPlayCounter : MonoBehaviour
{
    public static LevelPlayCounter Instance;

    private const string KEY_PLAY_COUNT = "level_play_count";
    private const string KEY_RESET_COUNT = "reset_count";
    private const string KEY_LEVEL_COMPLETE_COUNT = "level_complete_count";

    private int playCount = 0;
    private int resetCount = 0;
    private int levelCompleteCount = 0;

    public bool skipNextStartCount = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playCount = PlayerPrefs.GetInt(KEY_PLAY_COUNT, 0);
        resetCount = PlayerPrefs.GetInt(KEY_RESET_COUNT, 0);
        levelCompleteCount = PlayerPrefs.GetInt(KEY_LEVEL_COMPLETE_COUNT, 0);
    }

    // ---------------------------------------------------------------
    // 🚀 Call this when: LEVEL STARTS or LEVEL RESTART PRESSED
    // ---------------------------------------------------------------
    public void OnLevelPlayed()
    {
        playCount++;
        PlayerPrefs.SetInt(KEY_PLAY_COUNT, playCount);

        Debug.Log("🎮 Play Count: " + playCount);

        if (playCount >= 3)
        {
            ForceInterstitialAd();
        }
    }
    public void OnLevelReset()
    {
        resetCount++;
        PlayerPrefs.SetInt(KEY_RESET_COUNT, resetCount);

        Debug.Log("🔄 Reset Count: " + resetCount);

        if (resetCount >= 3)
        {
            ForceRewardAd();
        }
    }
    public void OnLevelCompleted()
    {
        levelCompleteCount++;
        PlayerPrefs.SetInt(KEY_LEVEL_COMPLETE_COUNT, levelCompleteCount);

        Debug.Log("🏆 Level Complete Count: " + levelCompleteCount);

        if (levelCompleteCount >= 5)
        {
            ForceRewardAd();
        }
    }

    // ---------------------------------------------------------------
    // 🎁 Force Rewarded Ad after 3 plays
    // ---------------------------------------------------------------
    private void ForceRewardAd()
    {
        Debug.Log("⚠️ Must watch rewarded ad!");

        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowRewarded(() =>
            {
                OnRewardWatched();

            });
        }
        else
        {
            Debug.LogWarning("⚠️ AdsManager not found — showing hint directly.");
                OnRewardWatched();
        }
    }

    // ---------------------------------------------------------------
    // 🔁 After watching rewarded ad → Reset counter
    // ---------------------------------------------------------------
    private void OnRewardWatched()
    {
        Debug.Log("🎉 Reward watched — counter reset!");

        // playCount = 0;
        // PlayerPrefs.SetInt(KEY_PLAY_COUNT, 0);
        resetCount = 0;
        PlayerPrefs.SetInt(KEY_RESET_COUNT, 0);
        levelCompleteCount = 0;
        PlayerPrefs.SetInt(KEY_LEVEL_COMPLETE_COUNT, 0);
    }

    // ---------------------------------------------------------------
    // 🎁 Force Interstitial Ad after 4 plays
    // ---------------------------------------------------------------
    private void ForceInterstitialAd()
    {
        Debug.Log("⚠️ Must watch interstitial ad!");

        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowInterstitial(() =>
            {
                OnInterstitialWatched();

            });
        }
        else
        {
            Debug.LogWarning("⚠️ AdsManager not found — showing hint directly.");
                OnInterstitialWatched();
        }
    }

    // ---------------------------------------------------------------
    // 🔁 After watching interstitial ad → Reset counter
    // ---------------------------------------------------------------
    private void OnInterstitialWatched()
    {
        Debug.Log("🎉 Interstital watched — counter reset!");
        playCount = 0;
        PlayerPrefs.SetInt(KEY_PLAY_COUNT, 0);
    
    }
    
}
