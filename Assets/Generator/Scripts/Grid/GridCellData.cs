using CustomGrid;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;


[Serializable]
public class GridCellData
{
    public Grid<GridCellData> GridHolder;
    public E_GridCellType GridCellType;
    public Vector2Int Coordinate;
    public Vector3 Position;
    public Vector2 CellSize;

    public Dictionary<E_GridCellReferenceType, GridCellData> neighbors =
        new Dictionary<E_GridCellReferenceType, GridCellData>();

    public GridCellData AxisCell { get; }
    public bool IsRoomCorner;
    public int RoomID { get; private set; } = -1;

    // Nowe pole - obiekty znajdujące się w tej komórce
    public List<GameObject> CellObjects = new();

    public void SetAxisCell(GridCellData end)
    {
        throw new NotImplementedException();
    }

    public void SetRoomID(int roomId)
    {
        RoomID = roomId;
    }

    public void SetCellSize(Vector2 size)
    {
        CellSize = size;
    }

    public void SetCellSize(float cellScale)
    {
        Vector2 scale = new Vector2(cellScale, cellScale);
        SetCellSize(scale);
    }

    public void SetCoordinate(Vector2Int coordinate)
    {
        Coordinate = coordinate;
    }

    public void SetCoordinate(int x, int y)
    {
        SetCoordinate(new Vector2Int(x, y));
    }

    public void SetPosition(Vector3 position)
    {
        Position = position;
    }

    public void SetPosition(float x, float y, float z)
    {
        SetPosition(new Vector3(x, y, z));
    }

    public void SetGridParent(Grid<GridCellData> grid)
    {
        GridHolder = grid;
        var neighborCells = grid.GetNeighbourList(this, false);

        neighbors.Clear();

        foreach (var neighbor in neighborCells)
        {
            Vector2Int delta = neighbor.Coordinate - this.Coordinate;

            if (delta == new Vector2Int(0, 1))
            {
                neighbors.Add(E_GridCellReferenceType.E_up, neighbor);
            }
            else if (delta == new Vector2Int(0, -1))
            {
                neighbors.Add(E_GridCellReferenceType.E_down, neighbor);
            }
            else if (delta == new Vector2Int(-1, 0))
            {
                neighbors.Add(E_GridCellReferenceType.E_left, neighbor);
            }
            else if (delta == new Vector2Int(1, 0))
            {
                neighbors.Add(E_GridCellReferenceType.E_right, neighbor);
            }
        }
    }

    public void SetIsRoomCorner()
    {
        IsRoomCorner = true;
        GridCellType = E_GridCellType.RoomCorner;
    }

    public void SetIsRoomBorder()
    {
        GridCellType = E_GridCellType.RoomBorder;
    }

    public void DetectObjectsInCell(LayerMask detectionLayerMask)
    {
        CellObjects.Clear(); // Czyścimy stare dane

        Vector3 center = Position;
        Vector3 halfExtents = new Vector3(CellSize.x / 2f, 5f, CellSize.y / 2f); // 5f wysokości, możesz zmienić

        Collider[] colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity, detectionLayerMask);

        foreach (var collider in colliders)
        {
            GameObject obj = collider.gameObject;
            AddObject(obj);
        }
    }

    public List<Transform> FastDetectObjectInCell(LayerMask detectionLayerMask)
    {
        List<Transform> returnList = new List<Transform>();

        Vector3 center = Position;
        Vector3 halfExtents = new Vector3(CellSize.x / 2f, 5f, CellSize.y / 2f); // 5f wysokości, możesz zmienić

        Collider[] colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity, detectionLayerMask);

        foreach (var collider in colliders)
        {
            GameObject obj = collider.gameObject;
            returnList.Add(obj.transform);
        }

        return returnList;
    }

    public List<Matrix4x4> FlorMatrix4x4(Vector2 segmentSize)
    {
        List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();

        // Obliczamy liczbę segmentów wzdłuż osi X i Z
        int segmentCountX = Mathf.CeilToInt(CellSize.x / segmentSize.x);
        int segmentCountZ = Mathf.CeilToInt(CellSize.y / segmentSize.y);

        // Obliczamy skalę pojedynczego segmentu
        Vector3 segmentScale = new Vector3(segmentSize.x, 1f, segmentSize.y);

        for (int x = 0; x < segmentCountX; x++)
        {
            for (int z = 0; z < segmentCountZ; z++)
            {
                // Obliczamy pozycję każdego segmentu
                Vector3 segmentPosition = Position + new Vector3(
                    x * segmentSize.x - CellSize.x / 2 + segmentSize.x / 2,
                    0f,
                    z * segmentSize.y - CellSize.y / 2 + segmentSize.y / 2
                );

                Quaternion rotation = Quaternion.identity; // Brak rotacji dla podłogi
                Matrix4x4 newMatrix = Matrix4x4.TRS(segmentPosition, rotation, segmentScale);

                matrix4X4s.Add(newMatrix);
            }
        }

        return matrix4X4s;
    }

    public List<Matrix4x4> RoomWallMatrix4s4(Vector2 segmentSize)
    {
        List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();

        matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_up,
            neighbors[E_GridCellReferenceType.E_up], Quaternion.Euler(0, 270f, 0f),
            segmentSize));
        matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_down,
            neighbors[E_GridCellReferenceType.E_down],
            Quaternion.Euler(0, 90f, 0f), segmentSize));
        matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_left,
            neighbors[E_GridCellReferenceType.E_left], Quaternion.Euler(0, 180, 0),
            segmentSize));
        matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_right,
            neighbors[E_GridCellReferenceType.E_right], Quaternion.Euler(0, 0, 0),
            segmentSize));


        return matrix4X4s;
    }

    private List<Matrix4x4> GenerateSelectedRoomSide(E_GridCellReferenceType referenceType,
        GridCellData selectedReferenceGrid, Quaternion rotation, Vector2 segmentSize)
    {
        List<Matrix4x4> result = new List<Matrix4x4>();

        if (!ShouldGenerateWalls(selectedReferenceGrid))
            return result;

        int wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
        float scale = CellSize.y / wallCount / segmentSize.y;

        for (int i = 0; i < wallCount; i++)
        {
            Vector3 position = CalculatePosition(referenceType, i, segmentSize, scale);
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, new Vector3(1f, 1f, scale));
            result.Add(matrix);
        }
        
        for (int i = 0; i < wallCount; i++)
        {
            Vector3 position = CalculatePosition(referenceType, i, segmentSize, scale);
            Matrix4x4 matrix = Matrix4x4.TRS(position + new Vector3(0,2,0), rotation, new Vector3(1f, 1f, scale));
            result.Add(matrix);
        }

        return result;
    }

    private bool ShouldGenerateWalls(GridCellData selectedReferenceGrid)
    {
        if (selectedReferenceGrid.GridCellType == E_GridCellType.Empty)
            return true;

        return false;
        /*bool isCornerCase = IsRoomCorner && (GridCellType == E_GridCellType.Pass ||
                                             GridCellType == E_GridCellType.SpawnPass);

        if (isCornerCase)
        {
            return selectedReferenceGrid != AxisCell && (selectedReferenceGrid.GridCellType == E_GridCellType.Empty ||
                                                         selectedReferenceGrid.GridCellType == E_GridCellType.Hallway);
        }

        return (selectedReferenceGrid.GridCellType == E_GridCellType.Empty ||
                selectedReferenceGrid.GridCellType == E_GridCellType.Hallway) &&
               GridCellType != E_GridCellType.Pass &&
               GridCellType != E_GridCellType.SpawnPass;*/
    }

    private Vector3 CalculatePosition(E_GridCellReferenceType referenceType, int index, Vector2 segmentSize,
        float scale)
    {
        Vector3 offset = Vector3.zero;

        switch (referenceType)
        {
            case E_GridCellReferenceType.E_up:
                offset = new Vector3((index * segmentSize.x * scale) - (0.5f * (CellSize.x - 1)), 0, CellSize.x / 2f);
                break;
            case E_GridCellReferenceType.E_down:
                offset = new Vector3((index * segmentSize.x * scale) - (0.5f * (CellSize.x - 1)), 0, -CellSize.x / 2f);
                break;
            case E_GridCellReferenceType.E_left:
                offset = new Vector3(-CellSize.x / 2f, 0,
                    (index * segmentSize.y * scale) + (segmentSize.y / 2 * scale) - (0.5f * CellSize.y));
                break;
            case E_GridCellReferenceType.E_right:
                offset = new Vector3(CellSize.x / 2f, 0,
                    (index * segmentSize.y * scale) + (segmentSize.y / 2 * scale) - (0.5f * CellSize.y));
                break;
        }

        return Position + offset;
    }

    public void AddObject(GameObject obj)
    {
        if (!CellObjects.Contains(obj))
        {
            CellObjects.Add(obj);
        }
    }

    public List<GameObject> GetAllObjects()
    {
        return CellObjects;
    }

    public GridCellData GetDoorExitCell()
    {
        GameObject passObject = null;
        bool isBreakingWall = false;

        // Szukamy drzwi lub przełamywalnej ściany
        foreach (var obj in GetAllObjects())
        {
            if (obj.TryGetComponent(out DoorController doorController))
            {
                passObject = doorController.gameObject;
                break;
            }

            if (obj.TryGetComponent(out BreakingWallController breakingWallController))
            {
                passObject = breakingWallController.gameObject;
                isBreakingWall = true;
                break;
            }
        }

        if (passObject == null)
        {
            Debug.LogWarning("No passable object found in cell.");
            return null;
        }

        // Kierunek wyjścia zależnie od typu obiektu
        Vector3 doorDirection = isBreakingWall
            ? -passObject.transform.right
            : -passObject.transform.right;

        // Znajdź najlepszy kierunek w odniesieniu do grida
        E_GridCellReferenceType bestDirection = GetClosestDirection(doorDirection);

        // Sprawdź, czy w tym kierunku istnieje sąsiad
        if (neighbors.TryGetValue(bestDirection, out GridCellData neighborCell))
        {
            return neighborCell;
        }
        else
        {
            Debug.LogWarning("No neighbor in the exit direction!");
            return null;
        }
    }

    private E_GridCellReferenceType GetClosestDirection(Vector3 dir)
    {
        // Zerujemy oś Y, bo interesują nas tylko X i Z (2D)
        dir.y = 0;
        dir.Normalize();

        float dotUp = Vector3.Dot(dir, Vector3.forward);
        float dotDown = Vector3.Dot(dir, Vector3.back);
        float dotRight = Vector3.Dot(dir, Vector3.right);
        float dotLeft = Vector3.Dot(dir, Vector3.left);

        float max = Mathf.Max(dotUp, dotDown, dotRight, dotLeft);

        if (max == dotUp)
            return E_GridCellReferenceType.E_up;
        else if (max == dotDown)
            return E_GridCellReferenceType.E_down;
        else if (max == dotRight)
            return E_GridCellReferenceType.E_right;
        else
            return E_GridCellReferenceType.E_left;
    }
}

public enum E_GridCellReferenceType
{
    E_up,
    E_down,
    E_left,
    E_right
}

#region Old

/*public void SetAxisCell(GridCellData axisCell)
{
    AxisCell = axisCell;
}

public void SetIsRoomCorner()
{
    IsRoomCorner = true;
}

public void SetCellSize(Vector2 size)
{
    CellSize = size;
}

public void SetCellSize(float cellScale)
{
    Vector2 scale = new Vector2(cellScale, cellScale);
    SetCellSize(scale);
}

public void SetCoordinate(Vector2Int coordinate)
{
    Coordinate = coordinate;
}

public void SetCoordinate(int x, int y)
{
    SetCoordinate(new Vector2Int(x, y));
}

public void SetPosition(Vector3 postion)
{
    Position = postion;
}

public void SetPosition(float x, float y, float z)
{
    SetPosition(new Vector3(x, y, z));
}

public void SetGridParent(Grid<GridCellData> grid)
{
    GridHolder = grid;
    var neigbourCell = grid.GetNeighbourList(this, false);

    DownN = null;
    UpN = null;
    LeftN = null;
    RightN = null;

    foreach (var neighbor in neigbourCell)
    {
        Vector2Int delta = neighbor.Coordinate - this.Coordinate;

        if (delta == new Vector2Int(0, 1))
        {
            UpN = neighbor;
        }
        else if (delta == new Vector2Int(0, -1))
        {
            DownN = neighbor;
        }
        else if (delta == new Vector2Int(-1, 0))
        {
            LeftN = neighbor;
        }
        else if (delta == new Vector2Int(1, 0))
        {
            RightN = neighbor;
        }
    }
}

public List<Matrix4x4> RoomWallMatrix4s4(Vector2 segmentSize)
{
    List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();

    matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_up, UpN, Quaternion.Euler(0, 270f, 0f),
        segmentSize));
    matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_down, DownN,
        Quaternion.Euler(0, 90f, 0f), segmentSize));
    matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_left, LeftN, Quaternion.Euler(0, 180, 0),
        segmentSize));
    matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_right, RightN, Quaternion.Euler(0, 0, 0),
        segmentSize));


    return matrix4X4s;
}

private IEnumerable<Matrix4x4> GeneratePassRoomSide(Vector2 segmentSize)
{
    List<Matrix4x4> result = new List<Matrix4x4>();

    if (GridCellType == E_GridCellType.Pass || GridCellType == E_GridCellType.SecretPass ||
        GridCellType == E_GridCellType.SpawnPass)
    {
        var wallCount = 0f;
        var scale = 0f;
        if (AxisCell == UpN)
        {
            wallCount = math.max(0, (int)(CellSize.x / segmentSize.x));
            scale = CellSize.x / wallCount / segmentSize.x;

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = CalculatePosition(E_GridCellReferenceType.E_up, i, segmentSize, scale);
                Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                result.Add(newMatrix);
            }
        }
        else if (AxisCell == DownN)
        {
            wallCount = math.max(0, (int)(CellSize.x / segmentSize.x));
            scale = CellSize.x / wallCount / segmentSize.x;

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = CalculatePosition(E_GridCellReferenceType.E_down, i, segmentSize, scale);
                Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                result.Add(newMatrix);
            }
        }
        else if (AxisCell == LeftN)
        {
            wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
            scale = CellSize.y / wallCount / segmentSize.y;

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = CalculatePosition(E_GridCellReferenceType.E_left, i, segmentSize, scale);
                Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                result.Add(newMatrix);
            }
        }
        else if (AxisCell == RightN)
        {
            wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
            scale = CellSize.y / wallCount / segmentSize.y;

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = CalculatePosition(E_GridCellReferenceType.E_right, i, segmentSize, scale);
                Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                result.Add(newMatrix);
            }
        }
    }

    return result;
}

private List<Matrix4x4> GenerateSelectedRoomSide(E_GridCellReferenceType referenceType,
    GridCellData selectedReferenceGrid, Quaternion rotation, Vector2 segmentSize)
{
    List<Matrix4x4> result = new List<Matrix4x4>();

    if (!ShouldGenerateWalls(selectedReferenceGrid))
        return result;

    int wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
    float scale = CellSize.y / wallCount / segmentSize.y;

    for (int i = 0; i < wallCount; i++)
    {
        Vector3 position = CalculatePosition(referenceType, i, segmentSize, scale);
        Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, new Vector3(1f, 1f, scale));
        result.Add(matrix);
    }

    return result;
}

private bool ShouldGenerateWalls(GridCellData selectedReferenceGrid)
{
    bool isCornerCase = IsRoomCorner && (GridCellType == E_GridCellType.Pass ||
                                         GridCellType == E_GridCellType.SecretPass ||
                                         GridCellType == E_GridCellType.SpawnPass);

    if (isCornerCase)
    {
        return selectedReferenceGrid != AxisCell && (selectedReferenceGrid.GridCellType == E_GridCellType.Empty ||
                                                     selectedReferenceGrid.GridCellType == E_GridCellType.Hallway);
    }

    return (selectedReferenceGrid.GridCellType == E_GridCellType.Empty ||
            selectedReferenceGrid.GridCellType == E_GridCellType.Hallway) &&
           GridCellType != E_GridCellType.Pass && GridCellType != E_GridCellType.SecretPass &&
           GridCellType != E_GridCellType.SpawnPass;
}

private Vector3 CalculatePosition(E_GridCellReferenceType referenceType, int index, Vector2 segmentSize,
    float scale)
{
    Vector3 offset = Vector3.zero;

    switch (referenceType)
    {
        case E_GridCellReferenceType.E_up:
            offset = new Vector3((index * segmentSize.x * scale) - (0.5f * (CellSize.x - 1)), 0, CellSize.x / 2f);
            break;
        case E_GridCellReferenceType.E_down:
            offset = new Vector3((index * segmentSize.x * scale) - (0.5f * (CellSize.x - 1)), 0, -CellSize.x / 2f);
            break;
        case E_GridCellReferenceType.E_left:
            offset = new Vector3(-CellSize.x / 2f, 0,
                (index * segmentSize.y * scale) + (segmentSize.y / 2 * scale) - (0.5f * CellSize.y));
            break;
        case E_GridCellReferenceType.E_right:
            offset = new Vector3(CellSize.x / 2f, 0,
                (index * segmentSize.y * scale) + (segmentSize.y / 2 * scale) - (0.5f * CellSize.y));
            break;
    }

    return Position + offset;
}

public List<Matrix4x4> HallwayWallMatrix4x4(Vector2 segmentSize)
{
    try
    {
        List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();

        var wallCount = math.max(0, (int)(CellSize.x / segmentSize.x));
        var scale = CellSize.x / wallCount / segmentSize.x;

        AddWallToMatrix(UpN, wallCount, segmentSize, scale, true, ref matrix4X4s);
        AddWallToMatrix(DownN, wallCount, segmentSize, scale, false, ref matrix4X4s);

        wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
        scale = CellSize.y / wallCount / segmentSize.y;

        AddWallToMatrix(LeftN, wallCount, segmentSize, scale, true, ref matrix4X4s);
        AddWallToMatrix(RightN, wallCount, segmentSize, scale, false, ref matrix4X4s);

        return matrix4X4s;
    }
    catch (Exception e)
    {
        Debug.LogError("Exception caught: " + e.Message);
        return null;
    }
}

private void AddWallToMatrix(GridCellData node, int wallCount, Vector2 segmentSize, float scale, bool isVertical,
    ref List<Matrix4x4> matrix4X4s)
{
    // Sprawdzamy, czy node jest null lub jeśli GridCellType jest Empty
    if (node == null || node.GridCellType == E_GridCellType.Empty)
    {
        for (int i = 0; i < wallCount; i++)
        {
            Vector3 t = Position;
            if (isVertical)
            {
                t += new Vector3(
                    (-CellSize.x / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0,
                    (node == UpN ? CellSize.y / 2f : -CellSize.y / 2f));
            }
            else
            {
                t += new Vector3(
                    (node == LeftN ? -CellSize.x / 2f : CellSize.x / 2f), 0,
                    (-CellSize.y / 2f) + (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
            }

            Quaternion r = Quaternion.Euler(0f, (isVertical ? 90f : 0f), 0f);
            Vector3 s = new Vector3(1f, 1f, scale);

            Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
            matrix4X4s.Add(newMatrix);
        }
    }
}

internal IEnumerable<Matrix4x4> RoomPassWallMatrix4x4(Vector2 segmentSize)
{
    List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();
    matrix4X4s.AddRange(GeneratePassRoomSide(segmentSize));
    return matrix4X4s;
}

public IEnumerable<Matrix4x4> FlorMatrix4x4(Vector2 segmentSize)
{
    List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();

    // Obliczamy liczbę segmentów wzdłuż osi X i Z
    int segmentCountX = Mathf.CeilToInt(CellSize.x / segmentSize.x);
    int segmentCountZ = Mathf.CeilToInt(CellSize.y / segmentSize.y);

    // Obliczamy skalę pojedynczego segmentu
    Vector3 segmentScale = new Vector3(segmentSize.x, 1f, segmentSize.y);

    for (int x = 0; x < segmentCountX; x++)
    {
        for (int z = 0; z < segmentCountZ; z++)
        {
            // Obliczamy pozycję każdego segmentu
            Vector3 segmentPosition = Position + new Vector3(
                x * segmentSize.x - CellSize.x / 2 + segmentSize.x / 2,
                0f,
                z * segmentSize.y - CellSize.y / 2 + segmentSize.y / 2
            );

            Quaternion rotation = Quaternion.identity; // Brak rotacji dla podłogi
            Matrix4x4 newMatrix = Matrix4x4.TRS(segmentPosition, rotation, segmentScale);

            matrix4X4s.Add(newMatrix);
        }
    }

    return matrix4X4s;
}

public List<GridCellData> ReturnNeighbour()
{
    List<GridCellData> gridCellData = new List<GridCellData>
    {
        UpN,
        DownN,
        LeftN,
        RightN
    };
    return gridCellData;
}

public enum E_GridCellReferenceType
{
    E_up,
    E_down,
    E_left,
    E_right
}

public void SetupCellRoom(Room room)
{
    if (GridCellType == E_GridCellType.Room && connectedRoom == null)
    {
        connectedRoom = room;
    }
}

public Room GetConnectedRoom()
{
    return connectedRoom;
}*/ /*public void SetAxisCell(GridCellData axisCell)
{
    AxisCell = axisCell;
}

public void SetIsRoomCorner()
{
    IsRoomCorner = true;
}

public void SetCellSize(Vector2 size)
{
    CellSize = size;
}

public void SetCellSize(float cellScale)
{
    Vector2 scale = new Vector2(cellScale, cellScale);
    SetCellSize(scale);
}

public void SetCoordinate(Vector2Int coordinate)
{
    Coordinate = coordinate;
}

public void SetCoordinate(int x, int y)
{
    SetCoordinate(new Vector2Int(x, y));
}

public void SetPosition(Vector3 postion)
{
    Position = postion;
}

public void SetPosition(float x, float y, float z)
{
    SetPosition(new Vector3(x, y, z));
}

public void SetGridParent(Grid<GridCellData> grid)
{
    GridHolder = grid;
    var neigbourCell = grid.GetNeighbourList(this, false);

    DownN = null;
    UpN = null;
    LeftN = null;
    RightN = null;

    foreach (var neighbor in neigbourCell)
    {
        Vector2Int delta = neighbor.Coordinate - this.Coordinate;

        if (delta == new Vector2Int(0, 1))
        {
            UpN = neighbor;
        }
        else if (delta == new Vector2Int(0, -1))
        {
            DownN = neighbor;
        }
        else if (delta == new Vector2Int(-1, 0))
        {
            LeftN = neighbor;
        }
        else if (delta == new Vector2Int(1, 0))
        {
            RightN = neighbor;
        }
    }
}

public List<Matrix4x4> RoomWallMatrix4s4(Vector2 segmentSize)
{
    List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();

    matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_up, UpN, Quaternion.Euler(0, 270f, 0f),
        segmentSize));
    matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_down, DownN,
        Quaternion.Euler(0, 90f, 0f), segmentSize));
    matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_left, LeftN, Quaternion.Euler(0, 180, 0),
        segmentSize));
    matrix4X4s.AddRange(GenerateSelectedRoomSide(E_GridCellReferenceType.E_right, RightN, Quaternion.Euler(0, 0, 0),
        segmentSize));


    return matrix4X4s;
}

private IEnumerable<Matrix4x4> GeneratePassRoomSide(Vector2 segmentSize)
{
    List<Matrix4x4> result = new List<Matrix4x4>();

    if (GridCellType == E_GridCellType.Pass || GridCellType == E_GridCellType.SecretPass ||
        GridCellType == E_GridCellType.SpawnPass)
    {
        var wallCount = 0f;
        var scale = 0f;
        if (AxisCell == UpN)
        {
            wallCount = math.max(0, (int)(CellSize.x / segmentSize.x));
            scale = CellSize.x / wallCount / segmentSize.x;

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = CalculatePosition(E_GridCellReferenceType.E_up, i, segmentSize, scale);
                Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                result.Add(newMatrix);
            }
        }
        else if (AxisCell == DownN)
        {
            wallCount = math.max(0, (int)(CellSize.x / segmentSize.x));
            scale = CellSize.x / wallCount / segmentSize.x;

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = CalculatePosition(E_GridCellReferenceType.E_down, i, segmentSize, scale);
                Quaternion r = Quaternion.Euler(0f, 90f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                result.Add(newMatrix);
            }
        }
        else if (AxisCell == LeftN)
        {
            wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
            scale = CellSize.y / wallCount / segmentSize.y;

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = CalculatePosition(E_GridCellReferenceType.E_left, i, segmentSize, scale);
                Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                result.Add(newMatrix);
            }
        }
        else if (AxisCell == RightN)
        {
            wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
            scale = CellSize.y / wallCount / segmentSize.y;

            for (int i = 0; i < wallCount; i++)
            {
                Vector3 t = CalculatePosition(E_GridCellReferenceType.E_right, i, segmentSize, scale);
                Quaternion r = Quaternion.Euler(0f, 0f, 0f);
                Vector3 s = new Vector3(1f, 1f, scale);

                Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
                result.Add(newMatrix);
            }
        }
    }

    return result;
}

private List<Matrix4x4> GenerateSelectedRoomSide(E_GridCellReferenceType referenceType,
    GridCellData selectedReferenceGrid, Quaternion rotation, Vector2 segmentSize)
{
    List<Matrix4x4> result = new List<Matrix4x4>();

    if (!ShouldGenerateWalls(selectedReferenceGrid))
        return result;

    int wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
    float scale = CellSize.y / wallCount / segmentSize.y;

    for (int i = 0; i < wallCount; i++)
    {
        Vector3 position = CalculatePosition(referenceType, i, segmentSize, scale);
        Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, new Vector3(1f, 1f, scale));
        result.Add(matrix);
    }

    return result;
}

private bool ShouldGenerateWalls(GridCellData selectedReferenceGrid)
{
    bool isCornerCase = IsRoomCorner && (GridCellType == E_GridCellType.Pass ||
                                         GridCellType == E_GridCellType.SecretPass ||
                                         GridCellType == E_GridCellType.SpawnPass);

    if (isCornerCase)
    {
        return selectedReferenceGrid != AxisCell && (selectedReferenceGrid.GridCellType == E_GridCellType.Empty ||
                                                     selectedReferenceGrid.GridCellType == E_GridCellType.Hallway);
    }

    return (selectedReferenceGrid.GridCellType == E_GridCellType.Empty ||
            selectedReferenceGrid.GridCellType == E_GridCellType.Hallway) &&
           GridCellType != E_GridCellType.Pass && GridCellType != E_GridCellType.SecretPass &&
           GridCellType != E_GridCellType.SpawnPass;
}

private Vector3 CalculatePosition(E_GridCellReferenceType referenceType, int index, Vector2 segmentSize,
    float scale)
{
    Vector3 offset = Vector3.zero;

    switch (referenceType)
    {
        case E_GridCellReferenceType.E_up:
            offset = new Vector3((index * segmentSize.x * scale) - (0.5f * (CellSize.x - 1)), 0, CellSize.x / 2f);
            break;
        case E_GridCellReferenceType.E_down:
            offset = new Vector3((index * segmentSize.x * scale) - (0.5f * (CellSize.x - 1)), 0, -CellSize.x / 2f);
            break;
        case E_GridCellReferenceType.E_left:
            offset = new Vector3(-CellSize.x / 2f, 0,
                (index * segmentSize.y * scale) + (segmentSize.y / 2 * scale) - (0.5f * CellSize.y));
            break;
        case E_GridCellReferenceType.E_right:
            offset = new Vector3(CellSize.x / 2f, 0,
                (index * segmentSize.y * scale) + (segmentSize.y / 2 * scale) - (0.5f * CellSize.y));
            break;
    }

    return Position + offset;
}

public List<Matrix4x4> HallwayWallMatrix4x4(Vector2 segmentSize)
{
    try
    {
        List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();

        var wallCount = math.max(0, (int)(CellSize.x / segmentSize.x));
        var scale = CellSize.x / wallCount / segmentSize.x;

        AddWallToMatrix(UpN, wallCount, segmentSize, scale, true, ref matrix4X4s);
        AddWallToMatrix(DownN, wallCount, segmentSize, scale, false, ref matrix4X4s);

        wallCount = Mathf.Max(0, (int)(CellSize.y / segmentSize.y));
        scale = CellSize.y / wallCount / segmentSize.y;

        AddWallToMatrix(LeftN, wallCount, segmentSize, scale, true, ref matrix4X4s);
        AddWallToMatrix(RightN, wallCount, segmentSize, scale, false, ref matrix4X4s);

        return matrix4X4s;
    }
    catch (Exception e)
    {
        Debug.LogError("Exception caught: " + e.Message);
        return null;
    }
}

private void AddWallToMatrix(GridCellData node, int wallCount, Vector2 segmentSize, float scale, bool isVertical,
    ref List<Matrix4x4> matrix4X4s)
{
    // Sprawdzamy, czy node jest null lub jeśli GridCellType jest Empty
    if (node == null || node.GridCellType == E_GridCellType.Empty)
    {
        for (int i = 0; i < wallCount; i++)
        {
            Vector3 t = Position;
            if (isVertical)
            {
                t += new Vector3(
                    (-CellSize.x / 2f) + (i * segmentSize.x * scale) + (segmentSize.x / 2 * scale), 0,
                    (node == UpN ? CellSize.y / 2f : -CellSize.y / 2f));
            }
            else
            {
                t += new Vector3(
                    (node == LeftN ? -CellSize.x / 2f : CellSize.x / 2f), 0,
                    (-CellSize.y / 2f) + (i * segmentSize.y * scale) + (segmentSize.y / 2 * scale));
            }

            Quaternion r = Quaternion.Euler(0f, (isVertical ? 90f : 0f), 0f);
            Vector3 s = new Vector3(1f, 1f, scale);

            Matrix4x4 newMatrix = Matrix4x4.TRS(t, r, s);
            matrix4X4s.Add(newMatrix);
        }
    }
}

internal IEnumerable<Matrix4x4> RoomPassWallMatrix4x4(Vector2 segmentSize)
{
    List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();
    matrix4X4s.AddRange(GeneratePassRoomSide(segmentSize));
    return matrix4X4s;
}

public IEnumerable<Matrix4x4> FlorMatrix4x4(Vector2 segmentSize)
{
    List<Matrix4x4> matrix4X4s = new List<Matrix4x4>();

    // Obliczamy liczbę segmentów wzdłuż osi X i Z
    int segmentCountX = Mathf.CeilToInt(CellSize.x / segmentSize.x);
    int segmentCountZ = Mathf.CeilToInt(CellSize.y / segmentSize.y);

    // Obliczamy skalę pojedynczego segmentu
    Vector3 segmentScale = new Vector3(segmentSize.x, 1f, segmentSize.y);

    for (int x = 0; x < segmentCountX; x++)
    {
        for (int z = 0; z < segmentCountZ; z++)
        {
            // Obliczamy pozycję każdego segmentu
            Vector3 segmentPosition = Position + new Vector3(
                x * segmentSize.x - CellSize.x / 2 + segmentSize.x / 2,
                0f,
                z * segmentSize.y - CellSize.y / 2 + segmentSize.y / 2
            );

            Quaternion rotation = Quaternion.identity; // Brak rotacji dla podłogi
            Matrix4x4 newMatrix = Matrix4x4.TRS(segmentPosition, rotation, segmentScale);

            matrix4X4s.Add(newMatrix);
        }
    }

    return matrix4X4s;
}

public List<GridCellData> ReturnNeighbour()
{
    List<GridCellData> gridCellData = new List<GridCellData>
    {
        UpN,
        DownN,
        LeftN,
        RightN
    };
    return gridCellData;
}

public enum E_GridCellReferenceType
{
    E_up,
    E_down,
    E_left,
    E_right
}

public void SetupCellRoom(Room room)
{
    if (GridCellType == E_GridCellType.Room && connectedRoom == null)
    {
        connectedRoom = room;
    }
}

public Room GetConnectedRoom()
{
    return connectedRoom;
}*/

#endregion