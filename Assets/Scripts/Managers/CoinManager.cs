using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int coins;
    public int gems;

    public event Action<int> OnCoinsChanged;
    public event Action<int> OnGemsChanged;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
#if UNITY_ANDROID || UNITY_IOS
            // Subscribe safely to cloud event                      
            if (GooglePlayManager.Instance != null)
            {
                GooglePlayManager.Instance.OnCloudDataLoaded += OnCloudDataLoaded;
            }
#endif
            LoadData();
            OnCoinsChanged?.Invoke(coins);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =======================
    //   COINS
    // =======================

    public int GetCoins() => coins;

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveData();
        OnCoinsChanged?.Invoke(coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            SaveData();
            OnCoinsChanged?.Invoke(coins);
            return true;
        }

        Debug.Log("Not enough coins!");
        return false;
    }

    // =======================
    //   GEMS
    // =======================

    public int GetGems() => gems;

    public void AddGems(int amount)
    {
        gems += amount;
        SaveData();
        OnGemsChanged?.Invoke(gems);
    }

    public bool SpendGems(int amount)
    {
        if (gems >= amount)
        {
            gems -= amount;
            SaveData();
            OnGemsChanged?.Invoke(gems);
            return true;
        }

        Debug.Log("Not enough gems!");
        return false;
    }

    // =======================
    //   FIRST-TIME LEVEL REWARD
    // =======================

    public bool HasLevelRewarded(int levelIndex)
    {
        return PlayerPrefs.GetInt("LevelReward_" + levelIndex, 0) == 1;
    }

    public void MarkLevelAsRewarded(int levelIndex)
    {
        PlayerPrefs.SetInt("LevelReward_" + levelIndex, 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Rewards coins only if level completed for the FIRST TIME.
    /// </summary>
    public void RewardLevel(int levelIndex, int rewardAmount)
    {
        if (!HasLevelRewarded(levelIndex))
        {
            AddCoins(rewardAmount);
            MarkLevelAsRewarded(levelIndex);

            Debug.Log("First time completing Level " + levelIndex + " → Reward given: " + rewardAmount);
        }
        else
        {
            Debug.Log("Level " + levelIndex + " already rewarded → No more coins.");
        }
    }

    // =======================
    //   SAVE / LOAD
    // =======================

    private void SaveData()
    {
        
#if UNITY_ANDROID || UNITY_IOS

        if (GooglePlayManager.Instance != null && GooglePlayManager.Instance.playerData != null)
        {
            GooglePlayManager.Instance.playerData.coins = coins;
            GooglePlayManager.Instance.SaveGame(GooglePlayManager.Instance.playerData);
        }
#endif
        PlayerPrefs.SetInt("Coins", coins);
        // PlayerPrefs.SetInt("Gems", gems);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        // gems = PlayerPrefs.GetInt("Gems", 0);

    }

    private void OnCloudDataLoaded(PlayerData data)
    {
        coins = data.coins;
        PlayerPrefs.SetInt("LevelReward_" + data.currentLevel, 1);
        // Optionally update PlayerPrefs
        PlayerPrefs.SetInt("Coins", coins);

        OnCoinsChanged?.Invoke(coins);

        Debug.Log("CoinManager updated coins from cloud: " + coins);
    }


}
