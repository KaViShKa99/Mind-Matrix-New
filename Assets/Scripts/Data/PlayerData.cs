using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int coins = 0;
    public List<string> achievements = new List<string>();

    public static PlayerData Current;

    public static void Load()
    {
        Current = CrazyGamesSaveSystem.Load();
    }

    public static void Save(PlayerData data)
    {
        CrazyGamesSaveSystem.Save(data);
    }
}
