using System;

[Serializable]
public class PlayerData
{
    public int coins;
    public int currentLevel;
    public int unlockedLevel;

    public PlayerData() {}   // IMPORTANT for JsonUtility

    // Constructor
    public PlayerData(int coins = 0, int currentLevel = 1, int unlockedLevel = 1)
    {
        this.coins = coins;
        this.currentLevel = currentLevel;
        this.unlockedLevel = unlockedLevel;
    }

    public override string ToString()
    {
        return $"coins: {coins}, currentLevel: {currentLevel}, unlockedLevel: {unlockedLevel}";
    }

}
