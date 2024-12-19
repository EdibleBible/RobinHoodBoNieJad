using System;
using System.Collections.Generic;

public static class PrimAlgorithm
{
    public static List<Edge> FindMST(List<Edge> allEdges, List<Point> allPoints)
    {
        List<Edge> mst = new List<Edge>();

        bool[] visited = new bool[allPoints.Count];
        var priorityQueue = new PriorityQueue<Edge>();

        visited[0] = true;

        foreach (Edge edge in allEdges)
        {
            if (edge.Point1 == allPoints[0] || edge.Point2 == allPoints[0])
                priorityQueue.Enqueue(edge, CalculateCost(edge));
        }

        // Główna pętla algorytmu Prima
        while (priorityQueue.Count > 0)
        {
            var edge = priorityQueue.Dequeue();

            // Sprawdź, czy krawędź prowadzi do nieodwiedzonego wierzchołka
            Point nextPoint = !visited[allPoints.IndexOf(edge.Item.Point1)] ? edge.Item.Point1 : edge.Item.Point2;
            if (visited[allPoints.IndexOf(nextPoint)]) continue;

            // Dodaj krawędź do MST
            mst.Add(edge.Item);
            visited[allPoints.IndexOf(nextPoint)] = true;

            // Dodaj sąsiadujące krawędzie do kolejki priorytetowej
            foreach (var nextEdge in allEdges)
            {
                if ((nextEdge.Point1 == nextPoint && !visited[allPoints.IndexOf(nextEdge.Point2)]) ||
                    (nextEdge.Point2 == nextPoint && !visited[allPoints.IndexOf(nextEdge.Point1)]))
                {
                    priorityQueue.Enqueue(nextEdge, CalculateCost(nextEdge));
                }
            }
        }

        return mst;
    }

    // Funkcja do obliczania kosztu (np. odległości euklidesowej)
    public static double CalculateCost(Edge edge)
    {
        double dx = edge.Point1.X - edge.Point2.X;
        double dy = edge.Point1.Y - edge.Point2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}

