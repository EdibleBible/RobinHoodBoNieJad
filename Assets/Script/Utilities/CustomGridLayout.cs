using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CustomGridLayout : LayoutGroup
{
    public int columns = 3;
    public int rows = 3;
    public Vector2 spacing = new Vector2(10, 10);
    
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        ArrangeCells();
    }

    public override void CalculateLayoutInputVertical()
    {
        ArrangeCells();
    }

    public override void SetLayoutHorizontal()
    {
        ArrangeCells();
    }

    public override void SetLayoutVertical()
    {
        ArrangeCells();
    }

    private void ArrangeCells()
    {
        int childCount = transform.childCount;
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = (parentWidth - (columns - 1) * spacing.x) / columns;
        float cellHeight = (parentHeight - (rows - 1) * spacing.y) / rows;
        Vector2 cellSize = new Vector2(cellWidth, cellHeight);
        
        float startX = (parentWidth - (cellSize.x * columns + (columns - 1) * spacing.x)) / 2f;
        float startY = (parentHeight - (cellSize.y * rows + (rows - 1) * spacing.y)) / 2f;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = rectChildren[i];
            int row = i / columns;
            int col = i % columns;

            float posX = startX + col * (cellSize.x + spacing.x);
            float posY = startY + row * (cellSize.y + spacing.y);

            SetChildAlongAxis(child, 0, posX, cellSize.x);
            SetChildAlongAxis(child, 1, posY, cellSize.y);
        }
    }
}