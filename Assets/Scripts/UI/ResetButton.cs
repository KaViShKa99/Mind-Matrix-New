using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public void OnResetPressed()
    {
        if (GooglePlayManager.Instance != null)
        {
            GooglePlayManager.Instance.DeleteCloudSave();
            Debug.Log("Cloud save deleted!");

        }
        else
        {
            Debug.LogError("GooglePlayManager not found!");
        }
    }
}
