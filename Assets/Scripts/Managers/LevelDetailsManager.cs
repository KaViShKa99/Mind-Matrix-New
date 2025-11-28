using System;
using UnityEngine;

public class LevelDetailsManager : MonoBehaviour
{
    public static LevelDetailsManager Instance { get; private set; }

    [Header("Player Actions")]
    private int adWatchCount = 0;
    private int hintCount = 0;
    private int restartCount = 0;

    [Header("Game Data")]
    private int level = 0;
    private int moveCount = 0;
    private int startMoveCount = 0;
    private float elapsedTime = 0f;
    private float startTime = 0f;
    private int gridSize = 3;
    public bool timerRunning = true;

    public bool IsPaused { get; private set; }

    // Events UI (or other systems) can subscribe to
    public event Action<int> OnMoveChanged;
    public event Action<float> OnTimeChanged;
    public event Action<int> OnLevelChanged;
    public event Action<int> OnGridSizeChanged;

    public event Action<int> OnAdWatchChanged;
    public event Action<int> OnHintChanged;
    public event Action<int> OnRestartChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // invoke level/move events only when values change from external callers
        if (timerRunning && !IsPaused)
        {
            elapsedTime -= Time.deltaTime; // fixed: increment instead of subtract
            OnTimeChanged?.Invoke(elapsedTime);
        }
    }

    public void ReduceMove()
    {
        moveCount = Mathf.Max(0, moveCount - 1);
        OnMoveChanged?.Invoke(moveCount);
    }

    public void ResetLevelInfo()
    {
        moveCount = 0;
        elapsedTime = 0f;
        startTime = 0f;
        OnMoveChanged?.Invoke(moveCount);
        OnTimeChanged?.Invoke(elapsedTime);
    }

    public void AddHint()
    {
        hintCount++;
        OnHintChanged?.Invoke(hintCount);
    }

    public void AddRestart()
    {
        restartCount++;
        OnRestartChanged?.Invoke(restartCount);
    }

    public void SetUIDetails(int level, int moveCount, float elapsedTime, int gridSize)
    {
        this.level = level;
        this.moveCount = moveCount;
        this.startMoveCount = moveCount;
        this.startTime = elapsedTime;
        this.elapsedTime = elapsedTime;
        this.gridSize = gridSize;

        OnLevelChanged?.Invoke(this.level);
        OnMoveChanged?.Invoke(this.moveCount);
        OnTimeChanged?.Invoke(this.elapsedTime);
        OnGridSizeChanged?.Invoke(this.gridSize);

        adWatchCount = 0;
        hintCount = 0;
        restartCount = 0;

        OnAdWatchChanged?.Invoke(adWatchCount);
        OnHintChanged?.Invoke(hintCount);
        OnRestartChanged?.Invoke(restartCount);

    }

    public void StopTimer() => timerRunning = false;
    public void StartTimer() => timerRunning = true;



    public int GetAdWatchCount() => adWatchCount;
    public int GetHintCount() => hintCount;
    public int GetRestartCount() => restartCount;

    public int GetMoveCount() => moveCount;
    public float GetTimerCount() => elapsedTime;
    public int GetLevelSize() => gridSize;
    public int GetLevel() => level;
    public float GetStartTime() => startTime;
    public int GetStartMoveCount() => startMoveCount;
    public int GetUsedMoves() => startMoveCount - moveCount;

    public float GetUsedTime() => Mathf.Round((startTime - elapsedTime) * 100f) / 100f;


    public void PauseGame()
    {
        IsPaused = true;
        StopTimer();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        StartTimer();
    }


    public void AddAdWatch()
    {
        adWatchCount++;
        OnAdWatchChanged?.Invoke(adWatchCount);
    }

   


}


