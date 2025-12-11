using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;

public class UpdateChecker : MonoBehaviour
{
    private AppUpdateManager appUpdateManager;

    private void Start()
    {
        CheckForUpdate();
    }

    void CheckForUpdate()
    {
        appUpdateManager = new AppUpdateManager();

        var appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();
        appUpdateInfoOperation.Completed += (operation) =>
        {
            if (operation.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogError("Error: " + operation.Error);
                return;
            }

            var info = operation.GetResult();

            // IMMEDIATE UPDATE (priority >= 5 recommended)

            if (info.UpdateAvailability == UpdateAvailability.UpdateAvailable &&
                info.UpdatePriority >= 5)
            {
                Debug.Log("Immediate update available");
                StartImmediateUpdate(info);
                return;
            }

            // FLEXIBLE UPDATE (normal update)
            // if (info.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            // {
            //     Debug.Log("Flexible update available");
            //     StartFlexibleUpdate(info);
            //     return;
            // }

            Debug.Log("No update available.");
        };
    }

    void StartImmediateUpdate(AppUpdateInfo info)
    {
        var request = appUpdateManager.StartUpdate(info, AppUpdateOptions.ImmediateAppUpdateOptions());

        request.Completed += (op) =>
        {
            if (op.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogError("Immediate update failed: " + op.Error);
            }
            else
            {
                Debug.Log("Immediate update started.");
            }
        };
    }

    void StartFlexibleUpdate(AppUpdateInfo info)
    {
        var request = appUpdateManager.StartUpdate(info, AppUpdateOptions.FlexibleAppUpdateOptions());

        request.Completed += (op) =>
        {
            if (op.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogError("Flexible update failed: " + op.Error);
                return;
            }

            Debug.Log("Flexible update downloaded. Installing…");
            CompleteFlexibleUpdate();
        };
    }

    void CompleteFlexibleUpdate()
    {
        var completeRequest = appUpdateManager.CompleteUpdate();

        completeRequest.Completed += (op) =>
        {
            if (op.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogError("Install failed: " + op.Error);
            }
            else
            {
                Debug.Log("Update installed successfully.");
            }
        };
    }
}
