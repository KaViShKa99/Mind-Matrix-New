using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


#if UNITY_ANDROID || UNITY_IOS
using Firebase;
using Firebase.Analytics;
#endif


public class TileManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform tileContainer;
    public GridLayoutGroup gridLayoutGroup;
    public GameObject tilePrefab;        // normal tile with Tile.cs
    public GameObject emptyTilePrefab;
    public GameObject correctTilePrefab; // visually 'locked' correct tile
    public PuzzleChecker checker;
    public RectTransform popupCoinStart;
    public RectTransform coinUITopRight;
    public FTUEManager ftueManager;

    [Header("Animation")]
    public float slideDuration = 0.15f;

    [HideInInspector] public int gridSize = 3;

    [HideInInspector] public bool ftueLocked = false;


    private Transform emptyTile;
    private bool isAnimating = false;

    private Stack<MoveRecord> moveHistory = new Stack<MoveRecord>();

    [System.Serializable]
    public class MoveRecord
    {
        public int tileIndex;
        public int emptyIndex;
    }

    public void Initialize(int gridSize)
    {
        this.gridSize = gridSize;
    }

    public void GenerateGrid()
    {
        moveHistory.Clear();
        // clean
        foreach (Transform child in tileContainer) Destroy(child.gameObject);

        int total = gridSize * gridSize;
        for (int i = 1; i <= total - 1; i++)
        {
            GameObject t = Instantiate(tilePrefab, tileContainer);
            t.name = "Tile_" + i;
            var label = t.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (label) label.text = i.ToString();

            // assign manager reference will be done in editor or tile will find PuzzleManager
        }

        GameObject empty = Instantiate(emptyTilePrefab, tileContainer);
        empty.name = "EmptyTile";
        emptyTile = empty.transform;

        LayoutRebuilder.ForceRebuildLayoutImmediate(tileContainer);
    }

    public bool CanMove(Transform tile)
    {
        int tileIndex = tile.GetSiblingIndex();
        int emptyIndex = emptyTile.GetSiblingIndex();
        Vector2Int tPos = IndexToGrid(tileIndex);
        Vector2Int ePos = IndexToGrid(emptyIndex);
        int dx = Mathf.Abs(tPos.x - ePos.x), dy = Mathf.Abs(tPos.y - ePos.y);
        return dx + dy == 1;
    }

    public bool TryMoveTile(Transform tile)
    {
        if (isAnimating || !CanMove(tile)) return false;
        StartCoroutine(SlideTile((RectTransform)tile));
        return true;
    }

    public Vector3 UIToWorld(RectTransform uiElement, Camera uiCamera)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, uiElement.position);
        Vector3 worldPos = uiCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0; // force particles on correct Z plane
        return worldPos;
    }

    private IEnumerator ReverseMovesCoroutine(int stepsToReverse)
    {
        if (isAnimating) yield break;

        int reversed = 0;

        while (reversed < stepsToReverse && moveHistory.Count > 0)
        {
            MoveRecord last = moveHistory.Pop();

            Transform tile = tileContainer.GetChild(last.emptyIndex);
            Transform empty = tileContainer.GetChild(last.tileIndex);

            yield return StartCoroutine(ReverseSlide((RectTransform)tile, (RectTransform)empty));

            reversed++;

            yield return new WaitForSeconds(0.05f); // optional small delay between steps
        }
    }
    public void ReverseMoves(int steps)
    {
        StartCoroutine(ReverseMovesCoroutine(steps));
    }


    private IEnumerator ReverseSlide(RectTransform tileRect, RectTransform emptyRect)
    {
        isAnimating = true;

        Vector2 start = tileRect.anchoredPosition;
        Vector2 end = emptyRect.anchoredPosition;

        gridLayoutGroup.enabled = false;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / slideDuration;
            tileRect.anchoredPosition = Vector2.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        tileRect.anchoredPosition = end;

         // ⭐ CONVERT UI POS → WORLD POS
        Vector3 worldPos = UIToWorld(tileRect, Camera.main);

        // ⭐ SPAWN BURST HERE
        TileBurstSpawner.Instance.SpawnBurst(worldPos);

        // ⭐ PLAY SQUEEZE ANIMATION
        tileRect.GetComponent<TileSqueeze>().PlaySqueeze();

        // swap back sibling indexes
        int tileIndex = tileRect.GetSiblingIndex();
        int emptyIndex = emptyTile.GetSiblingIndex();

        tileRect.SetSiblingIndex(emptyIndex);
        emptyTile.SetSiblingIndex(tileIndex);

        gridLayoutGroup.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(tileContainer);

        isAnimating = false;

        // HandlePostMove();
        AudioManager.Instance.PlayTileSlide();

        if (checker != null)
            checker.CheckCorrectTiles();
    }



    private IEnumerator SlideTile(RectTransform tileRect)
    {
        isAnimating = true;

        RectTransform emptyRect = emptyTile.GetComponent<RectTransform>();

        Vector2 start = tileRect.anchoredPosition;
        Vector2 end = emptyRect.anchoredPosition;

        gridLayoutGroup.enabled = false;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / slideDuration;
            tileRect.anchoredPosition = Vector2.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        tileRect.anchoredPosition = end;

        // ⭐ CONVERT UI POS → WORLD POS
        Vector3 worldPos = UIToWorld(tileRect, Camera.main);

        // ⭐ SPAWN BURST HERE
        TileBurstSpawner.Instance.SpawnBurst(worldPos);

        // ⭐ PLAY SQUEEZE ANIMATION
        tileRect.GetComponent<TileSqueeze>().PlaySqueeze();

        int tileIndex = tileRect.GetSiblingIndex();
        int emptyIndex = emptyTile.GetSiblingIndex();

        // ⭐ RECORD MOVE — MUST BE BEFORE INDEX SWAP
        moveHistory.Push(new MoveRecord { tileIndex = tileIndex, emptyIndex = emptyIndex });


        tileRect.SetSiblingIndex(emptyIndex);
        emptyTile.SetSiblingIndex(tileIndex);

        gridLayoutGroup.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(tileContainer);

        isAnimating = false;

        if (checker != null)
            checker.CheckCorrectTiles();


        // Notify FTUE step
        // Inside SlideTile coroutine, after move:
       if (ftueManager != null && ftueManager.IsActive())
        {
            AudioManager.Instance.PlayTileSlide();
            ftueManager.OnCorrectFTUEMove(tileIndex);
        }
        else
        {
            HandlePostMove();
        }


        // if (!ftueManager.IsActive())
        // {
        //      HandlePostMove();
        // }


    }

    // handle common post-move logic
    private void HandlePostMove()
    {
        AudioManager.Instance.PlayTileSlide();
        LevelDetailsManager.Instance.ReduceMove();

        if (checker != null)
            checker.CheckCorrectTiles();

        if (checker != null && checker.IsPuzzleComplete())
        {
            LevelDetailsManager.Instance.StopTimer();

            StarRatingManager.Instance.RewardStars();


            checker.OnPuzzleComplete();
            AudioManager.Instance.PlayLevelComplete();
            
            int currentLevel = LevelStageManager.Instance.SelectedLevel;

            if(currentLevel < 112)
            {
                LevelStageManager.Instance.UnlockNextLevel();
            }


            if(CoinManager.Instance.HasLevelRewarded(currentLevel) == false)
            {
                 StartCoroutine(DelayedCoinFlyEffect());
            }
           
            CoinManager.Instance.RewardLevel(currentLevel, 50);
            AchievementManager.Instance.CheckPuzzleAchievements(LevelDetailsManager.Instance);
            
#if UNITY_ANDROID || UNITY_IOS       
            FirebaseInit.Instance.LogLevelCompleteEvent();
#endif
            LevelPlayCounter.Instance.OnLevelCompleted();


        }
        if(checker.IsPuzzleComplete() == false &&
            LevelDetailsManager.Instance.GetMoveCount() <= 0)
        {
            int currentLevel = LevelStageManager.Instance.SelectedLevel;

            LevelDetailsManager.Instance.StopTimer();
            if (!StarRatingManager.Instance.HasLevelStarRewarded(currentLevel))
            {
                StarRatingManager.Instance.SetStars(currentLevel, 0); // no stars

            }

            if (checker != null) checker.OnGameOver();
            AudioManager.Instance.PlayGameOver();        

#if UNITY_ANDROID || UNITY_IOS
            FirebaseInit.Instance.LogLevelFailedEvent();
#endif

        }
    }

    private IEnumerator DelayedCoinFlyEffect()
    {
        yield return new WaitForSeconds(0.8f); // Adjust delay time as needed
        CoinFlyEffect.Instance.SpawnCoinsRandomExplosion(popupCoinStart, coinUITopRight, 15);
    }

    public int[] GetStateArray()
    {
        int total = gridSize * gridSize;
        int[] state = new int[total];
        for (int i = 0; i < total; i++)
        {
            Transform child = tileContainer.GetChild(i);
            if (child == emptyTile) state[i] = 0;
            else
            {
                var label = child.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                state[i] = (label != null) ? int.Parse(label.text) : -1;
            }
        }
        return state;
    }

    public Transform GetTileAtIndex(int index)
    {
        if (index < 0 || index >= tileContainer.childCount) return null;
        return tileContainer.GetChild(index);
    }

    public Transform GetEmptyTransform() => emptyTile;

    public void ReplaceWithCorrectTile(int siblingIndex, int tileNumber)
    {
        // Remove the child at the index and instantiate a correct tile there
        Transform child = tileContainer.GetChild(siblingIndex);
        Vector3 oldLocalPos = child.localPosition;
        DestroyImmediate(child.gameObject);
        GameObject newTile = Instantiate(correctTilePrefab, tileContainer);
        newTile.name = "CorrectTile_" + tileNumber;
        newTile.transform.SetSiblingIndex(siblingIndex);
        newTile.transform.localPosition = oldLocalPos;
        var label = newTile.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (label) label.text = tileNumber.ToString();

        // ⭐ Ensure TileSqueeze exists
        TileSqueeze squeeze = newTile.GetComponent<TileSqueeze>();
        if (squeeze == null)
            squeeze = newTile.AddComponent<TileSqueeze>();

        // ⭐ PLAY squeeze on the new tile
        squeeze.PlaySqueeze();
    }

    public void ReplaceWithNormalTile(int siblingIndex, int tileNumber)
    {
        Transform child = tileContainer.GetChild(siblingIndex);
        Vector3 oldLocalPos = child.localPosition;
        DestroyImmediate(child.gameObject);
        GameObject newTile = Instantiate(tilePrefab, tileContainer);
        newTile.name = "Tile_" + tileNumber;
        newTile.transform.SetSiblingIndex(siblingIndex);
        newTile.transform.localPosition = oldLocalPos;
        var label = newTile.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (label) label.text = tileNumber.ToString();

        // ⭐ Ensure TileSqueeze exists
        TileSqueeze squeeze = newTile.GetComponent<TileSqueeze>();
        if (squeeze == null)
            squeeze = newTile.AddComponent<TileSqueeze>();

        // ⭐ PLAY squeeze on the new tile
        squeeze.PlaySqueeze();
    }

    public Vector2Int IndexToGrid(int index) => new Vector2Int(index % gridSize, index / gridSize);
    public int GridToIndex(int col, int row) => row * gridSize + col;

    public void SwipeTile(Transform tile, Vector2 direction)
    {
        if (isAnimating) return;

        int tileIndex = tile.GetSiblingIndex();
        int emptyIndex = emptyTile.GetSiblingIndex();

        Vector2Int tPos = IndexToGrid(tileIndex);
        Vector2Int ePos = IndexToGrid(emptyIndex);

        // Determine if empty tile is in the swipe direction
        Vector2Int moveDir = Vector2Int.zero;

        if (direction == Vector2.right && ePos.x == tPos.x + 1 && ePos.y == tPos.y)
            moveDir = Vector2Int.right;
        else if (direction == Vector2.left && ePos.x == tPos.x - 1 && ePos.y == tPos.y)
            moveDir = Vector2Int.left;
        else if (direction == Vector2.up && ePos.y == tPos.y - 1 && ePos.x == tPos.x)
            moveDir = Vector2Int.up;
        else if (direction == Vector2.down && ePos.y == tPos.y + 1 && ePos.x == tPos.x)
            moveDir = Vector2Int.down;

        // If valid move
        if (moveDir != Vector2Int.zero)
        {
            StartCoroutine(SlideTile((RectTransform)tile));
        }
    }

    public int GetMoveCount()
    {
        return moveHistory.Count;
    }

}
