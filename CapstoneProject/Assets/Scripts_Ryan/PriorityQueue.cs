using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<(T item, float priority)> heap = new List<(T item, float priority)>();
    //returns index of parent
    private int parent(int index) => (index - 1) / 2;
    //returns index of left child
    private int leftChild(int index) => 2 * index + 1;
    //returns index of right child
    private int rightChild(int index) => 2 * index + 2;

    public void Enqueue(T item, float priority)
    {
        heap.Add((item, priority));
        shiftUp(heap.Count - 1);

        //DEBUG PURPOSES
        printHeap();
    }

    public T Pop()
    {
        int size = heap.Count;
        if(size == 0)
        {
            Debug.LogError("PRIORITY QUEUE: Tried popping a value when the priority queue has no elements");
        }
        T result = heap[0].item;
        heap[0] = heap[size - 1];
        heap.RemoveAt(size - 1);
        shiftDown(0);
        return result;
    }

    public T Peek()
    {
        return heap[0].item;
    }

    public int Count()
    {
        return heap.Count;
    }

    private void shiftDown(int index)
    {
        int maxIndex = index;
        int l = leftChild(index);
        if(l < heap.Count && heap[l].priority > heap[maxIndex].priority) maxIndex = l;
        int r = rightChild(index);
        if(r < heap.Count && heap[r].priority > heap[maxIndex].priority) maxIndex = r;

        if(index != maxIndex)
        {
            var temp = heap[index];
            heap[index] = heap[maxIndex];
            heap[maxIndex] = temp;
            shiftDown(maxIndex);
        }
    }

    private void shiftUp(int index)
    {
        while(index > 0 && heap[parent(index)].priority < heap[index].priority)
        {
            var temp = heap[parent(index)];
            heap[parent(index)] = heap[index];
            heap[index] = temp;
            index = parent(index);
        }
    }

    private void printHeap()
    {
        string heapString = "Print Heap: ";
        foreach(var item in heap)
        {
            heapString += item + " "; 
        }

        Debug.Log(heapString);
    }
}
