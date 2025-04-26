using System;
using System.Collections.Generic;
using System.Linq;

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

    public IEnumerable<Triangle> BowyerWatson(Dictionary<NewRoom, Point> points)
    {
        // Utwórz mapowanie z punktu na pomieszczenie
        var pointToRoom = points.ToDictionary(p => p.Value, p => p.Key);

        // Generowanie supertrójkąta obejmującego wszystkie punkty
        var supraTriangle = GenerateSupraTriangle();
        var triangulation = new HashSet<Triangle> { supraTriangle };

        // Klasyczny przebieg algorytmu Bowyer-Watsona
        foreach (var point in points)
        {
            var badTriangles = FindBadTriangles(point.Value, triangulation);
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
                var newTriangle = new Triangle(point.Value, edge.Point1, edge.Point2);
                triangulation.Add(newTriangle);
            }
        }

        // Usuwanie trójkątów zawierających wierzchołki supertrójkąta
        triangulation.RemoveWhere(o => o.Vertices.Any(v => supraTriangle.Vertices.Contains(v)));

        // Filtrowanie trójkątów – pozostawiamy te, które łączą tylko pomieszczenia, które nie dotykają się.
        var filteredTriangles = new HashSet<Triangle>();

        foreach (var triangle in triangulation)
        {
            // Wyciągamy unikalne pomieszczenia powiązane z wierzchołkami trójkąta
            HashSet<NewRoom> triangleRooms = new HashSet<NewRoom>();
            foreach (var vertex in triangle.Vertices)
            {
                if (pointToRoom.TryGetValue(vertex, out NewRoom room))
                    triangleRooms.Add(room);
            }

            // Jeśli trójkąt zawiera wierzchołki z więcej niż jednego pomieszczenia,
            // sprawdzamy czy którakolwiek para pomieszczeń jest ze sobą w kontakcie.
            bool valid = true;
            NewRoom[] roomArray = triangleRooms.ToArray();
            /*for (int i = 0; i < roomArray.Length && valid; i++)
            {
                for (int j = i + 1; j < roomArray.Length; j++)
                {
                    bool contact = false;
                    // Sprawdzamy, czy w którymś z pomieszczeń w ContactRooms wpis dla drugiego pomieszczenia wskazuje kontakt.
                    if (roomArray[i].ContactRooms.TryGetValue(roomArray[j], out contact) ||
                        roomArray[j].ContactRooms.TryGetValue(roomArray[i], out contact))
                    {
                        if (contact)
                        {
                            valid = false;
                            break;
                        }
                    }
                }
            }*/

            if (valid)
                filteredTriangles.Add(triangle);
        }

        return filteredTriangles;
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