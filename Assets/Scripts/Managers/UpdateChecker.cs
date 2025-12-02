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

//     private AppUpdateManager appUpdateManager;
//     private AppUpdateInfo appUpdateInfo;

//     void Start()
//     {
//         appUpdateManager = new AppUpdateManager();
//         // backgroundPanel.SetActive(false);
//         // updatePanel.SetActive(false);
//         backgroundPanel.SetActive(true);
//         updatePanel.SetActive(true);
        

//         CheckForUpdate();
//     }

//     // -----------------------------------------------------------------------
//     // CHECK FOR UPDATE
//     // -----------------------------------------------------------------------
//     void CheckForUpdate()
//     {
//         var request = appUpdateManager.GetAppUpdateInfo();

//         request.Completed += (operation) =>
//         {
//             if (operation.Error != AppUpdateErrorCode.NoError)
//             {
//                 Debug.Log("Update check failed: " + operation.Error);
//                 return;
//             }

//             appUpdateInfo = operation.GetResult();

//             if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable)
//             {
//                 bool forced = appUpdateInfo.UpdatePriority >= 5;
//                 ShowUpdatePopup(forced);
//             }
//         };
//     }

//     // -----------------------------------------------------------------------
//     // SHOW POPUP
//     // -----------------------------------------------------------------------
//     void ShowUpdatePopup(bool forced)
//     {
//         updatePanel.SetActive(true);
//         backgroundPanel.SetActive(true);

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

//         updateButton.onClick.RemoveAllListeners();
//         cancelButton.onClick.RemoveAllListeners();
//         quitButton.onClick.RemoveAllListeners();

//         if (forced)
//             updateButton.onClick.AddListener(() => StartImmediateUpdate());
//         else
//             updateButton.onClick.AddListener(() => StartFlexibleUpdate());

//         cancelButton.onClick.AddListener(CancelUpdate);
//         quitButton.onClick.AddListener(QuitGame);
//     }

//     // -----------------------------------------------------------------------
//     // IMMEDIATE UPDATE
//     // -----------------------------------------------------------------------
//     void StartImmediateUpdate()
//     {
//         var request = appUpdateManager.StartUpdate(appUpdateInfo, AppUpdateOptions.ImmediateAppUpdateOptions());

//         request.Completed += (operation) =>
//         {
//             if (operation.Error != AppUpdateErrorCode.NoError)
//                 Debug.Log("Immediate update failed: " + operation.Error);
//         };
//     }

//     // -----------------------------------------------------------------------
//     // FLEXIBLE UPDATE (NO PROGRESS SUPPORTED)
//     // -----------------------------------------------------------------------
//     void StartFlexibleUpdate()
//     {
//         // Hide update buttons and allow cancel
//         updateButton.gameObject.SetActive(false);
//         quitButton.gameObject.SetActive(false);
//         cancelButton.gameObject.SetActive(true);

//         updateTitle.text = "Updating...";
//         updateDescription.text = "Downloading update…";

//         var request = appUpdateManager.StartUpdate(appUpdateInfo, AppUpdateOptions.FlexibleAppUpdateOptions());

//         request.Completed += (operation) =>
//         {
//             if (operation.Error != AppUpdateErrorCode.NoError)
//             {
//                 Debug.Log("Flexible update failed: " + operation.Error);
//                 return;
//             }

//             updateDescription.text = "Update downloaded. Installing…";

//             // COMPLETE UPDATE
//             appUpdateManager.CompleteUpdate();
//         };
//     }

//     // -----------------------------------------------------------------------
//     // CANCEL UPDATE
//     // -----------------------------------------------------------------------
//     void CancelUpdate()
//     {
//         updatePanel.SetActive(false);
//         backgroundPanel.SetActive(false);
//         Debug.Log("Update cancelled by user.");
//     }

//     // -----------------------------------------------------------------------
//     // QUIT GAME
//     // -----------------------------------------------------------------------
//     void QuitGame()
//     {
// #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
// #else
//         Application.Quit();
// #endif
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using TMPro;

// #if !UNITY_EDITOR
using Google.Play.AppUpdate;
using Google.Play.Common;
// #endif

public class UpdateChecker : MonoBehaviour
{
    [Header("Main UI")]
    public GameObject backgroundPanel;
    public GameObject updatePanel;
    public TMP_Text updateTitle;
    public TMP_Text updateDescription;
    public Button updateButton;
    public Button cancelButton;
    public Button quitButton;

// #if !UNITY_EDITOR
    private AppUpdateManager appUpdateManager;
    private AppUpdateInfo appUpdateInfo;
// #endif

    void Start()
    {
        Debug.Log("UpdateChecker START");

        
        // Test Mode in Editor
// #if UNITY_EDITOR
//         Debug.Log("Editor Mode: Simulating update available");
//         ShowUpdatePopup(forced: false); // simulate non-forced update
// #else
        appUpdateManager = new AppUpdateManager();

        backgroundPanel.SetActive(false);
        updatePanel.SetActive(false);

        CheckForUpdate();
// #endif
    }

// #if !UNITY_EDITOR
    void CheckForUpdate()
    {
        Debug.Log("Checking for update…");

        var request = appUpdateManager.GetAppUpdateInfo();

        request.Completed += (operation) =>
        {
            if (operation.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogError("Update check failed: " + operation.Error);
                return;
            }

            appUpdateInfo = operation.GetResult();

            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                bool forced = appUpdateInfo.UpdatePriority >= 5;
                ShowUpdatePopup(forced);
            }
        };
    }
// #endif

    void ShowUpdatePopup(bool forced)
    {
        Debug.Log("Showing Update Popup. Forced = " + forced);

        updatePanel.SetActive(true);
        backgroundPanel.SetActive(true);

        if (forced)
        {
            updateTitle.text = "Update Required";
            updateDescription.text = "A new version is required to continue.";
            cancelButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(true);
        }
        else
        {
            updateTitle.text = "Update Available";
            updateDescription.text = "A newer version is available.";
            cancelButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(false);
        }

        updateButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();

        // Add debug logs for button clicks
        updateButton.onClick.AddListener(() => Debug.Log("Update Button CLICKED"));
        cancelButton.onClick.AddListener(() => Debug.Log("Cancel Button CLICKED"));
        quitButton.onClick.AddListener(() => Debug.Log("Quit Button CLICKED"));

// #if UNITY_EDITOR
        // Editor simulation actions
        updateButton.onClick.AddListener(() => Debug.Log("Editor: Simulate Update Start"));
// #endif

// #if !UNITY_EDITOR
        // Real actions on device
        if (forced)
            updateButton.onClick.AddListener(() => StartImmediateUpdate());
        else
            updateButton.onClick.AddListener(() => StartFlexibleUpdate());

        cancelButton.onClick.AddListener(CancelUpdate);
        quitButton.onClick.AddListener(QuitGame);
// #endif
    }

// #if !UNITY_EDITOR
    void StartImmediateUpdate()
    {
        var request = appUpdateManager.StartUpdate(appUpdateInfo, AppUpdateOptions.ImmediateAppUpdateOptions());
        request.Completed += (operation) =>
        {
            if (operation.Error != AppUpdateErrorCode.NoError)
                Debug.LogError("Immediate update failed: " + operation.Error);
            else
                Debug.Log("Immediate update started successfully.");
        };
    }

    void StartFlexibleUpdate()
    {
        updateButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);

        updateTitle.text = "Updating...";
        updateDescription.text = "Downloading update…";

        var request = appUpdateManager.StartUpdate(appUpdateInfo, AppUpdateOptions.FlexibleAppUpdateOptions());

        request.Completed += (operation) =>
        {
            if (operation.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogError("Flexible update failed: " + operation.Error);
                return;
            }

            updateDescription.text = "Update downloaded. Installing…";
            appUpdateManager.CompleteUpdate();
        };
    }
// #endif

    void CancelUpdate()
    {
        Debug.Log("Update CANCELED by user");
        updatePanel.SetActive(false);
        backgroundPanel.SetActive(false);
    }

    void QuitGame()
    {
        Debug.Log("QUIT button pressed → Exiting game");
// #if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
// #else
        Application.Quit();
// #endif
    }
}
