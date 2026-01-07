// using System.Collections;
// using UnityEngine;
// using Google.Play.AppUpdate;
// using Google.Play.Common;

// public class InAppUpdateManager : MonoBehaviour
// {
//     [SerializeField] private AppUpdateType updateType = AppUpdateType.Flexible;
//     private AppUpdateManager appUpdateManager;

//     private void Start()
//     {
//         appUpdateManager = new AppUpdateManager();
//         StartCoroutine(CheckForUpdate());
//     }

//     private IEnumerator CheckForUpdate()
//     {
//         var appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();
//         yield return appUpdateInfoOperation;

//         if (appUpdateInfoOperation.IsSuccessful)
//         {
//             var info = appUpdateInfoOperation.GetResult();

//             if (info.UpdateAvailability == UpdateAvailability.UpdateAvailable)
//             {
//                 AppUpdateOptions updateOptions = (updateType == AppUpdateType.Immediate) 
//                     ? AppUpdateOptions.ImmediateAppUpdateOptions() 
//                     : AppUpdateOptions.FlexibleAppUpdateOptions();

//                 if (info.IsUpdateTypeAllowed(updateOptions))
//                 {
//                     // This triggers the Google Play default UI/Dialog
//                     var startUpdateRequest = appUpdateManager.StartUpdate(info, updateOptions);
//                     yield return startUpdateRequest;

//                     // If Flexible, we must still call CompleteUpdate to finish the install
//                     if (updateType == AppUpdateType.Flexible && startUpdateRequest.Status == AppUpdateStatus.Downloaded)
//                     {
//                         // Note: For Flexible, Google recommends showing a simple "Restart" button 
//                         // once downloaded. If you want it fully automatic:
//                         appUpdateManager.CompleteUpdate();
//                     }
//                 }
//             }
//         }
//     }

//     private void OnEnable()
//     {
//         // Essential for Immediate updates: resumes if the user leaves and returns to the app
//         if (updateType == AppUpdateType.Immediate) 
//         {
//             StartCoroutine(ResumeImmediateUpdate());
//         }
//     }

//     private IEnumerator ResumeImmediateUpdate()
//     {
//         var op = appUpdateManager.GetAppUpdateInfo();
//         yield return op;

//         if (op.IsSuccessful && op.GetResult().UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
//         {
//             appUpdateManager.StartUpdate(op.GetResult(), AppUpdateOptions.ImmediateAppUpdateOptions());
//         }
//     }
// }
using System.Collections;
using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;

public class InAppUpdateManager : MonoBehaviour
{
    [SerializeField] private AppUpdateType updateType = AppUpdateType.Flexible;
    private AppUpdateManager appUpdateManager;

    private void Awake()
    {
        // FIX 1: Initialize here. Awake runs before OnEnable and Start.
        appUpdateManager = new AppUpdateManager();
    }

    private void Start()
    {
        StartCoroutine(CheckForUpdate());
    }

    private void OnEnable()
    {
        if (updateType == AppUpdateType.Immediate) 
        {
            StartCoroutine(ResumeImmediateUpdate());
        }
    }

    private IEnumerator ResumeImmediateUpdate()
    {
        // FIX 2: Safety check to prevent NullReferenceException
        if (appUpdateManager == null) yield break;

        var op = appUpdateManager.GetAppUpdateInfo();
        yield return op;

        if (op.IsSuccessful)
        {
            var result = op.GetResult();
            if (result.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
            {
                appUpdateManager.StartUpdate(result, AppUpdateOptions.ImmediateAppUpdateOptions());
            }
        }
    }

    private IEnumerator CheckForUpdate()
    {
        // Safety check
        if (appUpdateManager == null) yield break;

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
                    yield return appUpdateManager.StartUpdate(info, updateOptions);
                }
            }
        }
    }
}