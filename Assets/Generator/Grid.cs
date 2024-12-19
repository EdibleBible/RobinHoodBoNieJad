using CodeMonkey.Utils;
using System;
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
            return new Vector3(x,0, y) * cellSize + originPosition;
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
        public Vector2Int GeneratedGridSize()
        {
            return new Vector2Int(width, height);
        }

        public void DebugGrid()
        {
            for(int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    UtilsClass.CreateWorldText(gridArray[x,y].ToString(), null,GetWorldPosition(x,y) + new Vector3(cellSize,1,cellSize) * .5f,5,Color.white,TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width,height), GetWorldPosition(width, height), Color.white, 100f);
        }
    }
}




