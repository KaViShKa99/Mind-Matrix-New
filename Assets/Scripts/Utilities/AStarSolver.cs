// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class AStarSolver
// {
//     private int gridSize;

//     public AStarSolver(int gridSize)
//     {
//         this.gridSize = gridSize;
//     }

//     public List<int[]> Solve(int[] start, int[] goal)
//     {
//         MinHeap<Node> open = new MinHeap<Node>();
//         HashSet<string> visited = new HashSet<string>();

//         Node startNode = new Node(start, null, 0, Heuristic(start, goal));
//         open.Add(startNode);

//         while (open.Count > 0)
//         {
//             Node current = open.RemoveMin();
//             string hash = string.Join(",", current.state);

//             if (visited.Contains(hash)) continue;
//             visited.Add(hash);

//             if (IsGoal(current.state, goal)) return BuildPath(current);

//             foreach (var neighbor in GetNeighbors(current.state))
//             {
//                 string nhash = string.Join(",", neighbor);
//                 if (visited.Contains(nhash)) continue;
//                 Node next = new Node(neighbor, current, current.g + 1, Heuristic(neighbor, goal));
//                 open.Add(next);
//             }
//         }

//         return null;
//     }

//     private bool IsGoal(int[] a, int[] b)
//     {
//         for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
//         return true;
//     }

//     private List<int[]> BuildPath(Node node)
//     {
//         var path = new List<int[]>();
//         while (node != null)
//         {
//             path.Insert(0, node.state);
//             node = node.parent;
//         }
//         return path;
//     }

//     private IEnumerable<int[]> GetNeighbors(int[] state)
//     {
//         int index = Array.IndexOf(state, 0);
//         Vector2Int pos = new Vector2Int(index % gridSize, index / gridSize);
//         Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

//         foreach (var dir in dirs)
//         {
//             Vector2Int newPos = pos + dir;
//             if (newPos.x < 0 || newPos.x >= gridSize || newPos.y < 0 || newPos.y >= gridSize) continue;
//             int newIndex = newPos.y * gridSize + newPos.x;
//             int[] ns = (int[])state.Clone();
//             ns[index] = ns[newIndex];
//             ns[newIndex] = 0;
//             yield return ns;
//         }
//     }

//     private int Heuristic(int[] state, int[] goal)
//     {
//         int h = 0;
//         for (int i = 0; i < state.Length; i++)
//         {
//             if (state[i] == 0) continue;
//             int val = state[i] - 1;
//             int x1 = i % gridSize, y1 = i / gridSize;
//             int x2 = val % gridSize, y2 = val / gridSize;
//             h += Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
//         }
//         return h;
//     }

//     public class Node : IComparable<Node>
//     {
//         public int[] state;
//         public Node parent;
//         public int g;
//         public int h;
//         public int f => g + h;

//         public Node(int[] s, Node p, int g, int h) { state = s; parent = p; this.g = g; this.h = h; }
//         public int CompareTo(Node other) => f.CompareTo(other.f);
//     }
// }
using System;
using System.Collections.Generic;
using UnityEngine;

// --- StateData Class (Optimized for Hashing) ---
public class StateData
{
    public int[] state;

    public StateData(int[] s) { state = s; }

    // Efficient Hash Code Calculation: Replaces slow string.Join()
    public override int GetHashCode()
    {
        unchecked // Allows overflow
        {
            int hash = 17;
            for (int i = 0; i < state.Length; i++)
            {
                // Simple combination function
                hash = hash * 31 + state[i];
            }
            return hash;
        }
    }

    // Efficient Equality Check: Checks array content directly
    public override bool Equals(object obj)
    {
        if (obj is StateData other)
        {
            if (state.Length != other.state.Length) return false;
            for (int i = 0; i < state.Length; i++)
            {
                if (state[i] != other.state[i]) return false;
            }
            return true;
        }
        return false;
    }
}


// --- AStarSolver Class ---
public class AStarSolver
{
    private int gridSize;
    private float weight; // For Weighted A* (W=1 for optimal A*)

    public AStarSolver(int gridSize)
    {
        this.gridSize = gridSize;
        this.weight = 1f; 
    }

    // Solve signature with heuristicWeight: W=1 for optimal A*, W>1 for fast sub-optimal
    public List<int[]> Solve(int[] start, int[] goal, float heuristicWeight = 1f)
    {
        this.weight = heuristicWeight;

        MinHeap<Node> open = new MinHeap<Node>(); 
        HashSet<StateData> visited = new HashSet<StateData>();

        StateData startData = new StateData(start);
        int h = Heuristic(start, goal);
        Node startNode = new Node(startData, null, 0, h, weight);
        
        open.Add(startNode);
        visited.Add(startData); 

        while (open.Count > 0)
        {
            Node current = open.RemoveMin();

            if (IsGoal(current.stateData.state, goal)) return BuildPath(current);

            foreach (var neighborState in GetNeighbors(current.stateData.state))
            {
                StateData neighborData = new StateData(neighborState);

                if (visited.Contains(neighborData)) continue;

                int neighborH = Heuristic(neighborState, goal);
                Node next = new Node(neighborData, current, current.g + 1, neighborH, weight);

                visited.Add(neighborData); 
                open.Add(next);
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
            path.Insert(0, node.stateData.state);
            node = node.parent;
        }
        return path;
    }

    private IEnumerable<int[]> GetNeighbors(int[] state)
    {
        int index = Array.IndexOf(state, 0);
        Vector2Int pos = new Vector2Int(index % gridSize, index / gridSize);
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

    // Manhattan + Linear Conflict Heuristic
    private int Heuristic(int[] state, int[] goal)
    {
        int h = 0;
        int totalTiles = state.Length;

        // 1. Manhattan Distance Calculation
        for (int i = 0; i < totalTiles; i++)
        {
            if (state[i] == 0) continue;
            int val = state[i];
            int goalIndex = Array.IndexOf(goal, val);
            
            int x1 = i % gridSize, y1 = i / gridSize;
            int x2 = goalIndex % gridSize, y2 = goalIndex / gridSize;
            
            h += Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        }

        // 2. Linear Conflict Heuristic (Crucial for 4x4 performance)
        if (gridSize > 2)
        {
            h += 2 * GetLinearConflicts(state, goal);
        }

        return h;
    }

    private int GetLinearConflicts(int[] state, int[] goal)
    {
        int conflicts = 0;
        
        for (int row = 0; row < gridSize; row++)
        {
            conflicts += CheckConflictsInLine(state, goal, row, true);
        }

        for (int col = 0; col < gridSize; col++)
        {
            conflicts += CheckConflictsInLine(state, goal, col, false);
        }

        return conflicts;
    }

    private int CheckConflictsInLine(int[] state, int[] goal, int lineIndex, bool isRow)
    {
        int conflicts = 0;
        int[] lineTiles = new int[gridSize]; 
        
        for (int i = 0; i < gridSize; i++)
        {
            int index = isRow ? (lineIndex * gridSize + i) : (i * gridSize + lineIndex);
            lineTiles[i] = state[index];
        }

        for (int i = 0; i < gridSize - 1; i++)
        {
            int tile1 = lineTiles[i];
            if (tile1 == 0) continue; 

            int goalIndex1 = Array.IndexOf(goal, tile1);
            int goalLine1 = isRow ? (goalIndex1 / gridSize) : (goalIndex1 % gridSize);

            if (goalLine1 == lineIndex)
            {
                for (int j = i + 1; j < gridSize; j++)
                {
                    int tile2 = lineTiles[j];
                    if (tile2 == 0) continue; 
                    
                    int goalIndex2 = Array.IndexOf(goal, tile2);
                    int goalLine2 = isRow ? (goalIndex2 / gridSize) : (goalIndex2 % gridSize);

                    if (goalLine2 == lineIndex)
                    {
                        int goalLinePos1 = isRow ? (goalIndex1 % gridSize) : (goalIndex1 / gridSize);
                        int goalLinePos2 = isRow ? (goalIndex2 % gridSize) : (goalIndex2 / gridSize);

                        if (goalLinePos1 > goalLinePos2)
                        {
                            conflicts++;
                        }
                    }
                }
            }
        }
        return conflicts;
    }

    public class Node : IComparable<Node>
    {
        public StateData stateData; 
        public Node parent;
        public int g; 
        public int h; 
        public float f; 

        public Node(StateData s, Node p, int g, int h, float weight) 
        { 
            stateData = s; 
            parent = p; 
            this.g = g; 
            this.h = h; 
            this.f = g + weight * h; 
        }

        public int CompareTo(Node other) => f.CompareTo(other.f);
    }
}