using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaveFunctionCollapse))]
public class HouseGenerator : MonoBehaviour
{
    private WaveFunctionCollapse waveFunctionCollapse;
    private List<Plot> _plots;
    private Cell[,] _cellGrid;

    [Header("Plot Settings")]
    [SerializeField]
    private Vector2Int _minPlotSize;
    [SerializeField]
    private Vector2Int _maximumPlotSize;
    [SerializeField]
    private Tile _tileToReplace;
    [SerializeField]
    private List<Tile> _roadTiles;


    private void OnEnable()
    {
        waveFunctionCollapse = gameObject.GetComponent<WaveFunctionCollapse>();
        waveFunctionCollapse.OnAllCellsCollapsed += FindSuitablePlotPosition;
        _plots = new List<Plot>();
    }

    private void OnDisable()
    {
        waveFunctionCollapse.OnAllCellsCollapsed -= FindSuitablePlotPosition;
    }

    void Start()
    {
        
    }

    public void FindSuitablePlotPosition()
    {
        _cellGrid = waveFunctionCollapse.GetGrid();

        List<Cell> roadCells = new List<Cell>();

        for (int i = 0; i < _cellGrid.GetLength(0); i++)
        {
            for (int j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (!_roadTiles.Contains(_cellGrid[i, j].Tile)) continue;

                roadCells.Add(_cellGrid[i, j]);
            }
        }


        foreach (Cell roadCell in roadCells) Debug.Log("x: " + roadCell._position.x + " y:" + roadCell._position.y);

    }

    private void OnDrawGizmos()
    {
        if (_plots == null) return;

        if (_plots.Count == 0) return;

        foreach (Plot plot in _plots) Gizmos.DrawCube(plot.PlotBounds.center, plot.PlotBounds.size);
    }
}
