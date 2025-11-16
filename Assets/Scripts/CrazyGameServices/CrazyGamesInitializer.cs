using UnityEngine;
using CrazyGames;

public class CrazyGamesInitializer : MonoBehaviour
{
    private void Awake()
    {
        CrazySDK.Init(() =>
        {
            Debug.Log("CrazyGames SDK initialized");

            PlayerData.Load();
        });
    }
}
