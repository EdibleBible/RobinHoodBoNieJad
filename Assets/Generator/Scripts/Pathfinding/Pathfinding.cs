using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    public bool CAN_PATH_TOUCHED = true;

    private int MOVE_DIAGONAL_COST = 10;
    private int MOVE_STRAIGHT_COST = 5;

    private int MOVE_EMPTY_CELL_COST = 2;
    private int MOVE_HALLWAY_CELL_COST = 1;

    private CustomGrid.Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    public Pathfinding(int width, int height, float cellSize, Vector3 originalPosition, List<GridCellData> roomGridCell, List<GridCellData> allGridCell)
    {
        grid = new CustomGrid.Grid<PathNode>(width, height, cellSize, originalPosition,
            (CustomGrid.Grid<PathNode> g, int x, int y) => new PathNode(g, x, y, allGridCell.Where(cell => cell.Coordinate.x == x && cell.Coordinate.y == y) .FirstOrDefault()));

        foreach(var gridCell in roomGridCell)
        {
            var node = grid.GetValue(gridCell.Coordinate.x,gridCell.Coordinate.y);
            node.IsWalkable = false;
            grid.SetValue(gridCell.Coordinate.x, gridCell.Coordinate.y, node);
        }
    }

    public CustomGrid.Grid<PathNode> GetGrid()
    {
        return grid;
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        if (startNode == null || endNode == null)
        {
            // Invalid Path
            return null;
        }

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetValue(x, y);
                pathNode.GCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.CameFromNode = null;
            }
        }

        startNode.GCost = 0;
        startNode.HCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in grid.GetNeighbourList(currentNode, false))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.IsWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.GCost)
                {
                    neighbourNode.CameFromNode = currentNode;
                    neighbourNode.GCost = tentativeGCost;
                    neighbourNode.HCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

    /*public List<PathNode> GetNeighbourList(PathNode currentNode, bool allowDiagonals)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.X - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y));

            if (allowDiagonals)
            {
                // Left Down
                if (currentNode.Y - 1 >= 0) neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y - 1));
                // Left Up
                if (currentNode.Y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y + 1));
            }
        }
        if (currentNode.X + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y));

            if (allowDiagonals)
            {
                // Right Down
                if (currentNode.Y - 1 >= 0) neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y - 1));
                // Right Up
                if (currentNode.Y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y + 1));
            }
        }
        // Down
        if (currentNode.Y - 1 >= 0) neighbourList.Add(GetNode(currentNode.X, currentNode.Y - 1));
        // Up
        if (currentNode.Y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.X, currentNode.Y + 1));

        return neighbourList;
    }*/

    public PathNode GetNode(int x, int y)
    {
        return grid.GetValue(x, y);
    }


    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.CameFromNode != null)
        {
            path.Add(currentNode.CameFromNode);
            currentNode = currentNode.CameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.X - b.X);
        int yDistance = Mathf.Abs(a.Y - b.Y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        int baseCost = MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;

        int usedCostPenalty = 0;

        if(b.cellData.GridCellType == E_GridCellType.Empty)
        {
            usedCostPenalty = MOVE_EMPTY_CELL_COST;
        }
        else if(b.cellData.GridCellType == E_GridCellType.HallwayInside || b.cellData.GridCellType == E_GridCellType.HallwayBorder)
        {
            usedCostPenalty = MOVE_HALLWAY_CELL_COST;

        }

        return baseCost + usedCostPenalty;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].FCost < lowestFCostNode.FCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    internal void DrawDebugGrid()
    {
        if (grid == null)
            return;
        grid.DebugGrid();
    }
}



