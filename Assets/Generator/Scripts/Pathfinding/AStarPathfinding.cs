using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    public class Node
    {
        public Vector2Int Position { get; private set; }
        public Node Previouse { get; set; }

        public float cost { get; set; }

        public Node(Vector2Int position)
        {
            Position = position;
        }
    }

    public struct PathCost
    {
        public bool traversable { get; set; }
        public float cost { get; set; }
    }

    static readonly Vector2Int[] neighbours =
    {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1)
    };

    


    /*    public class Node
        {
            public GridCellData CellData { get; set; }  // Pokój (komórka) w siatce
            public float GCost { get; set; }  // Koszt dotarcia z punktu początkowego
            public float HCost { get; set; }  // Koszt heurystyczny (odległość do celu)
            public float FCost => GCost + HCost;  // Całkowity koszt (G + H)
            public Node Parent { get; set; }  // Rodzic w drzewie wyszukiwania

            //
            public Node(GridCellData cellData)
            {
                CellData = cellData;
                GCost = float.MaxValue;
                HCost = 0;
                Parent = null;
            }
        }

        private List<Node> openList = new List<Node>();
        private List<Node> closedList = new List<Node>();
        private List<GridCellData> allGridCell;

        public AStarPathfinding(List<GridCellData> allGridCell)
        {
            this.allGridCell = allGridCell;
        }

        public List<GridCellData> FindPath(GridCellData startRoom, GridCellData endRoom)
        {
            openList.Clear();
            closedList.Clear();

            Node startNode = new Node(startRoom);
            Node endNode = new Node(endRoom);

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = GetNodeWithLowestFCost();

                if (currentNode.CellData == endRoom)
                {
                    return RetracePath(startNode, currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighbor))
                        continue;

                    float newGCost = currentNode.GCost + GetDistance(currentNode, neighbor);
                    if (newGCost < neighbor.GCost)
                    {
                        neighbor.GCost = newGCost;
                        neighbor.HCost = GetDistance(neighbor, endNode);
                        neighbor.Parent = currentNode;

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }

            return null;  // Brak ścieżki
        }

        private Node GetNodeWithLowestFCost()
        {
            Node lowestFCostNode = openList[0];
            foreach (Node node in openList)
            {
                if (node.FCost < lowestFCostNode.FCost)
                    lowestFCostNode = node;
            }
            return lowestFCostNode;
        }

        private List<Node> GetNeighbors(Node node)
        {
            // W tym przypadku, zakładamy, że sąsiedzi to sąsiednie komórki w siatce,
            // możesz dodać logikę do filtracji tylko tych, które są połączone krawędziami.
            List<Node> neighbors = new List<Node>();

            // Załóżmy, że masz funkcję GetAdjacentRooms, która zwraca sąsiadujące komórki.
            List<GridCellData> adjacentRooms = GetAdjacentRooms(node.CellData);
            foreach (var room in adjacentRooms)
            {
                neighbors.Add(new Node(room));
            }

            return neighbors;
        }

        private List<GridCellData> GetAdjacentRooms(GridCellData room)
        {
            // Tu zaimplementuj logikę, która zwróci sąsiadujące komórki (pokoje)
            // np. na podstawie połączeń między pokojami.
            List<GridCellData> adjacentRooms = new List<GridCellData>();

            // Dodaj odpowiednią logikę do znajdowania sąsiadów
            return adjacentRooms;
        }

        private float GetDistance(Node a, Node b)
        {
            // Możesz użyć np. odległości Manhattan w przypadku siatki 2D:
            return Mathf.Abs(a.CellData.Coordinate.x - b.CellData.Coordinate.x) + Mathf.Abs(a.CellData.Coordinate.y - b.CellData.Coordinate.y);
        }

        private List<GridCellData> RetracePath(Node startNode, Node endNode)
        {
            List<GridCellData> path = new List<GridCellData>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.CellData);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }*/
}

