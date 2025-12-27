
using UnityEngine;
using System.Collections.Generic;

public class FTUEManager : MonoBehaviour
{
    public TileManager tileManager;
    public FTUEHandAnimator hand;
    public FTUEHighlight highlight;

    private AStarSolver solver;
    private List<int[]> solution;
    private int stepIndex = 1;
    private bool active = false;

    public void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.MuteBackgroundMusic();
        }
    }

    public void StartFTUE()
    {
        active = true;

        solver = new AStarSolver(tileManager.gridSize);

        int total = tileManager.gridSize * tileManager.gridSize;
        int[] start = tileManager.GetStateArray();

        int[] goal = new int[total];
        for (int i = 0; i < total - 1; i++) goal[i] = i + 1;
        goal[total - 1] = 0;

        solution = solver.Solve(start, goal, 1f);

        if (solution == null || solution.Count < 2)
        {
            Debug.LogError("FTUE: No solution found");
            return;
        }

        stepIndex = 1;
        ShowStep();
    }

    void ShowStep()
    {
        if (!active || stepIndex >= solution.Count) return;

        int[] prev = solution[stepIndex - 1];
        int[] next = solution[stepIndex];

        int tileIndex = System.Array.IndexOf(next, 0);

        Transform tile = tileManager.GetTileAtIndex(tileIndex);
        Transform empty = tileManager.GetEmptyTransform();

        if (!tile || !empty) return;

        // highlight.Show(tile);
        hand.Play(tile.position, empty.position);
    }

    public void OnCorrectFTUEMove(int movedTileIndex)
    {
        if (!active) return;

        int expected = System.Array.IndexOf(solution[stepIndex], 0);
        if (movedTileIndex != expected) return;

        hand.Stop();
        // highlight.Hide();

        stepIndex++;

        if (stepIndex >= solution.Count)
        {
            active = false;
            FTUEController.Instance.OnFTUEFinished();
        }
        else
        {
            ShowStep();
        }
    }

    public bool IsActive() => active;
}
