using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, ExecuteAlways]
public struct GridOptions
{
    [Header("Grid Material")]
    public Material EmptyCellMaterial;
    public Material RoomCellMaterial;
    public Material PassCellMaterial;

    [Header("Grid Parameters")]
    public Vector2Int MaxAxisSize;
    public Vector2Int MinAxisSize;
    public float cellScale;

    public Vector2Int RandomizeGridSize()
    {
        int randomX = UnityEngine.Random.RandomRange(MinAxisSize.x,MaxAxisSize.x);
        int randomY = UnityEngine.Random.RandomRange(MinAxisSize.y,MaxAxisSize.y);
        return new Vector2Int(randomX, randomY);
    }


    /*    public Material EmptyCellMaterial;
        public Material RoomCellMaterial;
        public Material PassCellMaterial;
        public Vector2Int MaxAxisSize;
        public Vector2Int MinAxisSize;
        [HideInInspector] public Vector2Int CurrAxisSize;
        public List<GridCellData> AllGridCell;
        [SerializeField] private Vector3 cellScale;  // Zmienna dla skali komórek

        // Getter i setter dla CellScale, który będzie aktualizował GridCellOffset
        public Vector3 CellScale
        {
            get => cellScale;
            set
            {
                if (cellScale != value)
                {
                    cellScale = value;
                    // Zaktualizuj offset, gdy zmienia się skala
                    UpdateGridCellOffset();
                }
            }
        }

        // GridCellOffset będzie automatycznie ustawiany w zależności od CellScale
        public Vector3 GridCellOffset;

        public GridData(Vector2Int maxAxisSize, Vector2Int minAxisSize, Vector3 gridCellOffset, Vector3 cellScale) : this()
        {
            if (minAxisSize.x > maxAxisSize.x || minAxisSize.y > maxAxisSize.y)
            {
                throw new ArgumentException("MinAxisSize must be less than or equal to MaxAxisSize.");
            }

            MaxAxisSize = maxAxisSize;
            MinAxisSize = minAxisSize;
            GridCellOffset = gridCellOffset;
            AllGridCell = new List<GridCellData>();
            CellScale = cellScale; // Inicjalizujemy skalę
        }

        // Ta metoda będzie wywoływana, aby zaktualizować GridCellOffset, gdy CellScale się zmieni
        private void UpdateGridCellOffset()
        {
            // Ustawienie GridCellOffset na te same wartości co CellScale
            GridCellOffset = new Vector3(CellScale.x, CellScale.y, CellScale.z);
        }

        // Inicjalizacja siatki
        public void GenerateEmptyGrid()
        {
            int xAxis = UnityEngine.Random.Range(MinAxisSize.x, MaxAxisSize.x);
            int yAxis = UnityEngine.Random.Range(MinAxisSize.y, MaxAxisSize.y);
            CurrAxisSize = new Vector2Int(xAxis, yAxis);
            GenerateEmptyGrid(CurrAxisSize);
        }

        public void GenerateEmptyGrid(Vector2Int gridSize)
        {
            CurrAxisSize = gridSize;

            for (int x = 0; x < CurrAxisSize.x; x++) // if (xAxis == 10 cord in x 0 => 9)
            {
                for (int y = 0; y < CurrAxisSize.y; y++) // if (yAxis == 10 cord in y 0 => 9)
                {
                    GridCellData newCell = new GridCellData();
                    newCell.GridCellType = E_GridCellType.Empty;
                    newCell.SetCoordinate(x, y);
                    newCell.SetPosition(x * GridCellOffset.x, GridCellOffset.y, y * GridCellOffset.z); // y on 3 axes is z
                    AllGridCell.Add(newCell);
                }
            }
        }

        // Znalezienie komórki siatki na podstawie współrzędnych
        public GridCellData FindSelectedGridCell(int x, int y)
        {
            GridCellData selectedGrid = AllGridCell.Find(cell => cell.Coordinate.x == x && cell.Coordinate.y == y);
            return selectedGrid;
        }

        public void OnValidate()
        {
            // Wywołaj metodę instancyjną, aby zaktualizować GridCellOffset po każdej zmianie
            UpdateGridCellOffset();
        }*/
}


