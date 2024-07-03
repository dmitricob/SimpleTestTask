using UnityEngine;

namespace UI.Grid
{
    // Interface for grid that can hide different realizations e.g. based on grid layout group or based on simple math calculation positions
    public interface IGrid
    {
        void SetGridSize(int rows, int columns);
        void AddChild(GameObject child);
        void RemoveChild(GameObject child);
    }
}