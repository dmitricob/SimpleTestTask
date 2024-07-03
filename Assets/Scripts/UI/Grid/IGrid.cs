using System.Collections.Generic;
using UnityEngine;

namespace UI.Grid
{
    public interface IGrid
    {
        void SetGridSize(int rows, int columns);
        void AddChild(GameObject child);
        void RemoveChild(GameObject child);
    }
}