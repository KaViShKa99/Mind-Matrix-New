using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    void Awake()
    {
        if (AudioManager.Instance == null)
        {
            // Load the AudioManager prefab from Resources folder
            GameObject audioPrefab = Resources.Load<GameObject>("AudioManager");
            if (audioPrefab != null)
            {
                Instantiate(audioPrefab);
                Debug.Log("🎧 AudioManager auto-loaded by Bootstrapper.");
            }
            else
            {
                Debug.LogWarning("⚠️ AudioManager prefab not found in Resources!");
            }
        }
    }
}
