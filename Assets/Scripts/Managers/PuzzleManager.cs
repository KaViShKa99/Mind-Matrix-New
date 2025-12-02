using UnityEngine;
using Firebase;
using Firebase.Analytics;

public class PuzzleManager : MonoBehaviour
{
    [Header("Core Components (assign in Inspector)")]
    public TileManager tileManager;
    public PuzzleGenerator generator;
    public PuzzleChecker checker;
    public HintSystem hintSystem;
    public PopupBoxUI levelCompleteUI;
    // public LevelDetailsManager levelDetailsManager;

    [Header("Gameplay")]
    private int gridSize = 3;
    private int shuffleMoves = 0;

    private int currentLevel;
    private int moveCount = 1;
    private float elapsedTime = 0f;
    private bool gameOverTriggered = false;

    void Update()
    {
        if (!checker.IsPuzzleComplete() && LevelDetailsManager.Instance.GetTimerCount() <= 0 && !gameOverTriggered)
        {
            gameOverTriggered = true;

            LevelDetailsManager.Instance.StopTimer();

            checker.OnGameOver();
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayGameOver();

            FirebaseInit.Instance.LogLevelCompleteEvent();
        }

    }

    void Start()
    {
        // if (!LevelPlayCounter.Instance.skipNextStartCount)
        // {
        //     LevelPlayCounter.Instance.OnLevelPlayed();
        // }
        // LevelPlayCounter.Instance.skipNextStartCount = false;


        LevelDetailsManager.Instance.StartTimer();
        // AdsManager.Instance.LoadBanner();

        currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);

        // Adjust difficulty based on level
        SetLevelDifficulty(currentLevel);

        FirebaseInit.Instance.LogLevelEvent();
        FirebaseInit.Instance.LogCoinsEarnedEvent(CoinManager.Instance.GetCoins());


        // Basic sanity
        if (tileManager == null || generator == null || checker == null)
        {
            Debug.LogError("PuzzleManager: Missing required components.");
            return;
        }

        // Initialize subsystems
        tileManager.Initialize(gridSize);
        generator.Initialize(tileManager, gridSize);
        checker.Initialize(tileManager, gridSize);
        if (hintSystem != null) hintSystem.Initialize(tileManager, gridSize);

        // Create tiles and shuffle
        generator.GenerateTilesInOrder();
        generator.ShuffleTiles(shuffleMoves);
        LevelDetailsManager.Instance.SetUIDetails(currentLevel,moveCount,elapsedTime,gridSize);

        checker.CheckCorrectTiles();

        // Optional check
        if (checker.IsPuzzleComplete())
        {
            checker.OnPuzzleComplete();
            AudioManager.Instance.PlayLevelComplete();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.musicSource.Stop();
        }

    }

   

    private void SetLevelDifficulty(int level)
    {
        if (level < 13)
        {
            gridSize = 3; // Fixed for all 15 levels

            switch (level)
            {
                case 1: shuffleMoves = 2; moveCount = 10; elapsedTime = 60; break;
                case 2: shuffleMoves = 4; moveCount = 10; elapsedTime = 60; break;
                case 3: shuffleMoves = 6; moveCount = 15; elapsedTime = 60; break;
                case 4: shuffleMoves = 8; moveCount = 15; elapsedTime = 60; break;
                case 5: shuffleMoves = 10; moveCount = 20; elapsedTime = 60; break;
                case 6: shuffleMoves = 12; moveCount = 20; elapsedTime = 65; break;
                case 7: shuffleMoves = 14; moveCount = 25; elapsedTime = 65; break;
                case 8: shuffleMoves = 16; moveCount = 25; elapsedTime = 65; break;
                case 9: shuffleMoves = 18; moveCount = 25; elapsedTime = 65; break;
                case 10: shuffleMoves = 20; moveCount = 25; elapsedTime = 65; break;
                case 11: shuffleMoves = 22; moveCount = 30; elapsedTime = 65; break;//14
                case 12: shuffleMoves = 24; moveCount = 30; elapsedTime = 65; break;//16
                default: shuffleMoves = 5; moveCount = 20; elapsedTime = 30; break;
            }
        }
        else if (level >= 13 && level < 29)
        {
            gridSize = 4;

            switch (level)
            {
                case 13: shuffleMoves = 2; moveCount = 20; elapsedTime = 5; break;
                case 14: shuffleMoves = 4; moveCount = 20; elapsedTime = 60; break;
                case 15: shuffleMoves = 6; moveCount = 22; elapsedTime = 60; break;
                case 16: shuffleMoves = 8; moveCount = 24; elapsedTime = 60; break;
                case 17: shuffleMoves = 10; moveCount = 25; elapsedTime = 60; break;
                case 18: shuffleMoves = 12; moveCount = 26; elapsedTime = 60; break;
                case 19: shuffleMoves = 14; moveCount = 28; elapsedTime = 60; break;
                case 20: shuffleMoves = 16; moveCount = 30; elapsedTime = 65; break;
                case 21: shuffleMoves = 18; moveCount = 32; elapsedTime = 65; break;
                case 22: shuffleMoves = 20; moveCount = 34; elapsedTime = 65; break;
                case 23: shuffleMoves = 22; moveCount = 36; elapsedTime = 70; break;
                case 24: shuffleMoves = 24; moveCount = 38; elapsedTime = 70; break;
                case 25: shuffleMoves = 26; moveCount = 40; elapsedTime = 70; break;
                case 26: shuffleMoves = 28; moveCount = 42; elapsedTime = 75; break;
                case 27: shuffleMoves = 30; moveCount = 44; elapsedTime = 80; break;
                case 28: shuffleMoves = 32; moveCount = 46; elapsedTime = 85; break;

                default: shuffleMoves = 5; moveCount = 20; elapsedTime = 30; break;
            }

        }
        else if (level < 59) // Levels 29–58
        {
            gridSize = 5;

            switch (level)
            {
                case 29: shuffleMoves = 34; moveCount = 48; elapsedTime = 5; break;
                case 30: shuffleMoves = 36; moveCount = 50; elapsedTime = 95; break;
                case 31: shuffleMoves = 38; moveCount = 52; elapsedTime = 100; break;
                case 32: shuffleMoves = 40; moveCount = 54; elapsedTime = 105; break;
                case 33: shuffleMoves = 42; moveCount = 56; elapsedTime = 110; break;
                case 34: shuffleMoves = 44; moveCount = 58; elapsedTime = 115; break;
                case 35: shuffleMoves = 46; moveCount = 60; elapsedTime = 120; break;
                case 36: shuffleMoves = 48; moveCount = 62; elapsedTime = 125; break;
                case 37: shuffleMoves = 50; moveCount = 64; elapsedTime = 130; break;
                case 38: shuffleMoves = 52; moveCount = 66; elapsedTime = 135; break;
                case 39: shuffleMoves = 54; moveCount = 68; elapsedTime = 140; break;
                case 40: shuffleMoves = 56; moveCount = 70; elapsedTime = 145; break;
                case 41: shuffleMoves = 58; moveCount = 72; elapsedTime = 150; break;
                case 42: shuffleMoves = 60; moveCount = 74; elapsedTime = 155; break;
                case 43: shuffleMoves = 62; moveCount = 76; elapsedTime = 160; break;
                case 44: shuffleMoves = 64; moveCount = 78; elapsedTime = 165; break;
                case 45: shuffleMoves = 66; moveCount = 80; elapsedTime = 170; break;
                case 46: shuffleMoves = 68; moveCount = 82; elapsedTime = 175; break;
                case 47: shuffleMoves = 70; moveCount = 84; elapsedTime = 180; break;
                case 48: shuffleMoves = 72; moveCount = 86; elapsedTime = 185; break;
                case 49: shuffleMoves = 74; moveCount = 88; elapsedTime = 190; break;
                case 50: shuffleMoves = 76; moveCount = 90; elapsedTime = 195; break;
                case 51: shuffleMoves = 78; moveCount = 92; elapsedTime = 200; break;
                case 52: shuffleMoves = 80; moveCount = 94; elapsedTime = 205; break;
                case 53: shuffleMoves = 82; moveCount = 96; elapsedTime = 210; break;
                case 54: shuffleMoves = 84; moveCount = 98; elapsedTime = 215; break;
                case 55: shuffleMoves = 86; moveCount = 100; elapsedTime = 220; break;
                case 56: shuffleMoves = 88; moveCount = 102; elapsedTime = 225; break;
                case 57: shuffleMoves = 90; moveCount = 104; elapsedTime = 230; break;
                case 58: shuffleMoves = 92; moveCount = 106; elapsedTime = 235; break;

                default: shuffleMoves = 36; moveCount = 50; elapsedTime = 90; break;
            }
        }


        Debug.Log($"Loaded Level {level} | Grid Size: {gridSize} | Shuffle Moves: {shuffleMoves}");
    }
}
