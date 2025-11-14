// using UnityEngine;
// using TMPro;

// public class LevelDetailsManager : MonoBehaviour
// {
//     [Header("References")]
//     public TextMeshProUGUI moveText;
//     public TextMeshProUGUI timeText;
//     public TextMeshProUGUI levelCount;

//     [Header("Game Data")]
//     private int level = 0;
//     private int moveCount = 0;
//     private float elapsedTime = 0f;
//     public bool timerRunning = true;

//     private void OnValidate()
//     {
//         UpdateMoveText();
//         UpdateTimeText();
//         UpdateLevelText();
//     }

//     void Update()
//     {
//         UpdateLevelText();
//         UpdateMoveText();
//         if (timerRunning)
//         {
//             elapsedTime -= Time.deltaTime;
//             UpdateTimeText();
//         }
//     }

//     public void ReduceMove()
//     {
//         moveCount--;
//         UpdateMoveText();
//     }

//     public void ResetLevelInfo()
//     {
//         moveCount = 0;
//         elapsedTime = 0f;
//         UpdateMoveText();
//         UpdateTimeText();
//     }

//    public void SetUIDetails(int level, int moveCount, float elapsedTime)
//     {
//         this.level = level;
//         this.moveCount = moveCount;
//         this.elapsedTime = elapsedTime;
//     }

//     private void UpdateLevelText()
//     {
//         levelCount.text = $"{level}";
//     }
//     private void UpdateMoveText()
//     {
//         moveText.text = $"{moveCount}";
//     }

//     private void UpdateTimeText()
//     {
//         int minutes = Mathf.FloorToInt(elapsedTime / 60);
//         int seconds = Mathf.FloorToInt(elapsedTime % 60);
//         timeText.text = $"{minutes:00}:{seconds:00}";
//     }

//     public void StopTimer() => timerRunning = false;
//     public void StartTimer() => timerRunning = true;

//     public int GetMoveCount()
//     {
//         return moveCount;
//     }
//     public float GetTimerCount()
//     {
//         return elapsedTime;
//     }
// }
using UnityEngine;
using TMPro;


public class LevelDetailsManager : MonoBehaviour
{
    public static LevelDetailsManager Instance { get; private set; }

    [Header("References")]
    public TextMeshProUGUI moveText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelCount;

    [Header("Game Data")]
    private int level = 0;
    private int moveCount = 0;
    private float elapsedTime = 0f;
    public bool timerRunning = true;

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

    private void OnValidate()
    {
        UpdateMoveText();
        UpdateTimeText();
        UpdateLevelText();
    }

    private void Update()
    {
        UpdateLevelText();
        UpdateMoveText();

        if (timerRunning)
        {
            elapsedTime -= Time.deltaTime;
            UpdateTimeText();
        }
    }

    public void ReduceMove()
    {
        moveCount--;
        UpdateMoveText();
    }

    public void ResetLevelInfo()
    {
        moveCount = 0;
        elapsedTime = 0f;
        UpdateMoveText();
        UpdateTimeText();
    }

    public void SetUIDetails(int level, int moveCount, float elapsedTime)
    {
        this.level = level;
        this.moveCount = moveCount;
        this.elapsedTime = elapsedTime;
    }

    private void UpdateLevelText()
    {
        if (levelCount != null)
            levelCount.text = $"{level}";
    }

    private void UpdateMoveText()
    {
        if (moveText != null)
            moveText.text = $"{moveCount}";
    }

    private void UpdateTimeText()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void StopTimer() => timerRunning = false;
    public void StartTimer() => timerRunning = true;

    public int GetMoveCount() => moveCount;
    public float GetTimerCount() => elapsedTime;
    public bool IsPaused { get; private set; }

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


