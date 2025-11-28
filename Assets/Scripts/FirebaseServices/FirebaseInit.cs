using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;

public class FirebaseInit : MonoBehaviour
{
    // Singleton instance
    public static FirebaseInit Instance { get; private set; }

    private bool firebaseReady = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeFirebase();
    }

    /// <summary>
    /// Initializes Firebase Analytics and Crashlytics
    /// </summary>
    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                firebaseReady = true;

                // Initialize Analytics
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);

                // Initialize Crashlytics
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                Crashlytics.Log("Crashlytics initialized");

                Debug.Log("Firebase initialized successfully");
            }
            else
            {
                firebaseReady = false;
                Debug.LogError("Could not resolve Firebase dependencies: " + status);
            }
        });
    }

    /// <summary>
    /// Logs level completion event
    /// </summary>
    public void LogLevelCompleteEvent()
    {
        if (!firebaseReady || LevelDetailsManager.Instance == null) return;

        var level = LevelDetailsManager.Instance;

        FirebaseAnalytics.LogEvent("level_complete",
            new Parameter("level_number", level.GetLevel()),
            new Parameter("used_time", level.GetUsedTime()),
            new Parameter("max_time", level.GetStartTime()),
            new Parameter("used_moves", level.GetUsedMoves()),
            new Parameter("max_moves", level.GetStartMoveCount()),
            new Parameter("hints_used", level.GetHintCount()),
            new Parameter("restarts", level.GetRestartCount()),
            new Parameter("ads_watched", level.GetAdWatchCount())
        );

        // Debug.Log($"[Editor Simulation] Event: level_complete | level: {level.GetLevel()}, time: {level.GetUsedTime()}s,max_time: {level.GetStartTime()}, used_moves: {moves}, max_moves: {level.GetStartMoveCount()}, hints: {level.GetHintCount()}, restarts: {level.GetRestartCount()}, ads_watched: {level.GetAdWatchCount()}");

    }

    /// <summary>
    /// Logs level failed event
    /// </summary>
    public void LogLevelFailedEvent()
    {
        if (!firebaseReady || LevelDetailsManager.Instance == null) return;

        var level = LevelDetailsManager.Instance;

        FirebaseAnalytics.LogEvent("level_failed",
            new Parameter("level_number", level.GetLevel()),
            new Parameter("used_time", level.GetUsedTime()),
            new Parameter("max_time", level.GetStartTime()),
            new Parameter("used_moves", level.GetUsedMoves()),
            new Parameter("max_moves", level.GetStartMoveCount()),
            new Parameter("hints_used", level.GetHintCount()),
            new Parameter("restarts", level.GetRestartCount()),
            new Parameter("ads_watched", level.GetAdWatchCount())
        );

        // Debug.Log($"[Editor Simulation] Event: level_failed | level: {level.GetLevel()}, time: {level.GetUsedTime()}s, max_time: {level.GetStartTime()}, used_moves: {moves}, max_moves: {startMoveCount}, hints: {level.GetHintCount()}, restarts: {level.GetRestartCount()}, ads_watched: {level.GetAdWatchCount()}");

    }

    /// <summary>
    /// Logs the current level when player starts a level
    /// </summary>
    public void LogLevelEvent()
    {
        if (!firebaseReady || LevelDetailsManager.Instance == null) return;

        FirebaseAnalytics.LogEvent("level",
            new Parameter("current_level", LevelStageManager.Instance.SelectedLevel),
            new Parameter("unlocked_level", LevelStageManager.Instance.UnlockedLevel)
        );                                                                                                                                                                                    

        // Debug.Log($"[Editor Simulation] Event: current_level | level: {LevelStageManager.Instance.SelectedLevel} (unlocked: {LevelStageManager.Instance.UnlockedLevel})");

    }

    public void LogButtonClickedEvent(string name)
    {
        if (!firebaseReady) return;

        FirebaseAnalytics.LogEvent("clicked_button",
            new Parameter("button", name)
        );

        // Debug.Log($"[Editor Simulation] Event: clicked_button | button: {name}");
    }

    /// <summary>
    /// Optional: Log custom coin earned events
    /// </summary>
    public void LogCoinsEarnedEvent(int coins)
    {
        if (!firebaseReady) return;

        FirebaseAnalytics.LogEvent("coins_earned",
            new Parameter("amount", coins)
        );

        // Debug.Log($"[Editor Simulation] Event: coins_earned | amount: {coins}");
    }
}
