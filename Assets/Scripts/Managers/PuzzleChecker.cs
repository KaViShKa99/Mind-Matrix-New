using UnityEngine;
using TMPro;

public class PuzzleChecker : MonoBehaviour
{
    public TileManager tileManager;
    public PopupBoxUI levelCompleteUI;
    public PopupBoxUI GameOverUI;
    [HideInInspector] public int gridSize = 3;

    public void Initialize(TileManager manager, int gridSize)
    {
        tileManager = manager;
        this.gridSize = gridSize;
    }

    // Replaces tiles with correctTilePrefab when in correct position and vice versa
    public void CheckCorrectTiles()
    {
        int total = gridSize * gridSize;
        for (int i = 0; i < total; i++)
        {
            Transform child = tileManager.GetTileAtIndex(i);
            if (child == tileManager.GetEmptyTransform()) continue;

            TextMeshProUGUI label = child.GetComponentInChildren<TextMeshProUGUI>();
            if (label == null) continue;

            if (!int.TryParse(label.text, out int tileNumber)) continue;

            bool shouldBeCorrect = tileNumber == i + 1;
            bool isCorrectPrefab = child.name.StartsWith("CorrectTile");

            if (shouldBeCorrect && !isCorrectPrefab)
            {
                tileManager.ReplaceWithCorrectTile(i, tileNumber);
                // after replacement, we need to re-run checks because sibling order changed
                CheckCorrectTiles();
                return;
            }
            else if (!shouldBeCorrect && isCorrectPrefab)
            {
                tileManager.ReplaceWithNormalTile(i, tileNumber);
                CheckCorrectTiles();
                return;
            }
        }
    }

    public bool IsPuzzleComplete()
    {
        int total = gridSize * gridSize;
        for (int i = 0; i < total; i++)
        {
            Transform child = tileManager.GetTileAtIndex(i);
            if (child == tileManager.GetEmptyTransform()) continue;

            TextMeshProUGUI label = child.GetComponentInChildren<TextMeshProUGUI>();
            if (label == null) continue;

            if (!int.TryParse(label.text, out int tileNumber)) return false;
            if (tileNumber != i + 1) return false;
        }
        return true;
    }

    public void OnPuzzleComplete()
    {
        Debug.Log("PuzzleManager: Puzzle complete");
        if (levelCompleteUI != null) levelCompleteUI.ShowPopup();

    }
    public void OnGameOver()
    {
        Debug.Log("PuzzleManager: Puzzle Over");
        if (GameOverUI != null) GameOverUI.ShowPopup();

    }
}
