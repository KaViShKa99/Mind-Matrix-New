using UnityEngine;
using CrazyGames;

public static class CrazyGamesSaveSystem
{
    private const string LOCAL_KEY = "PLAYER_DATA_JSON";

    public static void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);

        // Save locally
        PlayerPrefs.SetString(LOCAL_KEY, json);
        PlayerPrefs.Save();

        // Save to CrazyGames data module
        if (CrazySDK.IsAvailable)
        {
            CrazySDK.Data.SetString(LOCAL_KEY, json);
        }
    }

    public static PlayerData Load()
    {
        string json = null;

        // Try cloud / CrazyGames data module
        if (CrazySDK.IsAvailable)
        {
            if (CrazySDK.Data.HasKey(LOCAL_KEY))
            {
                json = CrazySDK.Data.GetString(LOCAL_KEY, null);
            }
        }

        // If we got data from cloud / CrazyGames
        if (!string.IsNullOrEmpty(json))
        {
            var cloudData = JsonUtility.FromJson<PlayerData>(json);
            // Sync to local cache
            PlayerPrefs.SetString(LOCAL_KEY, json);
            PlayerPrefs.Save();
            return cloudData;
        }

        // Fallback to PlayerPrefs
        if (PlayerPrefs.HasKey(LOCAL_KEY))
        {
            string local = PlayerPrefs.GetString(LOCAL_KEY);
            return JsonUtility.FromJson<PlayerData>(local);
        }

        // No data => create new
        PlayerData newData = new PlayerData();
        Save(newData);
        return newData;
    }
}
