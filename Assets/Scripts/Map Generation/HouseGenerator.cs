using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaveFunctionCollapse))]
public class HouseGenerator : MonoBehaviour
{
    private WaveFunctionCollapse _waveFunctionCollapse;
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
    [SerializeField]
    private Tile _plotTile;


    private void OnEnable()
    {
        _waveFunctionCollapse = gameObject.GetComponent<WaveFunctionCollapse>();
        _waveFunctionCollapse.OnAllCellsCollapsed += FindSuitablePlotPosition;
        _plots = new List<Plot>();
    }

    private void OnDisable()
    {
        _waveFunctionCollapse.OnAllCellsCollapsed -= FindSuitablePlotPosition;
    }

    void Start()
    {
        
    }

    public void FindSuitablePlotPosition()
    {
        _cellGrid = _waveFunctionCollapse.GetGrid();

        List<Cell> roadCells = new List<Cell>();

        List<(int, Cell)> verticalRoads = new List<(int, Cell)>();
        List<(int, Cell)> horizontalRoads = new List<(int, Cell)>();

        for (int i = 0; i < _cellGrid.GetLength(0); i++)
        {
            int continuous = 0;

            for (int j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (!_roadTiles.Contains(_cellGrid[i, j].Tile))
                {
                    continuous = 0;
                    continue;
                }


                verticalRoads.Add((continuous, _cellGrid[i, j]));
                continuous++;
                roadCells.Add(_cellGrid[i, j]);
            }
        }

        for (int j = 0; j < _cellGrid.GetLength(1); j++)
        {
            int continuous = 0;

            for (int i = 0; i < _cellGrid.GetLength(0); i++)
            {
                if (!_roadTiles.Contains(_cellGrid[i, j].Tile))
                {
                    continuous = 0;
                    continue;
                }

                horizontalRoads.Add((continuous, _cellGrid[i, j]));
                continuous++;
            }
        }

        foreach ((int, Cell) soyLet in horizontalRoads)
        {

            if (soyLet.Item1 > 0) Debug.Log(soyLet.Item1 + " " + soyLet.Item2);

        }

    }



    private void OnDrawGizmos()
    {
        if (_plots == null) return;

        if (_plots.Count == 0) return;

        foreach (Plot plot in _plots)
        {

        }
    }
}
