using System;
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
    [SerializeField]
    private GameObject _houseObject;

    public Action OnPlotGenerated;

    [Header("Debug Settings")]
    [SerializeField]
    private bool _showPlotBounds;

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
                if (!_roadTiles.Contains(_cellGrid[i, j].Tile)) // tile is not a road anymore, break the segment
                {
                    continuous = 0;
                    continue;
                }

                verticalRoads.Add((continuous, _cellGrid[i, j])); //tile is a road and it a part of bigger road segment 
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

        foreach ((int Index, Cell Cell) segment in verticalRoads) // loop over road segments and create plots on both sides - plotA | plotB
        {
            if (segment.Index == 0)
            {
                if (plotA.Height > 1)
                {  
                    plotA.side = Sides.Left;
                    plotA.Grow(_tileToReplace);
                    if (plotA.IsValid(_minPlotSize)) CheckOverlap(plotA);
                }
                if (plotB.Height > 1)
                {
                    plotB.side = Sides.Right;
                    plotB.Grow(_tileToReplace);
                    if (plotB.IsValid(_minPlotSize)) CheckOverlap(plotB);
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


        foreach ((int Index, Cell Cell) segment in horizontalRoads) // loop over road segments and create plots on both sides - ▼plotA --- ▲plotB
        {
            if (segment.Index == 0)
            {
                if (plotA.Width > 1)
                { 
                    plotA.side = Sides.Down;
                    plotA.Grow(_tileToReplace);
                    if (plotA.IsValid(_minPlotSize)) CheckOverlap(plotA);
                }
                if (plotB.Width > 1)
                {    
                    plotB.side = Sides.Up;
                    plotB.Grow(_tileToReplace);
                    if (plotB.IsValid(_minPlotSize)) CheckOverlap(plotB);
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


        foreach (Plot plot in _plots)
        {
            plot.CreateGrid();

            GameObject houseInstance = Instantiate(_houseObject,
                                                   plot.PlotGrid[plot.PlotGrid.GetLength(0) / 2, plot.PlotGrid.GetLength(1) / 2].GetWorldSpacePosition(),
                                                   Quaternion.identity,
                                                   gameObject.transform);
            BoxCollider houseCollider;
            if (houseInstance.TryGetComponent<BoxCollider>(out houseCollider))
            {
                Bounds houseBounds = houseCollider.bounds;

                houseBounds.Encapsulate(plot.EndingCell.GetWorldSpacePosition());
                houseBounds.Encapsulate(plot.SideCell.GetWorldSpacePosition());
                houseBounds.Encapsulate(plot.StartingCell.GetWorldSpacePosition());
                houseCollider.size = houseBounds.size;
                //Not Tested 
                //houseCollider.center = houseBounds.center;
            }
            else
            {
                Debug.LogError("Can't acces house instance collider");
            }
        }

        OnPlotGenerated?.Invoke();

    }

    public List<Plot> GetPlots()
    {
        return _plots;
    }

    /// <summary>
    /// Checks if plot overlaps any other plot on the grid.<br/>
    /// If there is no overlap the function adds plot to the list.<br/>
    /// If the plot overlaps and is smaller than other; the plot it discarded.<br/>
    /// If the plot is bigger the other then other is discarded.<br/>
    /// </summary>

    private void CheckOverlap(Plot plot)
    {
        List<Plot> smallerPlots = new List<Plot>();

        foreach(Plot other in _plots)
        {
            if (other == plot) continue;

            if (other.bounds.Intersects(plot.bounds))
            {
                if (other.GetArea() > plot.GetArea()) return;

                smallerPlots.Add(other);
            }
        }

        foreach (Plot other in smallerPlots)
        {
            _plots.Remove(other);
        }

        _plots.Add(plot);
    }

    private void OnDrawGizmos()
    {
        if (_plots == null) return;

        if (_plots.Count == 0) return;

        if (!_showPlotBounds) return;

        Gizmos.color = Color.blue;

        foreach (Plot plot in _plots)
        {
            if (plot.PlotGrid == null) continue;

            Gizmos.color = plot.DEBUGCOLOR;
            for (int x = 0; x < plot.PlotGrid.GetLength(0); x++)
            {
                for (int y = 0; y < plot.PlotGrid.GetLength(1); y++)
                {
                    Gizmos.DrawSphere(plot.PlotGrid[x, y].GetWorldSpacePosition(), 0.2f);
                }
            }

            Gizmos.color = Color.white;
        }
    }
}
