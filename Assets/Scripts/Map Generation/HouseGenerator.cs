using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Vector2Int _maxPlotSize;
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

    /// <summary>
    /// Finds contiguous roads to create rectangular plots for house generations. 
    /// </summary>
    public void FindSuitablePlotPosition()
    {
        _cellGrid = _waveFunctionCollapse.GetGrid();

        List<(int, Cell)> verticalRoads = new List<(int Index, Cell Cell)>();
        List<(int, Cell)> horizontalRoads = new List<(int Index, Cell Cell)>();

        
        for (int i = 0; i < _cellGrid.GetLength(0); i++) //Row major scan for continuous roads
        {
            int continuous = 0;


            for (int j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (!_roadTiles.Contains(_cellGrid[i, j].Tile)) // is not road anymore
                {
                    continuous = 0;
                    continue;
                }

                verticalRoads.Add((continuous, _cellGrid[i, j]));
                continuous++;
            }
        }

        for (int j = 0; j < _cellGrid.GetLength(1); j++) //Column major scan for continuous roads
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

        Plot plotA = new Plot(_cellGrid, _maxPlotSize);
        Plot plotB = new Plot(_cellGrid, _maxPlotSize);

        foreach ((int Index, Cell Cell) segment in verticalRoads)
        {
            if (segment.Index == 0)
            {
                if (plotA.Height > 1)
                {
                    _plots.Add(plotA);
                    plotA.side = Sides.Left;
                }
                if (plotB.Height > 1)
                {
                    _plots.Add(plotB);
                    plotB.side = Sides.Right;
                }

                if (segment.Cell.Index.x - 1 > 0)
                {
                    plotA = new Plot(_cellGrid, _maxPlotSize);
                    plotA.StartingCell = _cellGrid[segment.Cell.Index.x - 1, segment.Cell.Index.y];
                }
                
                if (segment.Cell.Index.x + 1 < _cellGrid.GetLength(0))
                {
                    plotB = new Plot(_cellGrid, _maxPlotSize);
                    plotB.StartingCell = _cellGrid[segment.Cell.Index.x + 1, segment.Cell.Index.y];
                }
                
            }
            else
            {
                plotA.Height++;
                plotB.Height++;
            }

        }

        plotA = new Plot(_cellGrid, _maxPlotSize);
        plotB = new Plot(_cellGrid, _maxPlotSize);


        foreach ((int Index, Cell Cell) segment in horizontalRoads)
        {
            if (segment.Index == 0)
            {
                if (plotA.Width > 1)
                {
                    _plots.Add(plotA);
                    plotA.side = Sides.Down;
                }
                if (plotB.Width > 1)
                {
                    _plots.Add(plotB);
                    plotB.side = Sides.Up;
                }

                if (segment.Cell.Index.y - 1 > 0)
                {
                    plotA = new Plot(_cellGrid, _maxPlotSize);
                    plotA.StartingCell = _cellGrid[segment.Cell.Index.x, segment.Cell.Index.y - 1];
                }

                if (segment.Cell.Index.y + 1 < _cellGrid.GetLength(1))
                {
                    plotB = new Plot(_cellGrid, _maxPlotSize);
                    plotB.StartingCell = _cellGrid[segment.Cell.Index.x, segment.Cell.Index.y + 1];
                }
            }
            else
            {
                plotA.Width++;
                plotB.Width++;
            }

        }

        List<Plot> toRemove = new List<Plot>();

        foreach (Plot plot in _plots)
        {
            plot.Grow(_tileToReplace);

            if (plot.GetArea() < _minPlotSize.x * _minPlotSize.y) toRemove.Add(plot);
        }


        foreach (Plot plot in toRemove)
        {
            _plots.Remove(plot);
        }

    }


    /// <summary>
    /// Draws bounds.
    /// Thanks to https://gist.github.com/unitycoder/58f4b5d80f423d29e35c814a9556f9d9
    /// </summary>
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
            //

            if (plot.StartingCell == null) continue;
            if (plot.EndingCell == null) continue;
            if (plot.SideCell == null) continue;

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(plot.StartingCell.GetWorldSpacePosition(), .2f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(plot.EndingCell.GetWorldSpacePosition(), .2f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(plot.SideCell.GetWorldSpacePosition(), .2f);

            Gizmos.color = Color.white;

            DrawBounds(plot.bounds);

        }


    }
}
