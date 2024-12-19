using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneratorController : MonoBehaviour
{
    public GridData MainGridData;
    public RoomGanerateSetting RoomGanerateSetting;
    public DelaunayTriangulator Triangulator;
    public List<Point> AllPoints;
    public List<Edge> AllEdges;
    public List<Edge> SelectedEdges;

    //Debug 
    private Dictionary<GameObject,GridCellData> allObject = new Dictionary<GameObject, GridCellData>();

    public void GenerateDebugMesh()
    {
        allObject.Clear();
        foreach (var cell in MainGridData.AllGridCell)
        {
            var createdCell = new GameObject($"cell nr x:{cell.Coordinate.x} y:{cell.Coordinate.y}");
            createdCell.transform.parent = transform;
            createdCell.transform.position = cell.Position;

            MeshFilter meshFilter = createdCell.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            MeshRenderer meshRenderer = createdCell.AddComponent<MeshRenderer>();
            if (cell.GridCellType == E_GridCellType.Room)
            {
                meshRenderer.material = MainGridData.RoomCellMaterial;
            }
            else
            {
                meshRenderer.material = MainGridData.EmptyCellMaterial;
            }

            // Zastosowanie skali dla komórki
            createdCell.transform.localScale = MainGridData.CellScale;

            allObject.Add(createdCell, cell);
        }

    }

    public void SetupPassDebugMesh()
    {
        foreach (var cell in allObject)
        {
            if (cell.Value.GridCellType == E_GridCellType.Pass)
            {
                MeshRenderer meshRenderer = cell.Key.GetComponent<MeshRenderer>();
                meshRenderer.material = MainGridData.PassCellMaterial;
            }
        }
    }

    public List<Triangle> GenerateTriangulation()
    {
        Triangulator = new DelaunayTriangulator();
        List<Point> listOfPoints = new List<Point>();

        // Tworzenie punktów
        foreach (Room roomCentre in RoomGanerateSetting.CreatedRoom)
        {
            Point newPoint = new Point(roomCentre.cetroid.x, roomCentre.cetroid.z);
            newPoint.SetPointRoom(roomCentre);
            listOfPoints.Add(newPoint);
        }

        // Generowanie triangulacji
        List<Triangle> allTriangle = Triangulator.BowyerWatson(listOfPoints).ToList();


        AllEdges = GetEdgesFromTriangles(allTriangle);
        AllPoints = GetPointsFromTriangles(allTriangle);
        return allTriangle;
    }

    public List<Edge> GetEdgesFromTriangles(List<Triangle> triangles)
    {
        List<Edge> edges = new List<Edge>();

        foreach (var triangle in triangles)
        {
            // Dodaj krawędzie do listy
            AddEdgeIfNotExist(edges, new Edge(triangle.Vertices[0], triangle.Vertices[1]));
            AddEdgeIfNotExist(edges, new Edge(triangle.Vertices[1], triangle.Vertices[2]));
            AddEdgeIfNotExist(edges, new Edge(triangle.Vertices[2], triangle.Vertices[0]));
        }

        return edges;
    }

    private void AddEdgeIfNotExist(List<Edge> edges, Edge edge)
    {
        // Dodaj krawędź do listy, jeśli jeszcze jej tam nie ma (uwzględniając odwrotne kierunki)
        if (!edges.Contains(edge))
        {
            edges.Add(edge);
        }
    }

    public List<Point> GetPointsFromTriangles(List<Triangle> triangles)
    {
        List<Point> points = new List<Point>();

        foreach (var triangle in triangles)
        {
            // Dodaj punkty wierzchołków trójkąta do listy, jeśli jeszcze ich tam nie ma
            AddPointIfNotExist(points, triangle.Vertices[0]);
            AddPointIfNotExist(points, triangle.Vertices[1]);
            AddPointIfNotExist(points, triangle.Vertices[2]);
        }

        return points;
    }

    private void AddPointIfNotExist(List<Point> points, Point point)
    {
        // Dodaj punkt do listy, jeśli jeszcze go tam nie ma
        if (!points.Contains(point))
        {
            points.Add(point);
        }
    }

    public List<Edge> GetUsedEgdes(List<Edge> allEdge, List<Point> allPoints)
    {
        List<Edge> usedEdge = PrimAlgorithm.FindMST(allEdge, allPoints);
        int additionalEdges = 0;

        if(!RoomGanerateSetting.UseAdditionalEdges)
        {
            foreach (Edge edge in usedEdge)
            {
                var edgePoint1Vecotr2 = new Vector2((float)edge.Point1.X, (float)edge.Point1.Y);
                var edgePoint2Vecotr2 = new Vector2((float)edge.Point2.X, (float)edge.Point2.Y);


                var room1 = RoomGanerateSetting.CreatedRoom.Where(x => x.cetroid.x == edgePoint1Vecotr2.x && x.cetroid.z == edgePoint1Vecotr2.y).FirstOrDefault();
                var room2 = RoomGanerateSetting.CreatedRoom.Where(x => x.cetroid.x == edgePoint2Vecotr2.x && x.cetroid.z == edgePoint2Vecotr2.y).FirstOrDefault();
                edge.SetEdgeRoom(room1, room2);
                var point = FindConnectionPosition(room1, room2);
                edge.SetEnterExitRoom(point.entryPoint, point.exitPoint);
            }

            return usedEdge;
        }

        foreach (Edge edge in allEdge)
        {
            if (additionalEdges >= RoomGanerateSetting.AdditionSelectedEdges)
                break;

            if (usedEdge.Contains(edge))
                continue;

            var randomNumber = Random.Range(0, 100);

            if (randomNumber > RoomGanerateSetting.ChanceToSelectEdge)
                continue;

            usedEdge.Add(edge);
            additionalEdges++;
        }

        foreach(Edge edge in usedEdge)
        {
            var edgePoint1Vecotr2 = new Vector2((float)edge.Point1.X, (float)edge.Point1.Y);
            var edgePoint2Vecotr2 = new Vector2((float)edge.Point2.X, (float)edge.Point2.Y);


            var room1 = RoomGanerateSetting.CreatedRoom.Where(x => x.cetroid.x == edgePoint1Vecotr2.x && x.cetroid.z == edgePoint1Vecotr2.y).FirstOrDefault();
            var room2 = RoomGanerateSetting.CreatedRoom.Where(x => x.cetroid.x == edgePoint2Vecotr2.x && x.cetroid.z == edgePoint2Vecotr2.y).FirstOrDefault();
            var point = FindConnectionPosition(room1,room2);
            edge.SetEnterExitRoom(point.entryPoint,point.exitPoint);
        }

        return usedEdge;
    }

    public (GridCellData entryPoint, GridCellData exitPoint) FindConnectionPosition(Room room1, Room room2)
    {
        // Oblicz centroidy pokojów jako punkty odniesienia
        Vector2 room1Center = new Vector2(room1.cetroid.x, room1.cetroid.z);
        Vector2 room2Center = new Vector2(room2.cetroid.x, room2.cetroid.z);

        // Oblicz różnicę współrzędnych
        float dx = room2Center.x - room1Center.x;
        float dy = room2Center.y - room1Center.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        // Znormalizowany kierunek
        float nx = dx / distance;
        float ny = dy / distance;

        // Wyznacz punkty docelowe na podstawie kierunku
        Vector2 entryPointCandidate = room1Center + new Vector2(nx, ny) * 0.5f; // Punkt w kierunku wyjścia
        Vector2 exitPointCandidate = room2Center - new Vector2(nx, ny) * 0.5f;  // Punkt w kierunku wejścia

        // Znajdź najbliższe komórki na krawędziach pokojów
        GridCellData entryPoint = FindClosestEdgeCellToDirection(entryPointCandidate, room1);
        GridCellData exitPoint = FindClosestEdgeCellToDirection(exitPointCandidate, room2);

        return (entryPoint, exitPoint);
    }

    private GridCellData FindClosestEdgeCellToDirection(Vector2 candidatePoint, Room room)
    {
        GridCellData closestCell = null;
        float minDistance = float.MaxValue;

        foreach (var cell in room.CellInRoom)
        {
            // Sprawdź, czy komórka jest na krawędzi pokoju
            if (IsEdgeCell(cell, room))
            {
                float cellX = cell.Coordinate.x;
                float cellY = cell.Coordinate.y;

                // Oblicz odległość od punktu kandydata do komórki
                float distance = Vector2.Distance(candidatePoint, new Vector2(cellX, cellY));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCell = cell;
                }
            }
        }

        return closestCell;
    }

    private bool IsEdgeCell(GridCellData cell, Room room)
    {
        // Krawędź pokoju - znajdź minimalne i maksymalne wartości x i y
        int minX = room.CellInRoom.Min(c => c.Coordinate.x);
        int maxX = room.CellInRoom.Max(c => c.Coordinate.x);
        int minY = room.CellInRoom.Min(c => c.Coordinate.y);
        int maxY = room.CellInRoom.Max(c => c.Coordinate.y);

        // Komórka jest na krawędzi, jeśli jej współrzędne są równe minimalnym lub maksymalnym
        return cell.Coordinate.x == minX || cell.Coordinate.x == maxX ||
               cell.Coordinate.y == minY || cell.Coordinate.y == maxY;
    }

}
