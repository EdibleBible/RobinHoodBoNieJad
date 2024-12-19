using CustomGrid;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using CodeMonkey.Utils;
using Random = UnityEngine.Random;
using System.Drawing;
using NUnit.Framework;

[ExecuteAlways]
public class MapGeneratorController : MonoBehaviour
{
    public GridOptions MainGridData;
    public RoomGanerateSetting RoomGanerateSetting;


    public DelaunayTriangulator Triangulator;
    public List<Point> AllPoints;
    public List<Edge> AllEdges;
    public List<Edge> SelectedEdges;

    //GridCellDataGrid (mainInfoGrid)
    public CustomGrid.Grid<GridCellData> MainInfoGrid;

    //Debug 
    private Dictionary<GameObject, GridCellData> allObject = new Dictionary<GameObject, GridCellData>();
    public void GenerateGrid()
    {
        var randomSize = MainGridData.RandomizeGridSize();
        GenerateGrid(randomSize.x, randomSize.y);
    }
    public void GenerateGrid(int gridX, int gridY)
    {
        // Tworzenie głównej siatki
        MainInfoGrid = new CustomGrid.Grid<GridCellData>(gridX, gridY, MainGridData.cellScale, transform.position, (CustomGrid.Grid<GridCellData> g, int x, int y) =>
        {
            GridCellData cellData = new GridCellData();
            cellData.SetCoordinate(x, y);

            // Obliczanie pozycji (start od 0, 0)
            Vector3 position = new Vector3(x * MainGridData.cellScale, 0, y * MainGridData.cellScale);
            cellData.SetPosition(position);
            return cellData;
        });
    }
    public void DebugGridMesh()
    {
        Vector2Int gridSize = MainInfoGrid.GeneratedGridSize();
        allObject.Clear();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var cell = MainInfoGrid.GetValue(x, y);
                if (cell == null) continue;

                // Tworzenie komórki
                var createdCell = new GameObject($"cell nr x:{cell.Coordinate.x} y:{cell.Coordinate.y}");
                createdCell.transform.parent = transform;

                // Przypisanie pozycji
                createdCell.transform.position = cell.Position;

                // Dodanie komponentów wizualnych
                MeshFilter meshFilter = createdCell.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

                MeshRenderer meshRenderer = createdCell.AddComponent<MeshRenderer>();
                meshRenderer.material = cell.GridCellType == E_GridCellType.Room
                    ? MainGridData.RoomCellMaterial
                    : MainGridData.EmptyCellMaterial;

                // Skalowanie
                createdCell.transform.localScale = Vector3.one * MainGridData.cellScale;

                // Dodanie do słownika dla debugowania
                allObject.Add(createdCell, cell);
            }
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
    public void SetupHallwayDebugMesh()
    {
        foreach (var cell in allObject)
        {
            if (cell.Value.GridCellType == E_GridCellType.Hallway)
            {
                MeshRenderer meshRenderer = cell.Key.GetComponent<MeshRenderer>();
                meshRenderer.material = MainGridData.HallwayMaterial;
            }
        }
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

        if (!RoomGanerateSetting.UseAdditionalEdges)
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

        foreach (Edge edge in usedEdge)
        {
            var edgePoint1Vecotr2 = new Vector2((float)edge.Point1.X, (float)edge.Point1.Y);
            var edgePoint2Vecotr2 = new Vector2((float)edge.Point2.X, (float)edge.Point2.Y);


            var room1 = RoomGanerateSetting.CreatedRoom.Where(x => x.cetroid.x == edgePoint1Vecotr2.x && x.cetroid.z == edgePoint1Vecotr2.y).FirstOrDefault();
            var room2 = RoomGanerateSetting.CreatedRoom.Where(x => x.cetroid.x == edgePoint2Vecotr2.x && x.cetroid.z == edgePoint2Vecotr2.y).FirstOrDefault();
            var point = FindConnectionPosition(room1, room2);
            edge.SetEnterExitRoom(point.entryPoint, point.exitPoint);
        }

        return usedEdge;
    }

    public void RoomPathFind()
    {
        if (MainInfoGrid == null)
            return;

        List<GridCellData> roomCell = new List<GridCellData>();

        foreach (var room in RoomGanerateSetting.CreatedRoom)
        {
            foreach (var cell in room.CellInRoom)
            {
                if (cell.GridCellType == E_GridCellType.Pass)
                    continue;

                roomCell.Add(cell);
            }
        }

        Pathfinding pathfinding = new Pathfinding(MainInfoGrid.GetWidth(), MainInfoGrid.GetHeight(), MainGridData.cellScale, transform.position, roomCell);

        foreach (Edge edge in AllEdges)
        {

            List<PathNode> pathNodeCell = pathfinding.FindPath(edge.EntryGridCell.Coordinate.x, edge.EntryGridCell.Coordinate.y, edge.ExitGridCell.Coordinate.x, edge.ExitGridCell.Coordinate.y);

            foreach (var element in pathNodeCell)
            {
                if ((element.X == edge.EntryGridCell.Coordinate.x && element.Y == edge.EntryGridCell.Coordinate.y) || (element.X == edge.ExitGridCell.Coordinate.x && element.Y == edge.ExitGridCell.Coordinate.y))
                    continue;

                GridCellData toAdd = MainInfoGrid.GetValue(element.X, element.Y);
                toAdd.GridCellType = E_GridCellType.Hallway;
            }
        }
    }


    public void _testPathFind(GridCellData startCell, GridCellData endCell)
    {
        if (MainInfoGrid == null)
            return;

        List<GridCellData> roomCell = new List<GridCellData>();

        foreach (var room in RoomGanerateSetting.CreatedRoom)
        {
            foreach (var cell in room.CellInRoom)
            {
                if (cell.GridCellType == E_GridCellType.Pass)
                    continue;

                roomCell.Add(cell);
            }
        }

        var size = MainInfoGrid.GeneratedGridSize();
        Pathfinding pathfinding = new Pathfinding(size.x, size.y, MainGridData.cellScale, transform.position, roomCell);
        var list = pathfinding.FindPath(startCell.Coordinate.x, startCell.Coordinate.y, endCell.Coordinate.x, endCell.Coordinate.y);

        List<GridCellData> hallwaygridcell = new List<GridCellData>();

        foreach (var element in list)
        {
            if ((element.X == startCell.Coordinate.x && element.Y == startCell.Coordinate.y) || (element.X == endCell.Coordinate.x && element.Y == endCell.Coordinate.y))
                continue;

            GridCellData toAdd = MainInfoGrid.GetValue(element.X, element.Y);
            toAdd.GridCellType = E_GridCellType.Hallway;
            hallwaygridcell.Add(toAdd);
        }

    }

}



