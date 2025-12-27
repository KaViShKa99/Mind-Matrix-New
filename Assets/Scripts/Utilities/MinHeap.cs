using System;
using System.Collections.Generic;

public class MinHeap<T> where T : IComparable<T>
{
    private List<T> list = new List<T>();
    public int Count => list.Count;

    public void Add(T item)
    {
        list.Add(item);
        HeapifyUp(list.Count - 1);
    }

    public T RemoveMin()
    {
        if (Count == 0) throw new InvalidOperationException("Heap is empty.");

        T min = list[0];
        int lastIndex = list.Count - 1;
        list[0] = list[lastIndex];
        list.RemoveAt(lastIndex);
        HeapifyDown(0);
        return min;
    }

    private void HeapifyUp(int index)
    {
        int parent = (index - 1) / 2;
        while (index > 0 && list[index].CompareTo(list[parent]) < 0)
        {
            Swap(index, parent);
            index = parent;
            parent = (index - 1) / 2;
        }
    }

    private void HeapifyDown(int index)
    {
        int minChild;
        while (true)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            minChild = index;

            if (left < list.Count && list[left].CompareTo(list[minChild]) < 0)
                minChild = left;

            if (right < list.Count && list[right].CompareTo(list[minChild]) < 0)
                minChild = right;

            if (minChild != index)
            {
                Swap(index, minChild);
                index = minChild;
            }
            else
            {
                break;
            }
        }
    }

    private void Swap(int i, int j)
    {
        T temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
}