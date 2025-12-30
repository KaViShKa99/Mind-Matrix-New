using System.Collections;
using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;

public class InAppUpdateManager : MonoBehaviour
{
    [SerializeField] private AppUpdateType updateType = AppUpdateType.Flexible;
    private AppUpdateManager appUpdateManager;

    private void Start()
    {
        appUpdateManager = new AppUpdateManager();
        StartCoroutine(CheckForUpdate());
    }

    private IEnumerator CheckForUpdate()
    {
        var appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var info = appUpdateInfoOperation.GetResult();

            if (info.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                AppUpdateOptions updateOptions = (updateType == AppUpdateType.Immediate) 
                    ? AppUpdateOptions.ImmediateAppUpdateOptions() 
                    : AppUpdateOptions.FlexibleAppUpdateOptions();

                if (info.IsUpdateTypeAllowed(updateOptions))
                {
                    // This triggers the Google Play default UI/Dialog
                    var startUpdateRequest = appUpdateManager.StartUpdate(info, updateOptions);
                    yield return startUpdateRequest;

                    // If Flexible, we must still call CompleteUpdate to finish the install
                    if (updateType == AppUpdateType.Flexible && startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        // Note: For Flexible, Google recommends showing a simple "Restart" button 
                        // once downloaded. If you want it fully automatic:
                        appUpdateManager.CompleteUpdate();
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        // Essential for Immediate updates: resumes if the user leaves and returns to the app
        if (updateType == AppUpdateType.Immediate) 
        {
            StartCoroutine(ResumeImmediateUpdate());
        }
    }

    private IEnumerator ResumeImmediateUpdate()
    {
        var op = appUpdateManager.GetAppUpdateInfo();
        yield return op;

        if (op.IsSuccessful && op.GetResult().UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
        {
            appUpdateManager.StartUpdate(op.GetResult(), AppUpdateOptions.ImmediateAppUpdateOptions());
        }
    }
}
// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Google.Play.AppUpdate;
// using Google.Play.Common;

// public class InAppUpdateManager : MonoBehaviour
// {
//     [Header("Update Settings")]
//     [SerializeField] private AppUpdateType updateType = AppUpdateType.Flexible;

//     [Header("UI References")]
//     [SerializeField] private GameObject updatePanel;     
//     [SerializeField] private Slider progressBar;          
//     [SerializeField] private TMP_Text statusText;             
//     [SerializeField] private Button actionButton;         
//     [SerializeField] private TMP_Text buttonText;

//     private AppUpdateManager appUpdateManager;

//     private void Start()
//     {
//         appUpdateManager = new AppUpdateManager();
        
//         if (updatePanel != null) updatePanel.SetActive(false); 
//         if (actionButton != null) actionButton.gameObject.SetActive(false);
        
//         StartCoroutine(CheckForUpdate());
//     }

//     private IEnumerator CheckForUpdate()
//     {
//         var appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();
//         yield return appUpdateInfoOperation;

//         if (appUpdateInfoOperation.IsSuccessful)
//         {
//             var info = appUpdateInfoOperation.GetResult();

//             // Check if an update is available
//             if (info.UpdateAvailability == UpdateAvailability.UpdateAvailable)
//             {
//                 AppUpdateOptions updateOptions = (updateType == AppUpdateType.Immediate) 
//                     ? AppUpdateOptions.ImmediateAppUpdateOptions() 
//                     : AppUpdateOptions.FlexibleAppUpdateOptions();

//                 if (info.IsUpdateTypeAllowed(updateOptions))
//                 {
//                     var startUpdateRequest = appUpdateManager.StartUpdate(info, updateOptions);

//                     if (updateType == AppUpdateType.Flexible)
//                     {
//                         yield return StartCoroutine(MonitorFlexibleUpdate(startUpdateRequest));
//                     }
//                     else
//                     {
//                         // Immediate update: Play Store takes over the UI
//                         yield return startUpdateRequest;
//                     }
//                 }
//             }
//         }
//     }

//     private IEnumerator MonitorFlexibleUpdate(AppUpdateRequest updateOp)
//     {
//         if (updatePanel != null) updatePanel.SetActive(true);
        
//         while (!updateOp.IsDone)
//         {
//             if (updateOp.Status == AppUpdateStatus.Downloading)
//             {
//                 // Calculate progress manually using bytes
//                 if (updateOp.TotalBytesToDownload > 0)
//                 {
//                     float progress = (float)updateOp.BytesDownloaded / updateOp.TotalBytesToDownload;
                    
//                     if (progressBar != null) progressBar.value = progress;
//                     if (statusText != null) statusText.text = $"Downloading: {(progress * 100):F0}%";
//                 }
//             }
//             else if (updateOp.Status == AppUpdateStatus.Pending)
//             {
//                 if (statusText != null) statusText.text = "Preparing download...";
//             }

//             yield return null;
//         }

//         // Handle completion
//         if (updateOp.Status == AppUpdateStatus.Downloaded)
//         {
//             if (statusText != null) statusText.text = "Download Complete!";
            
//             if (actionButton != null)
//             {
//                 actionButton.gameObject.SetActive(true);
//                 buttonText.text = "Restart App";
//                 actionButton.onClick.RemoveAllListeners();
//                 // This triggers the installation and reboots the app
//                 actionButton.onClick.AddListener(() => appUpdateManager.CompleteUpdate());
//             }
//         }
//         else if (updateOp.Status == AppUpdateStatus.Failed || updateOp.Status == AppUpdateStatus.Canceled)
//         {
//             if (updatePanel != null) updatePanel.SetActive(false);
//             Debug.LogError("In-App Update failed or was canceled by the user.");
//         }
//     }

//     private void OnEnable()
//     {
//         // Re-check for an ongoing immediate update if the user backgrounds the app
//         if (updateType == AppUpdateType.Immediate) 
//         {
//             StartCoroutine(ResumeImmediateUpdate());
//         }
//     }

//     private IEnumerator ResumeImmediateUpdate()
//     {
//         var op = appUpdateManager.GetAppUpdateInfo();
//         yield return op;

//         if (op.IsSuccessful)
//         {
//             var result = op.GetResult();
//             if (result.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
//             {
//                 appUpdateManager.StartUpdate(result, AppUpdateOptions.ImmediateAppUpdateOptions());
//             }
//         }
//     }
// }