// using UnityEngine;
// using System;

// public class StarRatingManager : MonoBehaviour
// {
//     public static StarRatingManager Instance;

//     public event Action<int, int> OnStarUpdated; 
//     // (levelIndex, newStarCount)

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     // ===================================================
//     //  GET STAR COUNT FOR LEVEL
//     // ===================================================
//     public int GetStars(int levelIndex)
//     {
//         return PlayerPrefs.GetInt("Stars_Level_" + levelIndex, 0);
//     }

//     // ===================================================
//     //  SAVE STAR RATING
//     // ===================================================
//     public void SetStars(int levelIndex, int stars)
//     {
//         int currentStars = GetStars(levelIndex);

//         // Only update if higher rating earned
//         if (stars > currentStars)
//         {
//             PlayerPrefs.SetInt("Stars_Level_" + levelIndex, stars);
//             PlayerPrefs.Save();

//             Debug.Log("Updated Level " + levelIndex + " stars: " + stars);

//             OnStarUpdated?.Invoke(levelIndex, stars);
//         }
//         else
//         {
//             Debug.Log("Old star rating is higher → Not updating.");
//         }
//     }

//     // ===================================================
//     //  CALL THIS AFTER COMPLETING A LEVEL
//     // ===================================================
//     public void RewardStars()
//     {
//         int starCount = 1;
//         int levelIndex = LevelDetailsManager.Instance.GetLevel();
//         float completionTime = LevelDetailsManager.Instance.GetUsedTime();
//         int movesUsed = LevelDetailsManager.Instance.GetUsedMoves();

//         // ⭐⭐ Example rules (you can modify)
//         if (completionTime <= 10f && movesUsed <= 10) starCount = 3;     // 3 stars if under 2 minutes and 20 moves
//         else if (completionTime <= 20f && movesUsed <= 20) starCount = 2; // 2 stars if under 3 minutes and 30 moves

//         SetStars(levelIndex, starCount);
//     }

//     public bool HasLevelStarRewarded(int levelIndex)
//     {
//         return PlayerPrefs.GetInt("Stars_Level_" + levelIndex, 0) == 1;
//     }
// }

using UnityEngine;
using System;

public class StarRatingManager : MonoBehaviour
{
    public static StarRatingManager Instance;
    public event Action<int, int> OnStarUpdated;

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

    public int GetStars(int levelIndex)
    {
        return PlayerPrefs.GetInt("Stars_Level_" + levelIndex, 0);
    }

    public void SetStars(int levelIndex, int stars)
    {
        int currentStars = GetStars(levelIndex);

        if (stars > currentStars)
        {
            // Save locally
            PlayerPrefs.SetInt("Stars_Level_" + levelIndex, stars);
            PlayerPrefs.Save();

            // Save to cloud
            SaveStarsToCloud(levelIndex, stars);

            OnStarUpdated?.Invoke(levelIndex, stars);
            Debug.Log($"Stars Updated: Level {levelIndex} → {stars} Stars");
        }
        else
        {
            Debug.Log("Old star rating is higher → Not updating.");
        }
    }

    // private void SaveStarsToCloud(int levelIndex, int stars)
    // {
        
    //     if (GooglePlayManager.Instance == null || GooglePlayManager.Instance.playerData == null)
    //         return;

    //     GooglePlayManager.Instance.playerData.SetStar(levelIndex, stars);
    //     GooglePlayManager.Instance.SaveGame(GooglePlayManager.Instance.playerData);
    // }

    private void SaveStarsToCloud(int levelIndex, int stars)
    {
    #if UNITY_ANDROID || UNITY_IOS
        if (GooglePlayManager.Instance == null || GooglePlayManager.Instance.playerData == null)
            return;

        GooglePlayManager.Instance.playerData.SetStar(levelIndex, stars);
        GooglePlayManager.Instance.SaveGame(GooglePlayManager.Instance.playerData);
    #else
        // WebGL or other platforms – do nothing
        Debug.Log("💾 Cloud save skipped on WebGL / non-mobile platform");
    #endif
    }

    public void RewardStars()
    {
        int starCount = 1;
        int levelIndex = LevelDetailsManager.Instance.GetLevel();
        float completionTime = LevelDetailsManager.Instance.GetUsedTime();
        int movesUsed = LevelDetailsManager.Instance.GetUsedMoves();

        // if (completionTime <= 10f && movesUsed <= 10) starCount = 3;
        // else if (completionTime <= 20f && movesUsed <= 20) starCount = 2;

        // 3x3 Puzzle (Levels 1 to 12)
        if (levelIndex >= 1 && levelIndex <= 12)
        {
            if (completionTime <= 25f && movesUsed <= 20) starCount = 3;
            else if (completionTime <= 30f && movesUsed <= 30) starCount = 2;
        }
        // 4x4 Puzzle (Levels 13 to 32)
        else if (levelIndex >= 13 && levelIndex <= 32)
        {
            if (completionTime <= 35 && movesUsed <= 40) starCount = 3;
            else if (completionTime <= 60 && movesUsed <= 45) starCount = 2;
        }
        // 5x5 Puzzle (Levels 33 to 112)
        else if (levelIndex >= 33 && levelIndex <= 112)
        {
            if (completionTime <= 180f && movesUsed <= 200) starCount = 3;
            else if (completionTime <= 200f && movesUsed <= 250) starCount = 2;
        }

        SetStars(levelIndex, starCount);
    }

    public bool HasLevelStarRewarded(int levelIndex)
    {
        return PlayerPrefs.GetInt("Stars_Level_" + levelIndex, 0) == 1;
    }
}
