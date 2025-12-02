using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintSystem : MonoBehaviour
{
    public TileManager tileManager;
    public GameObject arrowPrefab;
    public float arrowDuration = 0.8f;
    public int maxHintSteps = 3;

    public PopupBoxUI adWatchPopup;

    [HideInInspector] public int gridSize = 3;

    public void Initialize(TileManager manager, int gridSize)
    {
        tileManager = manager;
        this.gridSize = gridSize;
    }

    public void ShowHintsAfterCoinSpent()
    {
        StartCoroutine(HintCoroutine());

    }

    public void ShowHintsAfterAd()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowRewarded(() =>
            {
                StartCoroutine(HintCoroutine());

            });
        }
        else
        {
            Debug.LogWarning("⚠️ AdsManager not found — showing hint directly.");
            StartCoroutine(HintCoroutine());
        }

    }



    private IEnumerator HintCoroutine()
    {
        int total = gridSize * gridSize;
        int[] start = tileManager.GetStateArray();

        int[] goal = new int[total];
        for (int i = 0; i < total - 1; i++) goal[i] = i + 1;
        goal[total - 1] = 0;

        List<int[]> path = SolveAStar(start, goal);
        if (path == null)
        {
            Debug.Log("HintSystem: No path found");
            yield break;
        }

        int shown = 0;
        for (int i = 1; i < path.Count && shown < maxHintSteps; i++, shown++)
        {
            int[] prev = path[i - 1];
            int[] next = path[i];
            int emptyPrev = System.Array.IndexOf(prev, 0);
            int emptyNext = System.Array.IndexOf(next, 0);

            Vector2Int p1 = IndexToGrid(emptyPrev);
            Vector2Int p2 = IndexToGrid(emptyNext);
            Vector2Int dir = p2 - p1;

            int movedTileIndex = emptyNext;
            Transform tile = tileManager.GetTileAtIndex(movedTileIndex);
            if (tile == null) continue;

            GameObject arrow = Instantiate(arrowPrefab, tile);
            arrow.transform.localPosition = Vector3.zero;

            float angle = 0;
            if (dir == Vector2Int.up) angle = 0;
            else if (dir == Vector2Int.down) angle = 180;
            else if (dir == Vector2Int.left) angle = -90;
            else if (dir == Vector2Int.right) angle = 90;
            arrow.transform.localEulerAngles = new Vector3(0, 0, angle);

            Animator anim = arrow.GetComponent<Animator>();
            if (anim != null) anim.Play("ArrowFloat");

            Destroy(arrow, arrowDuration);
            yield return new WaitForSeconds(arrowDuration + 0.1f);
        }
    }

    // A* solver (same as original)
    private List<int[]> SolveAStar(int[] start, int[] goal)
    {
        var open = new PriorityQueue<Node>();
        var visited = new HashSet<string>();
        Node startNode = new Node(start, null, 0, Heuristic(start, goal));
        open.Enqueue(startNode);

        while (open.Count > 0)
        {
            Node current = open.Dequeue();
            string hash = string.Join(",", current.state);
            if (visited.Contains(hash)) continue;
            visited.Add(hash);

            if (IsGoal(current.state, goal)) return BuildPath(current);

            foreach (var neighbor in GetNeighbors(current.state))
            {
                string nhash = string.Join(",", neighbor);
                if (visited.Contains(nhash)) continue;
                Node next = new Node(neighbor, current, current.g + 1, Heuristic(neighbor, goal));
                open.Enqueue(next);
            }
        }
        return null;
    }

    private bool IsGoal(int[] a, int[] b)
    {
        for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
        return true;
    }

    private List<int[]> BuildPath(Node node)
    {
        var path = new List<int[]>();
        while (node != null)
        {
            path.Insert(0, node.state);
            node = node.parent;
        }
        return path;
    }

    private IEnumerable<int[]> GetNeighbors(int[] state)
    {
        int index = System.Array.IndexOf(state, 0);
        Vector2Int pos = IndexToGrid(index);
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in dirs)
        {
            Vector2Int newPos = pos + dir;
            if (newPos.x < 0 || newPos.x >= gridSize || newPos.y < 0 || newPos.y >= gridSize) continue;
            int newIndex = newPos.y * gridSize + newPos.x;
            int[] ns = (int[])state.Clone();
            ns[index] = ns[newIndex];
            ns[newIndex] = 0;
            yield return ns;
        }
    }

    private int Heuristic(int[] state, int[] goal)
    {
        int h = 0;
        for (int i = 0; i < state.Length; i++)
        {
            if (state[i] == 0) continue;
            int val = state[i] - 1;
            int x1 = i % gridSize, y1 = i / gridSize;
            int x2 = val % gridSize, y2 = val / gridSize;
            h += Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        }
        return h;
    }

    private class Node : System.IComparable<Node>
    {
        public int[] state;
        public Node parent;
        public int g;
        public int h;
        public int f => g + h;
        public Node(int[] s, Node p, int g, int h) { state = s; parent = p; this.g = g; this.h = h; }
        public int CompareTo(Node other) => f.CompareTo(other.f);
    }

    // helper
    private Vector2Int IndexToGrid(int index) => new Vector2Int(index % gridSize, index / gridSize);
}
