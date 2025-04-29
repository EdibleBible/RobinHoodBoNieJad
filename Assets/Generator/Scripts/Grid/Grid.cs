using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGrid
{
    public class Grid<TGridObj>
    {
        private int width;
        private int height;
        private float cellSize;
        private Vector3 originPosition;
        private TGridObj[,] gridArray;

        public Grid()
        {
        }

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObj>, int, int, TGridObj> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;

            gridArray = new TGridObj[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createGridObject(this, x, y);
                }
            }

        }

        public void SetValue(int x, int y, TGridObj value)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                gridArray[x, y] = value;
            }
        }
        public void SetValue(Vector3 worldPosition, TGridObj value)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetValue(x, y, value);
        }
        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        }
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0, y) * cellSize + originPosition;
        }
        public TGridObj GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return gridArray[x, y];
            }
            else
            {
                return default(TGridObj);
            }
        }
        public TGridObj GetValue(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetValue(x, y);
        }

        public float GetCellSize()
        {
            return cellSize;
        }
        public Vector2Int GeneratedGridSize()
        {
            return new Vector2Int(width, height);
        }
        public TGridObj[,] GetGridArray()
        {
            return gridArray;
        }
        public Vector2Int GetCoordinate(TGridObj selectedNode)
        {
            var xPos = 0;
            var yPos = 0;


            for (int x = 0; x < gridArray.GetLength(0); x++) // Iteruj po wymiarze X
            {
                for (int y = 0; y < gridArray.GetLength(1); y++) // Iteruj po wymiarze Y
                {
                    if (EqualityComparer<TGridObj>.Default.Equals(gridArray[x, y], selectedNode))
                    {
                        xPos = x;
                        yPos = y;
                        return new Vector2Int(xPos, yPos);
                    }
                }
            }

            return default(Vector2Int);

        }
        public List<TGridObj> GetNeighbourList(TGridObj selectedNode, bool allowDiagonals)
        {
            List<TGridObj> neighbourList = new List<TGridObj>();
            Vector2Int currentNode = GetCoordinate(selectedNode);

            if (currentNode.x - 1 >= 0)
            {
                // Left
                neighbourList.Add(GetValue(currentNode.x - 1, currentNode.y));

                if (allowDiagonals)
                {
                    // Left Down
                    if (currentNode.y - 1 >= 0) neighbourList.Add(GetValue(currentNode.x - 1, currentNode.y - 1));
                    // Left Up
                    if (currentNode.y + 1 < GetHeight()) neighbourList.Add(GetValue(currentNode.x - 1, currentNode.y + 1));
                }
            }
            if (currentNode.x + 1 < GetWidth())
            {
                // Right
                neighbourList.Add(GetValue(currentNode.x + 1, currentNode.y));

                if (allowDiagonals)
                {
                    // Right Down
                    if (currentNode.y - 1 >= 0) neighbourList.Add(GetValue(currentNode.x + 1, currentNode.y - 1));
                    // Right Up
                    if (currentNode.y + 1 < GetHeight()) neighbourList.Add(GetValue(currentNode.x + 1, currentNode.y + 1));
                }
            }
            // Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetValue(currentNode.x, currentNode.y - 1));
            // Up
            if (currentNode.y + 1 < GetHeight()) neighbourList.Add(GetValue(currentNode.x, currentNode.y + 1));

            return neighbourList;
        }
        public void DebugGrid()
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    var obj = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, 1, cellSize) * .5f, 5, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, height), GetWorldPosition(width, height), Color.white, 100f);
        }
        public int GetWidth()
        {
            return width;
        }
        public int GetHeight()
        {
            return height;
        }

        public List<TGridObj> GetAllGridElementList()
        {
            List<TGridObj> returnList = new List<TGridObj>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnList.Add(gridArray[x, y]);
                }
            }
            return returnList;
        }
    }
}




