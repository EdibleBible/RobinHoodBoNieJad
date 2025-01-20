using System;
using Unity.VisualScripting;

[Serializable]
public class Edge
{
    public Point Point1 { get; private set; }
    public Point Point2 { get; private set; }

    public Room Point1Room { get; private set; }
    public Room Point2Room { get; private set; }

    public GridCellData EntryGridCell; // cell in room
    public GridCellData ExitGridCell; // cell in room

    public GridCellData StartPathFindCell; // pass place axis
    public GridCellData EndPathFindCell; // pass place axis
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
        EntryGridCell = enterPoint;
        ExitGridCell = exitPoint;

        EntryGridCell.GridCellType = E_GridCellType.Pass;
        ExitGridCell.GridCellType = E_GridCellType.Pass;
    }
    public void SetStartPathFind(GridCellData start)
    {
        if (EntryGridCell.AxisCell == null)
        {
            StartPathFindCell = start;
            EntryGridCell.SetAxisCell(start);

        }
        else
        {
            StartPathFindCell = EntryGridCell.AxisCell;
        }
    }

    public void SetEndPathFind(GridCellData end)
    {
        if (ExitGridCell.AxisCell == null)
        {
            EndPathFindCell = end;
            ExitGridCell.SetAxisCell(end);
        }
        else
        {
            EndPathFindCell = ExitGridCell.AxisCell;
        }
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

