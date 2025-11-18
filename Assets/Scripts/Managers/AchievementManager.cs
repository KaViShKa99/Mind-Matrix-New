using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public event Action<string> OnAchievementUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ------------------------------
    // Achievement definitions
    // ------------------------------
    private Dictionary<string, bool> achievements = new Dictionary<string, bool>();

    private void Start()
    {
        LoadAchievements();
    }

    // ------------------------------
    // Check puzzle achievements
    // ------------------------------
    public void CheckPuzzleAchievements(LevelDetailsManager levelDetails)
    {

        if (levelDetails == null) return;

        int size = levelDetails.GetLevelSize();
        float startTime = levelDetails.GetStartTime();
        float time = levelDetails.GetTimerCount() - startTime;
        float elapsedTime = levelDetails.GetTimerCount();
        int movesUsed = levelDetails.GetMoveCount();
        int level = levelDetails.GetLevel();
        // bool usedHint = levelDetails.HasUsedHint(); // implement this in LevelDetailsManager

        Debug.Log("Time: " + elapsedTime+ " / Moves Used: " + movesUsed);

        // ----- Achievement checks -----
        TryUnlock("first_solve", level == 1);
        TryUnlock("comp_10_puzzle", level == 10);
        TryUnlock("comp_20_puzzle", level == 20);
        TryUnlock("comp_30_puzzle", level == 30);
        TryUnlock("comp_40_puzzle", level == 40);
        TryUnlock("comp_50_puzzle", level == 50);
        TryUnlock("3by3_under_15", size == 3 && time <= 15f);
        TryUnlock("4by4_under_30", size == 4 && time <= 30f);
        TryUnlock("comp_5by5_under_45", size == 5 && time <= 45f);

        if (time <= 15f)
        {
            int under15Sec = IncrementCounter("count_under_15_seconds");
            TryUnlock("comp_10_puzz_under_15_times", under15Sec >= 10);
            TryUnlock("comp_40_puzz_under_15_times", under15Sec >= 40);
        }

        if (movesUsed <= 15)
        {
            int under15Moves = IncrementCounter("count_under_15_moves");
            TryUnlock("comp_10_under_15_moves", under15Moves >= 10);
            
        }
        if (size == 4 && movesUsed <= 15)
        {
            int under15MovesSize4 = IncrementCounter("count_under_15_moves_size4");
            TryUnlock("comp_4by4_under_15_moves", under15MovesSize4 == 1);
            TryUnlock("comp_4by4_10_under15_moves", under15MovesSize4 == 10);       
        }
        // if (size == 4 && movesUsed <= 15)
        // {
        //     int under15MovesSize4 = IncrementCounter("count_under_15_moves_size4");
        //     TryUnlock("comp_4by4_under_15_moves", under15MovesSize4 == 1);
        //     TryUnlock("comp_4by4_10_under15_moves", under15MovesSize4 == 10);       
        // }
        if (size == 5 && movesUsed <= 20)
        {
            int under20Moves = IncrementCounter("count_under_20_moves_size5");
            TryUnlock("comp_5by5_under_20_moves", under20Moves == 1);
        }

        TryUnlock("reach_5by5_stage", level >= 29);
        TryUnlock("comp_58_puzzles", level >= 57);

        
       
    }

    // ------------------------------
    // Unlock achievement
    // ------------------------------
    private void TryUnlock(string id, bool condition)
    {
        if (!condition) return;

        if (achievements.ContainsKey(id) && achievements[id])
            return; // already unlocked

        achievements[id] = true;
        PlayerPrefs.SetInt(id, 1); // save locally
        PlayerPrefs.Save();

        Debug.Log("Achievement unlocked: " + id);

        OnAchievementUnlocked?.Invoke(id);
    }

    // ------------------------------
    // Load achievements from PlayerPrefs
    // ------------------------------
    public void LoadAchievements()
    {
        string[] ids = new string[]
        {
            "first_solve",
            "comp_10_puzzle",
            "comp_20_puzzle",
            "comp_30_puzzle",
            "comp_40_puzzle",
            "comp_50_puzzle",
            "3by3_under_15",    
            "4by4_under_30",
            "comp_5by5_under_45",
            "comp_10_puzz_under_15_times",
            "comp_40_puzz_under_30_times",
            "comp_4by4_under_15_moves",
            "comp_5by5_under_20_moves",
            "comp_4by4_10_under15_moves",
            "reach_5by5_stage",
            "comp_58_puzzles"

        };

        achievements.Clear();

        foreach (string id in ids)
        {
            Debug.Log("load achievement: " + id);
            Debug.Log("unlocked achievement: " + (PlayerPrefs.GetInt(id, 0) == 1));
            achievements[id] = PlayerPrefs.GetInt(id, 0) == 1;
        }
    }

    // ------------------------------
    // Optional: check if achievement unlocked
    // ------------------------------
    public bool IsUnlocked(string id)
    {
        return achievements.ContainsKey(id) && achievements[id];
    }

    private int IncrementCounter(string counterKey)
    {
        int value = PlayerPrefs.GetInt(counterKey, 0) + 1;
        PlayerPrefs.SetInt(counterKey, value);
        PlayerPrefs.Save();
        Debug.Log($"Counter {counterKey} => {value}");
        return value;
    }

    public int GetCounter(string counterKey)
    {
        return PlayerPrefs.GetInt(counterKey, 0);
    }
}
