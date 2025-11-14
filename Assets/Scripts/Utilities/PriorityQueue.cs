using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data = new List<T>();
    public int Count => data.Count;

    public void Enqueue(T item)
    {
        data.Add(item);
        int ci = data.Count - 1;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (data[ci].CompareTo(data[pi]) >= 0) break;
            (data[ci], data[pi]) = (data[pi], data[ci]);
            ci = pi;
        }
    }

    public T Dequeue()
    {
        int li = data.Count - 1;
        T front = data[0];
        data[0] = data[li];
        data.RemoveAt(li);
        li--;
        int pi = 0;
        while (true)
        {
            int ci = pi * 2 + 1;
            if (ci > li) break;
            int rc = ci + 1;
            if (rc <= li && data[rc].CompareTo(data[ci]) < 0) ci = rc;
            if (data[pi].CompareTo(data[ci]) <= 0) break;
            (data[pi], data[ci]) = (data[ci], data[pi]);
            pi = ci;
        }
        return front;
    }
}
