using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FTUEController : MonoBehaviour
{
    public static FTUEController Instance;

    [Header("Core")]
    public TileManager tileManager;
    public PuzzleGenerator generator;
    public PuzzleChecker checker;
    public FTUEManager ftueManager;

    [Header("FTUE Settings")]
    public int ftueGridSize = 3;
    public int shuffleMoves = 2;

    [Header("Celebration")]
    public GameObject puzzleBoard;
    public GameObject burstEffectPrefab;
    public Transform burstSpawnPoint;
    public TextMeshProUGUI completeText;
    public TextMeshProUGUI puzzleTitleText;
    public GameObject bgDimmer;

    private bool finished = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 🚫 Skip FTUE if already done
        if (PlayerPrefs.GetInt("FTUE_Completed", 0) == 1)
        {
            LoadHome();
            return;
        }

        // Init puzzle
        tileManager.Initialize(ftueGridSize);
        generator.Initialize(tileManager, ftueGridSize);
        checker.Initialize(tileManager, ftueGridSize);

        generator.GenerateTilesInOrder();
        generator.ShuffleTiles(shuffleMoves);
        checker.CheckCorrectTiles();

        // Start FTUE logic
        ftueManager.StartFTUE();
    }

    void Update()
    {
        if (!finished && checker.IsPuzzleComplete())
        {
            finished = true;
            OnFTUEFinished();
        }
    }

    public void OnFTUEFinished()
    {
        finished = true;

        PlayerPrefs.SetInt("FTUE_Completed", 1);
        PlayerPrefs.Save();


        // if (PlayerPrefs.GetInt("FTUE_Completed", 0) == 0)
        // {
        //     AudioManager.Instance?.PlayLevelComplete();
        // }
        // AudioManager.Instance?.PlayLevelComplete();



        // 🎉 Celebration
        if (burstEffectPrefab && burstSpawnPoint)
            Instantiate(burstEffectPrefab, burstSpawnPoint.position, Quaternion.identity);

        if (completeText)
        {
            bgDimmer.SetActive(true);
            completeText.gameObject.SetActive(true);
            completeText.text = "Great job! You solved the puzzle";


            if (puzzleTitleText)
                puzzleTitleText.gameObject.SetActive(false);
                
            if (puzzleBoard)
                puzzleBoard.SetActive(false);
        }

        StartCoroutine(LoadHomeDelayed());
    }

    IEnumerator LoadHomeDelayed()
    {
        yield return new WaitForSeconds(1.5f);
        LoadHome();
    }

    void LoadHome()
    {
        SceneManager.LoadScene("HomeScreenScene");
    }
}
