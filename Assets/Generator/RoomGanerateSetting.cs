using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public struct RoomGanerateSetting
{
    public Vector2Int MaxRoomSize;
    public Vector2Int MinRoomSize;
    public int RoomCount;
    public int MinRoomDistance;
    public int MaxAttempts;
    public List<Room> CreatedRoom;
    public bool UseAdditionalEdges;
    [Range(0, 25)] public int AdditionSelectedEdges;
    [Range(0, 100)] public int ChanceToSelectEdge;

    public void CreateRoomsOnGrid(GridData gridData)
    {

        CreatedRoom = new List<Room>();
        int attempt = 0;

        for (int i = 0; i < RoomCount; i++)
        {
            // Próba wygenerowania pokoju
            Room room = GenerateRoom(gridData, ref attempt);
            // Jeśli po kilku próbach nie udało się stworzyć pokoju, przerywamy
            if (room == null)
            {
                Debug.LogWarning("Unable to generate room after maximum attempts.");
                break; // Jeśli nie udało się wygenerować pokoju, przerwij
            }

            if (room.CellInRoom.Count > 0)
            {
                room.RoomID = i;
                room.cetroid = room.RoomCentroid();
                CreatedRoom.Add(room);
            }
        }
    }

    private Room GenerateRoom(GridData gridData, ref int attempt)
    {
        attempt++;

        if (attempt > MaxAttempts)
        {
            // Jeśli osiągnięto limit prób, zwróć null
            return null;
        }

        int width = UnityEngine.Random.Range(MinRoomSize.x, MaxRoomSize.x);
        int height = UnityEngine.Random.Range(MinRoomSize.y, MaxRoomSize.y);

        Room room = new Room();
        room.XAxisSize = width;
        room.YAxisSize = height;
        room.CellInRoom = new List<GridCellData>();

        bool roomIsGenerated = false;
        while (!roomIsGenerated)
        {
            int x = UnityEngine.Random.Range(0, gridData.CurrAxisSize.x - width);
            int y = UnityEngine.Random.Range(0, gridData.CurrAxisSize.y - height);

            if (CanPlaceRoom(gridData, new Vector2Int(x, y), width, height))
            {
                for (int i = x; i < x + width; i++)
                {
                    for (int j = y; j < y + height; j++)
                    {
                        GridCellData selcetedCell = gridData.FindSelectedGridCell(i, j);
                        selcetedCell.GridCellType = E_GridCellType.Room;
                        room.CellInRoom.Add(selcetedCell);
                    }
                }
            }
            else if (attempt > MaxAttempts)
            {
                return null; // Jeśli próba przekroczyła maksymalną liczbę, zakończ generowanie
            }

            roomIsGenerated = true;
        }
        return room;
    }

    // Funkcja walidująca, czy pokój zmieści się w siatce
    private bool CanPlaceRoom(GridData gridData, Vector2Int start, int width, int height)
    {
        for (int i = start.x - MinRoomDistance; i < start.x + width + MinRoomDistance; i++)
        {
            for (int j = start.y - MinRoomDistance; j < start.y + height + MinRoomDistance; j++)
            {
                if (i < 0 || j < 0 || i >= gridData.CurrAxisSize.x || j >= gridData.CurrAxisSize.y)
                    continue;

                GridCellData cell = gridData.AllGridCell.Find(c => c.Coordinate.x == i && c.Coordinate.y == j);

                // Jeśli komórka jest już zajęta przez pomieszczenie, nie można tu utworzyć pokoju
                if (cell != null && cell.GridCellType == E_GridCellType.Room)
                    return false;
            }
        }
        return true;
    }

    public void MakeTraingulateBetweenRoom()
    {
        List<Vector3> roomCentroid = CreatedRoom.Select(room => room.RoomCentroid()).ToList();
    }

}

public class Triangle
{
    public Point[] Vertices { get; } = new Point[3];
    public Point Circumcenter { get; private set; }
    public double RadiusSquared;

    public IEnumerable<Triangle> TrianglesWithSharedEdge
    {
        get
        {
            var neighbors = new HashSet<Triangle>();
            foreach (var vertex in Vertices)
            {
                var trianglesWithSharedEdge = vertex.AdjacentTriangles.Where(o =>
                {
                    return o != this && SharesEdgeWith(o);
                });
                neighbors.UnionWith(trianglesWithSharedEdge);
            }

            return neighbors;
        }
    }

    public Triangle(Point point1, Point point2, Point point3)
    {
        // In theory this shouldn't happen, but it was at one point so this at least makes sure we're getting a
        // relatively easily-recognised error message, and provides a handy breakpoint for debugging.
        if (point1 == point2 || point1 == point3 || point2 == point3)
        {
            throw new ArgumentException("Must be 3 distinct points");
        }

        if (!IsCounterClockwise(point1, point2, point3))
        {
            Vertices[0] = point1;
            Vertices[1] = point3;
            Vertices[2] = point2;
        }
        else
        {
            Vertices[0] = point1;
            Vertices[1] = point2;
            Vertices[2] = point3;
        }

        Vertices[0].AdjacentTriangles.Add(this);
        Vertices[1].AdjacentTriangles.Add(this);
        Vertices[2].AdjacentTriangles.Add(this);
        UpdateCircumcircle();
    }

    private void UpdateCircumcircle()
    {
        // https://codefound.wordpress.com/2013/02/21/how-to-compute-a-circumcircle/#more-58
        // https://en.wikipedia.org/wiki/Circumscribed_circle
        var p0 = Vertices[0];
        var p1 = Vertices[1];
        var p2 = Vertices[2];
        var dA = p0.X * p0.X + p0.Y * p0.Y;
        var dB = p1.X * p1.X + p1.Y * p1.Y;
        var dC = p2.X * p2.X + p2.Y * p2.Y;

        var aux1 = (dA * (p2.Y - p1.Y) + dB * (p0.Y - p2.Y) + dC * (p1.Y - p0.Y));
        var aux2 = -(dA * (p2.X - p1.X) + dB * (p0.X - p2.X) + dC * (p1.X - p0.X));
        var div = (2 * (p0.X * (p2.Y - p1.Y) + p1.X * (p0.Y - p2.Y) + p2.X * (p1.Y - p0.Y)));

        if (div == 0)
        {
            throw new DivideByZeroException();
        }

        var center = new Point(aux1 / div, aux2 / div);
        Circumcenter = center;
        RadiusSquared = (center.X - p0.X) * (center.X - p0.X) + (center.Y - p0.Y) * (center.Y - p0.Y);
    }

    private bool IsCounterClockwise(Point point1, Point point2, Point point3)
    {
        var result = (point2.X - point1.X) * (point3.Y - point1.Y) -
            (point3.X - point1.X) * (point2.Y - point1.Y);
        return result > 0;
    }

    public bool SharesEdgeWith(Triangle triangle)
    {
        var sharedVertices = Vertices.Where(o => triangle.Vertices.Contains(o)).Count();
        return sharedVertices == 2;
    }

    public bool IsPointInsideCircumcircle(Point point)
    {
        var d_squared = (point.X - Circumcenter.X) * (point.X - Circumcenter.X) +
            (point.Y - Circumcenter.Y) * (point.Y - Circumcenter.Y);
        return d_squared < RadiusSquared;
    }
}
[Serializable]
public class Edge
{
    public Point Point1 { get; private set; }
    public Point Point2 { get; private set; }

    public Room Point1Room { get; private set; }
    public Room Point2Room { get; private set; }

    public GridCellData entryGridCell;
    public GridCellData exitGridCell;

    public Edge(Point point1, Point point2)
    {
        Point1 = point1;
        Point2 = point2;
    }

    public void SetEdgeRoom(Room point1Room, Room point2Room)
    {
        Point1Room = point1Room;
        Point2Room = point2Room;
    }

    public void SetEnterExitRoom(GridCellData enterPoint, GridCellData exitPoint)
    {
        entryGridCell = enterPoint;
        exitGridCell = exitPoint;

        entryGridCell.GridCellType = E_GridCellType.Pass;
        exitGridCell.GridCellType = E_GridCellType.Pass;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != GetType()) return false;
        var edge = obj as Edge;

        var samePoints = Point1 == edge.Point1 && Point2 == edge.Point2;
        var samePointsReversed = Point1 == edge.Point2 && Point2 == edge.Point1;
        return samePoints || samePointsReversed;
    }

    public override int GetHashCode()
    {
        int hCode = (int)Point1.X ^ (int)Point1.Y ^ (int)Point2.X ^ (int)Point2.Y;
        return hCode.GetHashCode();
    }
}
[Serializable]
public class Point
{
    /// <summary>
    /// Used only for generating a unique ID for each instance of this class that gets generated
    /// </summary>
    private static int _counter;

    /// <summary>
    /// Used for identifying an instance of a class; can be useful in troubleshooting when geometry goes weird
    /// (e.g. when trying to identify when Triangle objects are being created with the same Point object twice)
    /// </summary>
    private readonly int _instanceId = _counter++;

    public double X { get; }
    public double Y { get; }
    public HashSet<Triangle> AdjacentTriangles { get; } = new HashSet<Triangle>();

    public Room PointRoom { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public void SetPointRoom(Room room)
    {
        PointRoom = room;
    }

    public override string ToString()
    {
        // Simple way of seeing what's going on in the debugger when investigating weirdness
        return $"{nameof(Point)} {_instanceId} {X:0.##}@{Y:0.##}";
    }
}
public class DelaunayTriangulator
{
    private double MaxX { get; set; }
    private double MaxY { get; set; }
    private IEnumerable<Triangle> border = new List<Triangle>();

    public IEnumerable<Point> GeneratePoints(int amount, double maxX, double maxY)
    {
        MaxX = maxX;
        MaxY = maxY;

        // TODO make more beautiful
        var point0 = new Point(0, 0);
        var point1 = new Point(0, MaxY);
        var point2 = new Point(MaxX, MaxY);
        var point3 = new Point(MaxX, 0);
        var points = new List<Point>() { point0, point1, point2, point3 };
        var tri1 = new Triangle(point0, point1, point2);
        var tri2 = new Triangle(point0, point2, point3);
        border = new List<Triangle>() { tri1, tri2 };

        var random = new System.Random();
        for (int i = 0; i < amount - 4; i++)
        {
            var pointX = random.NextDouble() * MaxX;
            var pointY = random.NextDouble() * MaxY;
            points.Add(new Point(pointX, pointY));
        }

        return points;
    }

    public IEnumerable<Triangle> BowyerWatson(IEnumerable<Point> points)
    {
        // Generowanie supertrójkąta obejmującego wszystkie punkty
        var supraTriangle = GenerateSupraTriangle();
        var triangulation = new HashSet<Triangle> { supraTriangle };

        foreach (var point in points)
        {
            var badTriangles = FindBadTriangles(point, triangulation);
            var polygon = FindHoleBoundaries(badTriangles);

            // Usuwanie złych trójkątów
            foreach (var triangle in badTriangles)
            {
                foreach (var vertex in triangle.Vertices)
                {
                    vertex.AdjacentTriangles.Remove(triangle);
                }
            }
            triangulation.RemoveWhere(o => badTriangles.Contains(o));

            // Dodawanie nowych trójkątów
            foreach (var edge in polygon)
            {
                var triangle = new Triangle(point, edge.Point1, edge.Point2);
                triangulation.Add(triangle);
            }
        }

        // Usuwanie wszystkich trójkątów zawierających wierzchołki supertrójkąta
        triangulation.RemoveWhere(o => o.Vertices.Any(v => supraTriangle.Vertices.Contains(v)));

        return triangulation;
    }
    private List<Edge> FindHoleBoundaries(ISet<Triangle> badTriangles)
    {
        var edges = new List<Edge>();
        foreach (var triangle in badTriangles)
        {
            edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
            edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
            edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));
        }
        var grouped = edges.GroupBy(o => o);
        var boundaryEdges = edges.GroupBy(o => o).Where(o => o.Count() == 1).Select(o => o.First());
        return boundaryEdges.ToList();
    }
    private Triangle GenerateSupraTriangle()
    {
        //   1  -> maxX
        //  / \
        // 2---3
        // |
        // v maxY
        var margin = 500;
        var point1 = new Point(0.5 * MaxX, -2 * MaxX - margin);
        var point2 = new Point(-2 * MaxY - margin, 2 * MaxY + margin);
        var point3 = new Point(2 * MaxX + MaxY + margin, 2 * MaxY + margin);
        return new Triangle(point1, point2, point3);
    }

    private ISet<Triangle> FindBadTriangles(Point point, HashSet<Triangle> triangles)
    {
        var badTriangles = triangles.Where(o => o.IsPointInsideCircumcircle(point));
        return new HashSet<Triangle>(badTriangles);
    }
}

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

