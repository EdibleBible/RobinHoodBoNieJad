using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<(T Item, double Priority)> _elements = new List<(T, double)>();

    public void Enqueue(T item, double priority)
    {
        _elements.Add((item, priority));
        _elements.Sort((x, y) => x.Priority.CompareTo(y.Priority)); // Sortowanie według priorytetu
    }

    public (T Item, double Priority) Dequeue()
    {
        if (_elements.Count == 0)
            throw new InvalidOperationException("The queue is empty.");

        var item = _elements[0];
        _elements.RemoveAt(0); // Usuwamy element o najwyższym priorytecie (najmniejszym numerze)
        return item;
    }

    public int Count => _elements.Count;

    public (T Item, double Priority) Peek()
    {
        if (_elements.Count == 0)
            throw new InvalidOperationException("The queue is empty.");

        return _elements[0];
    }
}

