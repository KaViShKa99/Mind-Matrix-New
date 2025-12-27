using UnityEngine;

#if UNITY_ANDROID || UNITY_IOS
using Firebase;
using Firebase.Analytics;
#endif

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

#if UNITY_ANDROID || UNITY_IOS
            FirebaseInit.Instance.LogLevelCompleteEvent();
#endif
        }

    }

    void Start()
    {
 
        LevelDetailsManager.Instance.StartTimer();

        if (GameStartManager.StartMode == GameStartMode.SelectedLevel)
        {
            currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        }
        else if (GameStartManager.StartMode == GameStartMode.UnlockedLevel)
        {
            currentLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        }
        else
        {
            currentLevel = 1; // fallback safety
        }

        // Adjust difficulty based on level
        SetLevelDifficulty(currentLevel);

#if UNITY_ANDROID || UNITY_IOS
        FirebaseInit.Instance.LogLevelEvent();
        FirebaseInit.Instance.LogCoinsEarnedEvent(CoinManager.Instance.GetCoins());
#endif


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
        else if (level >= 13 && level < 33)
        {
            gridSize = 4;

            switch (level)
            {
                case 13: shuffleMoves = 2; moveCount = 20; elapsedTime = 60; break;
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
                case 29: shuffleMoves = 34; moveCount = 48; elapsedTime = 85; break;
                case 30: shuffleMoves = 36; moveCount = 50; elapsedTime = 85; break;
                case 31: shuffleMoves = 38; moveCount = 52; elapsedTime = 85; break;
                case 32: shuffleMoves = 40; moveCount = 54; elapsedTime = 85; break;

                default: shuffleMoves = 5; moveCount = 20; elapsedTime = 30; break;
            }

        }
        else if (level < 113) // Levels 33–100
        {
            gridSize = 5;

            switch (level)
            {
                case 33: shuffleMoves = 2; moveCount = 30; elapsedTime = 110; break;
                case 34: shuffleMoves = 4; moveCount = 30; elapsedTime = 115; break;
                case 35: shuffleMoves = 6; moveCount = 35; elapsedTime = 120; break;
                case 36: shuffleMoves = 8; moveCount = 40; elapsedTime = 125; break;
                case 37: shuffleMoves = 10; moveCount = 45; elapsedTime = 130; break;
                case 38: shuffleMoves = 12; moveCount = 50; elapsedTime = 135; break;
                case 39: shuffleMoves = 14; moveCount = 55; elapsedTime = 140; break;
                case 40: shuffleMoves = 16; moveCount = 60; elapsedTime = 145; break;
                case 41: shuffleMoves = 18; moveCount = 65; elapsedTime = 150; break;
                case 42: shuffleMoves = 20; moveCount = 74; elapsedTime = 155; break;
                case 43: shuffleMoves = 22; moveCount = 76; elapsedTime = 160; break;
                case 44: shuffleMoves = 24; moveCount = 78; elapsedTime = 165; break;
                case 45: shuffleMoves = 26; moveCount = 80; elapsedTime = 170; break;
                case 46: shuffleMoves = 28; moveCount = 82; elapsedTime = 175; break;
                case 47: shuffleMoves = 30; moveCount = 84; elapsedTime = 180; break;
                case 48: shuffleMoves = 32; moveCount = 86; elapsedTime = 185; break;
                case 49: shuffleMoves = 34; moveCount = 88; elapsedTime = 190; break;
                case 50: shuffleMoves = 36; moveCount = 90; elapsedTime = 195; break;
                case 51: shuffleMoves = 38; moveCount = 92; elapsedTime = 200; break;
                case 52: shuffleMoves = 40; moveCount = 94; elapsedTime = 205; break;
                case 53: shuffleMoves = 42; moveCount = 96; elapsedTime = 210; break;
                case 54: shuffleMoves = 44; moveCount = 98; elapsedTime = 215; break;
                case 55: shuffleMoves = 46; moveCount = 100; elapsedTime = 220; break;
                case 56: shuffleMoves = 48; moveCount = 102; elapsedTime = 225; break;
                case 57: shuffleMoves = 50; moveCount = 104; elapsedTime = 230; break;
                case 58: shuffleMoves = 52; moveCount = 106; elapsedTime = 235; break;
                case 59: shuffleMoves = 54; moveCount = 108; elapsedTime = 240; break;
                case 60: shuffleMoves = 56; moveCount = 110; elapsedTime = 245; break;
                case 61: shuffleMoves = 58; moveCount = 112; elapsedTime = 250; break;
                case 62: shuffleMoves = 60; moveCount = 114; elapsedTime = 255; break;
                case 63: shuffleMoves = 62; moveCount = 116; elapsedTime = 260; break;
                case 64: shuffleMoves = 64; moveCount = 118; elapsedTime = 265; break;
                case 65: shuffleMoves = 66; moveCount = 120; elapsedTime = 270; break;
                case 66: shuffleMoves = 68; moveCount = 122; elapsedTime = 275; break;
                case 67: shuffleMoves = 70; moveCount = 124; elapsedTime = 280; break;
                case 68: shuffleMoves = 72; moveCount = 126; elapsedTime = 285; break;
                case 69: shuffleMoves = 74; moveCount = 128; elapsedTime = 290; break;
                case 70: shuffleMoves = 76; moveCount = 130; elapsedTime = 295; break;
                case 71: shuffleMoves = 78; moveCount = 132; elapsedTime = 300; break;
                case 72: shuffleMoves = 80; moveCount = 134; elapsedTime = 305; break;
                case 73: shuffleMoves = 82; moveCount = 136; elapsedTime = 310; break;
                case 74: shuffleMoves = 84; moveCount = 138; elapsedTime = 315; break;
                case 75: shuffleMoves = 86; moveCount = 140; elapsedTime = 320; break;
                case 76: shuffleMoves = 88; moveCount = 142; elapsedTime = 325; break;
                case 77: shuffleMoves = 90; moveCount = 144; elapsedTime = 330; break;
                case 78: shuffleMoves = 92; moveCount = 146; elapsedTime = 335; break;
                case 79: shuffleMoves = 94; moveCount = 148; elapsedTime = 340; break;
                case 80: shuffleMoves = 96; moveCount = 150; elapsedTime = 345; break;
                case 81: shuffleMoves = 98; moveCount = 152; elapsedTime = 350; break;
                case 82: shuffleMoves = 100; moveCount = 154; elapsedTime = 355; break;
                case 83: shuffleMoves = 102; moveCount = 156; elapsedTime = 360; break;
                case 84: shuffleMoves = 104; moveCount = 158; elapsedTime = 365; break;
                case 85: shuffleMoves = 106; moveCount = 160; elapsedTime = 370; break;
                case 86: shuffleMoves = 108; moveCount = 162; elapsedTime = 375; break;
                case 87: shuffleMoves = 110; moveCount = 164; elapsedTime = 380; break;
                case 88: shuffleMoves = 112; moveCount = 166; elapsedTime = 385; break;
                case 89: shuffleMoves = 114; moveCount = 168; elapsedTime = 390; break;
                case 90: shuffleMoves = 116; moveCount = 170; elapsedTime = 395; break;
                case 91: shuffleMoves = 118; moveCount = 172; elapsedTime = 400; break;
                case 92: shuffleMoves = 120; moveCount = 174; elapsedTime = 405; break;
                case 93: shuffleMoves = 122; moveCount = 176; elapsedTime = 410; break;
                case 94: shuffleMoves = 124; moveCount = 178; elapsedTime = 415; break;
                case 95: shuffleMoves = 126; moveCount = 180; elapsedTime = 420; break;
                case 96: shuffleMoves = 128; moveCount = 182; elapsedTime = 425; break;
                case 97: shuffleMoves = 130; moveCount = 184; elapsedTime = 430; break;
                case 98: shuffleMoves = 132; moveCount = 186; elapsedTime = 435; break;
                case 99: shuffleMoves = 134; moveCount = 188; elapsedTime = 440; break;
                case 100: shuffleMoves = 136; moveCount = 190; elapsedTime = 300; break;
                case 101: shuffleMoves = 137; moveCount = 190; elapsedTime = 310; break; //save 54
                case 102: shuffleMoves = 138; moveCount = 190; elapsedTime = 320; break;
                case 103: shuffleMoves = 139; moveCount = 190; elapsedTime = 330; break;
                case 104: shuffleMoves = 140; moveCount = 190; elapsedTime = 340; break;
                case 105: shuffleMoves = 141; moveCount = 190; elapsedTime = 350; break;
                case 106: shuffleMoves = 142; moveCount = 190; elapsedTime = 360; break;
                case 107: shuffleMoves = 143; moveCount = 190; elapsedTime = 370; break;
                case 108: shuffleMoves = 144; moveCount = 190; elapsedTime = 380; break;
                case 109: shuffleMoves = 145; moveCount = 190; elapsedTime = 390; break;
                case 110: shuffleMoves = 146; moveCount = 190; elapsedTime = 400; break;
                case 111: shuffleMoves = 148; moveCount = 190; elapsedTime = 410; break;
                case 112: shuffleMoves = 150; moveCount = 190; elapsedTime = 420; break;
                default: shuffleMoves = 2; moveCount = 56; elapsedTime = 110; break;
            }
        }


        Debug.Log($"Loaded Level {level} | Grid Size: {gridSize} | Shuffle Moves: {shuffleMoves}");
    }
}
