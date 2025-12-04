// using UnityEngine;
// using Google.Play.AppUpdate;

// public class UpdateChecker : MonoBehaviour
// {
//     private AppUpdateManager appUpdateManager;

//     void Start()
//     {
//         appUpdateManager = new AppUpdateManager();
//         CheckForUpdate();
//     }

//     void CheckForUpdate()
//     {
//         var appUpdateInfoOp = appUpdateManager.GetAppUpdateInfo();

//         appUpdateInfoOp.Completed += (op) =>
//         {
//             if (!op.IsSuccessful)
//             {
//                 Debug.LogError("Update check failed: " + op.Error);
//                 return;
//             }

//             var info = op.GetResult();

//             if (info.UpdateAvailability == UpdateAvailability.UpdateAvailable &&
//                 info.IsUpdateTypeAllowed(AppUpdateOptions.ImmediateAppUpdateOptions()))
//             {
//                 appUpdateManager.StartUpdate(info, AppUpdateOptions.ImmediateAppUpdateOptions());
//             }
//         };
//     }
// }



// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Google.Play.AppUpdate;
// using Google.Play.Common;

// public class UpdateChecker : MonoBehaviour
// {
//     [Header("Main UI")]
//     public GameObject backgroundPanel;
//     public GameObject updatePanel;
//     public TMP_Text updateTitle;
//     public TMP_Text updateDescription;
//     public Button updateButton;
//     public Button cancelButton;
//     public Button quitButton;

//     [Header("Progress UI")]
//     public GameObject progressSpinner; // A spinning icon or loading indicator

//     private AppUpdateManager appUpdateManager;
//     private AppUpdateInfo appUpdateInfo;

//     void Start()
//     {
//         Debug.Log("UpdateChecker START");

//         backgroundPanel.SetActive(false);
//         updatePanel.SetActive(false);
//         progressSpinner.SetActive(false);

//         appUpdateManager = new AppUpdateManager();
//         CheckForUpdate();
//     }

//     void CheckForUpdate()
//     {
//         Debug.Log("Checking for update…");

//         var request = appUpdateManager.GetAppUpdateInfo();

//         request.Completed += (operation) =>
//         {
//             if (operation.Error != AppUpdateErrorCode.NoError)
//             {
//                 Debug.LogError("Update check failed: " + operation.Error);
//                 return;
//             }

//             appUpdateInfo = operation.GetResult();

//             if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable)
//             {
//                 // bool forced = appUpdateInfo.UpdatePriority >= 5; // Priority >= 5 → forced update
//                 bool forced = appUpdateInfo.UpdatePriority < 5; // Priority >= 5 → flexible update
//                 ShowUpdatePopup(forced);
//             }
//             else
//             {
//                 Debug.Log("No update available.");
//             }
//         };
//     }

//     void ShowUpdatePopup(bool forced)
//     {
//         updatePanel.SetActive(true);
//         backgroundPanel.SetActive(true);

//         // Configure UI based on forced or optional update
//         if (forced)
//         {
//             updateTitle.text = "Update Required";
//             updateDescription.text = "A new version is required to continue.";
//             cancelButton.gameObject.SetActive(false);
//             quitButton.gameObject.SetActive(true);
//         }
//         else
//         {
//             updateTitle.text = "Update Available";
//             updateDescription.text = "A newer version is available.";
//             cancelButton.gameObject.SetActive(true);
//             quitButton.gameObject.SetActive(false);
//         }

//         // Remove previous listeners
//         updateButton.onClick.RemoveAllListeners();
//         cancelButton.onClick.RemoveAllListeners();
//         quitButton.onClick.RemoveAllListeners();

//         // Add new listeners
//         if (forced)
//             updateButton.onClick.AddListener(StartImmediateUpdate);
//         else
//             updateButton.onClick.AddListener(StartFlexibleUpdate);

//         cancelButton.onClick.AddListener(CancelUpdate);
//         quitButton.onClick.AddListener(QuitGame);
//     }

//     void StartImmediateUpdate()
//     {
//         Debug.Log("Starting Immediate Update…");

//         var request = appUpdateManager.StartUpdate(appUpdateInfo, AppUpdateOptions.ImmediateAppUpdateOptions());

//         request.Completed += (operation) =>
//         {
//             if (operation.Error != AppUpdateErrorCode.NoError)
//                 Debug.LogError("Immediate update failed: " + operation.Error);
//             else
//                 Debug.Log("Immediate update started successfully.");
//         };
//     }

//     void StartFlexibleUpdate()
//     {
//         Debug.Log("Starting Flexible Update…");

//         updateButton.gameObject.SetActive(false);
//         cancelButton.gameObject.SetActive(true);

//         updateTitle.text = "Updating...";
//         updateDescription.text = "Downloading update…";

//         progressSpinner.SetActive(true); // show spinner

//         var request = appUpdateManager.StartUpdate(appUpdateInfo, AppUpdateOptions.FlexibleAppUpdateOptions());

//         request.Completed += (operation) =>
//         {
//             if (operation.Error != AppUpdateErrorCode.NoError)
//             {
//                 Debug.LogError("Flexible update failed: " + operation.Error);
//                 progressSpinner.SetActive(false);
//                 return;
//             }

//             Debug.Log("Flexible update download completed. Installing…");
//             updateDescription.text = "Download completed. Installing…";

//             appUpdateManager.CompleteUpdate();
//             progressSpinner.SetActive(false);
//         };
//     }

//     void CancelUpdate()
//     {
//         Debug.Log("Update canceled by user");
//         updatePanel.SetActive(false);
//         backgroundPanel.SetActive(false);
//         progressSpinner.SetActive(false);
//     }

//     void QuitGame()
//     {
//         Debug.Log("Quit button pressed → Exiting game");
//         Application.Quit();
//     }
// }
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
