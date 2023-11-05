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

        Plot newHorizontalPlot = new Plot();
        foreach ((int, Cell) soyLet in horizontalRoads)
        {

            if (soyLet.Item1 == 0)
            {
                if (newHorizontalPlot.bounds != null && newHorizontalPlot.bounds.extents.magnitude > 1) _plots.Add(newHorizontalPlot);

                newHorizontalPlot = new Plot();
                newHorizontalPlot.bounds = new Bounds(new Vector3(soyLet.Item2._position.x, 0, soyLet.Item2._position.y), Vector3.one);
            }

            if (soyLet.Item1 > 0) newHorizontalPlot.bounds.Encapsulate(new Vector3(soyLet.Item2._position.x, 0, soyLet.Item2._position.y));

        }


        Plot newVerticalPlot = new Plot();
        foreach ((int, Cell) soyLet in verticalRoads)
        {

            if (soyLet.Item1 == 0)
            {
                if (newHorizontalPlot.bounds != null && newHorizontalPlot.bounds.extents.magnitude > 1) _plots.Add(newHorizontalPlot);

                newHorizontalPlot = new Plot();
                newHorizontalPlot.bounds = new Bounds(new Vector3(soyLet.Item2._position.x, 0, soyLet.Item2._position.y), Vector3.one);
            }

            if (soyLet.Item1 > 0) newHorizontalPlot.bounds.Encapsulate(new Vector3(soyLet.Item2._position.x, 0, soyLet.Item2._position.y));

        }

    }

    void DrawBounds(Bounds b, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, Color.blue, delay);
        Debug.DrawLine(p2, p3, Color.red, delay);
        Debug.DrawLine(p3, p4, Color.yellow, delay);
        Debug.DrawLine(p4, p1, Color.magenta, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, Color.blue, delay);
        Debug.DrawLine(p6, p7, Color.red, delay);
        Debug.DrawLine(p7, p8, Color.yellow, delay);
        Debug.DrawLine(p8, p5, Color.magenta, delay);

        // sides
        Debug.DrawLine(p1, p5, Color.white, delay);
        Debug.DrawLine(p2, p6, Color.gray, delay);
        Debug.DrawLine(p3, p7, Color.green, delay);
        Debug.DrawLine(p4, p8, Color.cyan, delay);
    }



    private void OnDrawGizmos()
    {
        if (_plots == null) return;

        if (_plots.Count == 0) return;

        foreach (Plot plot in _plots)
        {
            DrawBounds(plot.bounds);
        }
    }
}
