using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public void OnResetPressed()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (GooglePlayManager.Instance != null)
        {
            GooglePlayManager.Instance.DeleteCloudSave();
            Debug.Log("Cloud save deleted!");

        }
        else
        {
            Debug.LogError("GooglePlayManager not found!");
        }
#endif
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs reset!");
    }
}
