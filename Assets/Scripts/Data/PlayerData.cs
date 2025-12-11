// using System;

// [Serializable]
// public class PlayerData
// {
//     public int coins;
//     public int currentLevel;
//     public int unlockedLevel;

//     // Lives system
//     public int lives = 5;                      // current lives
//     public long lastLifeUnix = 0;              // unix seconds when timer started / last update

//     public PlayerData() {}   // IMPORTANT for JsonUtility

//     public PlayerData(int coins = 0, int currentLevel = 1, int unlockedLevel = 1, int lives = 5, long lastLifeUnix = 0)
//     {
//         this.coins = coins;
//         this.currentLevel = currentLevel;
//         this.unlockedLevel = unlockedLevel;
//         this.lives = lives;
//         this.lastLifeUnix = lastLifeUnix;
//     }

//     public override string ToString()
//     {
//         return $"coins: {coins}, currentLevel: {currentLevel}, unlockedLevel: {unlockedLevel}, lives: {lives}, lastLifeUnix: {lastLifeUnix}";
//     }
// }

///222222///
/// 
// using System;

// [Serializable]
// public class PlayerData
// {
//     public int coins;
//     public int currentLevel;
//     public int unlockedLevel;
//     public int levelReward;

//     public PlayerData() {}   // IMPORTANT for JsonUtility

//     // Constructor
//     public PlayerData(int coins = 0, int currentLevel = 1, int unlockedLevel = 1, int levelReward = 0)
//     {
//         this.coins = coins;
//         this.currentLevel = currentLevel;
//         this.unlockedLevel = unlockedLevel;
//         this.levelReward = levelReward;
//     }

//     public override string ToString()
//     {
//         return $"coins: {coins}, currentLevel: {currentLevel}, unlockedLevel: {unlockedLevel}, levelReward: {levelReward}";
//     }

// }

using System;
using System.Collections.Generic;

[Serializable]
public class LevelStarData
{
    public int levelIndex;
    public int stars;

    public LevelStarData(int levelIndex, int stars)
    {
        this.levelIndex = levelIndex;
        this.stars = stars;
    }
}

[Serializable]
public class PlayerData
{
    public int coins;
    public int currentLevel;
    public int unlockedLevel;
    public int levelReward;

    // ⭐ Store star ratings for every level
    public List<LevelStarData> starRatings = new List<LevelStarData>();

    public PlayerData() { }

    public PlayerData(
        int coins = 0,
        int currentLevel = 1,
        int unlockedLevel = 1,
        int levelReward = 0
    )
    {
        this.coins = coins;
        this.currentLevel = currentLevel;
        this.unlockedLevel = unlockedLevel;
        this.levelReward = levelReward;
    }

    // --- Set or update star for a level
    public void SetStar(int levelIndex, int stars)
    {
        LevelStarData entry = starRatings.Find(x => x.levelIndex == levelIndex);
        if (entry == null)
            starRatings.Add(new LevelStarData(levelIndex, stars));
        else
            entry.stars = stars;
    }

    // --- Get stars for a level
    public int GetStar(int levelIndex)
    {
        LevelStarData entry = starRatings.Find(x => x.levelIndex == levelIndex);
        return entry == null ? 0 : entry.stars;
    }

    public override string ToString()
    {
        return
            $"coins: {coins}, currentLevel: {currentLevel}, " +
            $"unlockedLevel: {unlockedLevel}, levelReward: {levelReward}, " +
            $"stars saved: {starRatings.Count}";
    }
}
