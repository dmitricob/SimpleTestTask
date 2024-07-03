using System;
using UnityEngine;
using UnityEngine.UI;

public class ResizableGrid : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private RectTransform _gridRectTransform;
    [SerializeField] private int _rows = 2;
    [SerializeField] private int _columns = 2;
    [SerializeField] private float _spacingModif = 0.1f;

    private void OnEnable()
    {
        SetupGrid(_rows, _columns);
    }

    private void SetupGrid(int rows, int columns)
    {
        var rect = _gridRectTransform.rect;

        var cellSize = Mathf.Min((rect.height) / rows, (rect.width) / columns);

        var totalHeight = cellSize * rows;
        var totalLength = cellSize * columns;

        _gridLayout.padding.left = _gridLayout.padding.right = (int)(rect.width - totalLength) / 2;
        _gridLayout.padding.top = _gridLayout.padding.bottom = (int)(rect.height - totalHeight) / 2;
        
        var spacing = cellSize * _spacingModif / 2;
        cellSize -= spacing;
        
        _gridLayout.spacing = new Vector2(spacing, spacing);
        _gridLayout.cellSize = new Vector2(cellSize, cellSize);
    }
    
}
