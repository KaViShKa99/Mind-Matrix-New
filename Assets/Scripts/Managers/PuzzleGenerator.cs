using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;   // Required for LayoutRebuilder


public class PuzzleGenerator : MonoBehaviour
{
    public TileManager tileManager;
    [HideInInspector] public int gridSize = 3;

    public void Initialize(TileManager manager, int gridSize)
    {
        this.tileManager = manager;
        this.gridSize = gridSize;
    }

    public void GenerateTilesInOrder()
    {
        if (tileManager == null) { Debug.LogError("PuzzleGenerator: TileManager missing"); return; }
        tileManager.GenerateGrid();
    }

    // Shuffle by making valid random moves from solved state (always solvable)
    public void ShuffleTiles(int moveCount)
    {
        if (tileManager == null) return;
        int total = gridSize * gridSize;
        if (total < 4 || moveCount <= 0) return;

        // Ensure layout is generated
        LayoutRebuilder.ForceRebuildLayoutImmediate(tileManager.tileContainer);

        int emptyIndex = total - 1;
        int lastMoved = -1;
        System.Random rng = new System.Random();

        for (int step = 0; step < moveCount; step++)
        {
            List<int> movable = new List<int>();
            Vector2Int emptyPos = tileManager.IndexToGrid(emptyIndex);

            for (int i = 0; i < total - 1; i++)
            {
                Vector2Int p = tileManager.IndexToGrid(i);
                int dx = Mathf.Abs(p.x - emptyPos.x);
                int dy = Mathf.Abs(p.y - emptyPos.y);
                if (dx + dy == 1 && i != lastMoved) movable.Add(i);
            }

            if (movable.Count == 0) break;
            int chosen = movable[rng.Next(movable.Count)];
            Transform chosenTile = tileManager.GetTileAtIndex(chosen);
            // swap sibling indices
            int tempIndex = chosenTile.GetSiblingIndex();
            Transform empty = tileManager.GetEmptyTransform();
            chosenTile.SetSiblingIndex(empty.GetSiblingIndex());
            empty.SetSiblingIndex(tempIndex);

            lastMoved = emptyIndex;
            emptyIndex = chosen;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(tileManager.tileContainer);
    }
}
