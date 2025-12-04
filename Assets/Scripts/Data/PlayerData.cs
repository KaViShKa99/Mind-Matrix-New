using System;

[Serializable]
public class PlayerData
{
    public int coins;
    public int currentLevel;
    public int unlockedLevel;
    public int levelReward;

    public PlayerData() {}   // IMPORTANT for JsonUtility

    // Constructor
    public PlayerData(int coins = 0, int currentLevel = 1, int unlockedLevel = 1, int levelReward = 0)
    {
        this.coins = coins;
        this.currentLevel = currentLevel;
        this.unlockedLevel = unlockedLevel;
        this.levelReward = levelReward;
    }

    public override string ToString()
    {
        return $"coins: {coins}, currentLevel: {currentLevel}, unlockedLevel: {unlockedLevel}, levelReward: {levelReward}";
    }

}
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
