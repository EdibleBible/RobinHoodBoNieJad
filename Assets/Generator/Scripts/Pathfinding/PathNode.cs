public class PathNode
{
    private CustomGrid.Grid<PathNode> grid;
    public int X;
    public int Y;

    public int GCost;
    public int HCost;
    public int FCost;

    public bool IsWalkable = true;
    public GridCellData cellData;

    public PathNode CameFromNode;

    public PathNode(CustomGrid.Grid<PathNode> grid, int x, int y, GridCellData cellData)
    {
        this.grid = grid;
        this.X = x;
        this.Y = y;
        this.cellData = cellData;
    }

    public override string ToString()
    {
        return X + "," + Y;
    }

    internal void CalculateFCost()
    {
        FCost = GCost + HCost;
    }
}



