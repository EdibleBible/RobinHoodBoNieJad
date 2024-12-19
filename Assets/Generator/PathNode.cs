public class PathNode
{
    private CustomGrid.Grid<PathNode> grid;
    public int X;
    public int Y;

    public int GCost;
    public int HCost;
    public int FCost;

    public bool IsWalkable = true;

    public PathNode CameFromNode;

    public PathNode(CustomGrid.Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.X = x;
        this.Y = y;
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



