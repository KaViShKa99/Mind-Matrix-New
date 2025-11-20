using System;
using UnityEngine;

public class LevelDetailsManager : MonoBehaviour
{
    public static LevelDetailsManager Instance { get; private set; }

    [Header("Game Data")]
    private int level = 0;
    private int moveCount = 0;
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
            elapsedTime += Time.deltaTime; // fixed: increment instead of subtract
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

    public void SetUIDetails(int level, int moveCount, float elapsedTime, int gridSize)
    {
        this.level = level;
        this.moveCount = moveCount;
        this.startTime = elapsedTime;
        this.elapsedTime = elapsedTime;
        this.gridSize = gridSize;

        OnLevelChanged?.Invoke(this.level);
        OnMoveChanged?.Invoke(this.moveCount);
        OnTimeChanged?.Invoke(this.elapsedTime);
        OnGridSizeChanged?.Invoke(this.gridSize);
    }

    public void StopTimer() => timerRunning = false;
    public void StartTimer() => timerRunning = true;

    public int GetMoveCount() => moveCount;
    public float GetTimerCount() => elapsedTime;
    public int GetLevelSize() => gridSize;
    public int GetLevel() => level;
    public float GetStartTime() => startTime;

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
}


