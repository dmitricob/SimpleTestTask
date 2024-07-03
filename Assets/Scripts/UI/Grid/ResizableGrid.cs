using UI.Grid;
using UnityEngine;
using UnityEngine.UI;

public class ResizableGrid : MonoBehaviour, IGrid
{
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private RectTransform _gridRectTransform;
    [SerializeField] private float _spacingModif = 0.1f;


    private void Awake()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetGridSize(int rows, int columns)
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

    public void AddChild(GameObject child)
    {
        child.transform.SetParent(transform);
    }
    
    public void RemoveChild(GameObject child)
    {
        child.transform.SetParent(null);
    }
}
