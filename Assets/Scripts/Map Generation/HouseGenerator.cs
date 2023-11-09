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
                if (!_roadTiles.Contains(_cellGrid[i, j].Tile))
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

        Plot southPlot = new Plot();
        Plot northPlot = new Plot();

        foreach ((int Index, Cell Cell) roadSegment in horizontalRoads) //Creates plots above and below horizontal roads according to their length 
        {
            if (roadSegment.Index == 0) // New segment began checks if old one should be added to list
            {
                if (southPlot.isValid())
                {
                    _plots.Add(southPlot);
                    southPlot.side = Sides.Down;
                }
                if (northPlot.isValid())
                {
                    _plots.Add(northPlot);
                    northPlot.side = Sides.Up;
                }


                southPlot = new Plot();
                northPlot = new Plot();

                if (roadSegment.Cell.Index.y + 1 < _cellGrid.GetLength(1)) northPlot.bounds = new Bounds(_cellGrid[roadSegment.Cell.Index.x, roadSegment.Cell.Index.y + 1].GetWorldSpacePosition(), Vector3.one);
                if (roadSegment.Cell.Index.y - 1 >= 0) southPlot.bounds = new Bounds(_cellGrid[roadSegment.Cell.Index.x, roadSegment.Cell.Index.y - 1].GetWorldSpacePosition(), Vector3.one);

            }

            if (roadSegment.Index > 0)
            {
                if (northPlot.bounds.extents.magnitude > 0) northPlot.bounds.Encapsulate(_cellGrid[roadSegment.Cell.Index.x, roadSegment.Cell.Index.y + 1].GetWorldSpacePosition());
                if (southPlot.bounds.extents.magnitude > 0) southPlot.bounds.Encapsulate(_cellGrid[roadSegment.Cell.Index.x, roadSegment.Cell.Index.y - 1].GetWorldSpacePosition());
            }

        }

        Plot westPlot = new Plot();
        Plot eastPlot = new Plot();
        foreach ((int Index, Cell Cell) roadSegment in verticalRoads) //Creates plots left and right vertical roads according to their length 
        {

            if (roadSegment.Index == 0)
            {
                if (westPlot.isValid())
                {
                    _plots.Add(westPlot);
                    westPlot.side = Sides.Left;
                }
                if (eastPlot.isValid())
                {
                    _plots.Add(eastPlot);
                    eastPlot.side = Sides.Right;
                }

                westPlot = new Plot();
                eastPlot = new Plot();

                if (roadSegment.Cell.Index.x + 1 < _cellGrid.GetLength(0)) eastPlot.bounds = new Bounds(_cellGrid[roadSegment.Cell.Index.x + 1, roadSegment.Cell.Index.y].GetWorldSpacePosition(), Vector3.one);
                if (roadSegment.Cell.Index.x - 1 >= 0) westPlot.bounds = new Bounds(_cellGrid[roadSegment.Cell.Index.x - 1, roadSegment.Cell.Index.y].GetWorldSpacePosition(), Vector3.one);
            }

            if (roadSegment.Index > 0)
            {

                if (westPlot.bounds.extents.magnitude > 0) westPlot.bounds.Encapsulate(_cellGrid[roadSegment.Cell.Index.x - 1, roadSegment.Cell.Index.y].GetWorldSpacePosition());
                if (eastPlot.bounds.extents.magnitude > 0) eastPlot.bounds.Encapsulate(_cellGrid[roadSegment.Cell.Index.x + 1, roadSegment.Cell.Index.y].GetWorldSpacePosition());
            }

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
            DrawBounds(plot.bounds);

            Gizmos.color = Color.white;

            switch (plot.side)
            {
                case Sides.Left:
                    Gizmos.color = Color.red;
                    break;
                case Sides.Right:
                    Gizmos.color = Color.blue;
                    break;
                case Sides.Up:
                    Gizmos.color = Color.yellow;
                    break;
                case Sides.Down:
                    Gizmos.color = Color.green;
                    break;

            }

            
            Gizmos.DrawSphere(plot.bounds.center, .2f);
            Gizmos.color = Color.white;
        }


    }
}
