using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    //The GridCell class is used to check if a position on the 5x5 grid has been used already (card been placed in this case)
    // Modularises code making it easy to track which cells are and are not working correctly or not being detected
    private GridXZ<GridCell> _grid;

    private bool _placed;

    public GridCell(GridXZ<GridCell> g, int x, int y)
    {
        _grid = g;
        _placed = false;
    }

    public bool IsPlaced() => _placed;

    public void SetPlaced(bool x) => _placed = x;
}
