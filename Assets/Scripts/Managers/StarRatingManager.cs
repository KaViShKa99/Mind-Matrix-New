using UnityEngine;
using System;

public class StarRatingManager : MonoBehaviour
{
    public static StarRatingManager Instance;

    public event Action<int, int> OnStarUpdated; 
    // (levelIndex, newStarCount)

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

    // ===================================================
    //  GET STAR COUNT FOR LEVEL
    // ===================================================
    public int GetStars(int levelIndex)
    {
        return PlayerPrefs.GetInt("Stars_Level_" + levelIndex, 0);
    }

    // ===================================================
    //  SAVE STAR RATING
    // ===================================================
    public void SetStars(int levelIndex, int stars)
    {
        int currentStars = GetStars(levelIndex);

        // Only update if higher rating earned
        // if (stars > currentStars)
        // {
            PlayerPrefs.SetInt("Stars_Level_" + levelIndex, stars);
            PlayerPrefs.Save();

            Debug.Log("Updated Level " + levelIndex + " stars: " + stars);

            OnStarUpdated?.Invoke(levelIndex, stars);
        // }
        // else
        // {
        //     Debug.Log("Old star rating is higher → Not updating.");
        // }
    }

    // ===================================================
    //  CALL THIS AFTER COMPLETING A LEVEL
    // ===================================================
    public void RewardStars()
    {
        int starCount = 1;
        int levelIndex = LevelDetailsManager.Instance.GetLevel();
        float completionTime = LevelDetailsManager.Instance.GetUsedTime();
        int movesUsed = LevelDetailsManager.Instance.GetUsedMoves();

        // ⭐⭐ Example rules (you can modify)
        if (completionTime <= 10f && movesUsed <= 10) starCount = 3;     // 3 stars if under 2 minutes and 20 moves
        else if (completionTime <= 20f && movesUsed <= 20) starCount = 2; // 2 stars if under 3 minutes and 30 moves

        SetStars(levelIndex, starCount);
    }

    public bool HasLevelStarRewarded(int levelIndex)
    {
        return PlayerPrefs.GetInt("Stars_Level_" + levelIndex, 0) == 1;
    }
}
