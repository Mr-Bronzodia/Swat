using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot 
{
    public Bounds bounds;
    public ESides side;

    public Cell StartingCell;

    public Cell EndingCell;

    public Cell SideCell;

    public int Width = 0;

    public int Height = 0;

    private Vector2Int _maxPlotSize;

    public Color DEBUGCOLOR {  get; private set; }

    public Cell[,] PlotGrid { get; private set; }

    private Cell[,] _worldGrid;

    public Plot(Cell[,] worldGrid, Vector2Int maxPlotSize)
    {
        _worldGrid = worldGrid;
        _maxPlotSize = maxPlotSize;
        DEBUGCOLOR = Random.ColorHSV();
    }

    /// <summary>
    /// Grows plot based on direction heuristic. 
    /// </summary>
    /// <param name="replacementTile">Tile the plot can safely replace</param>
    public void Grow(Tile replacementTile)
    {
        if (StartingCell == null) return;
        if (StartingCell.Index.x + Width > _worldGrid.GetLength(0) - 1 | StartingCell.Index.x + Width < 0) return;
        if (StartingCell.Index.y + Height > _worldGrid.GetLength(1) - 1| StartingCell.Index.y + Height < 0) return;

        EndingCell = _worldGrid[StartingCell.Index.x + Width, StartingCell.Index.y + Height];

        switch (side)
        {
            case ESides.Left:
                for (int i = 0; i < _maxPlotSize.x; i++)
                {
                    if (StartingCell.Index.x - i <= 0) break;

                    if (_worldGrid[StartingCell.Index.x - i, StartingCell.Index.y].Tile != replacementTile) break;


                    if (_worldGrid[StartingCell.Index.x - i, StartingCell.Index.y + (Height / 2)].Tile != replacementTile) break;


                    if (_worldGrid[EndingCell.Index.x - i, EndingCell.Index.y].Tile != replacementTile) break;

                    SideCell = _worldGrid[StartingCell.Index.x - i, StartingCell.Index.y];
                    Width -= 1;

                }
                break;
            case ESides.Right:
                for (int i = 0; i < _maxPlotSize.x; i++)
                {
                    if (StartingCell.Index.x + i >= _worldGrid.GetLength(0)) break;

                    if (_worldGrid[StartingCell.Index.x + i, StartingCell.Index.y].Tile != replacementTile) break;

                    if (_worldGrid[StartingCell.Index.x + i, StartingCell.Index.y + (Height / 2)].Tile != replacementTile) break;

                    if (_worldGrid[EndingCell.Index.x + i, EndingCell.Index.y].Tile != replacementTile) break;

                    SideCell = _worldGrid[StartingCell.Index.x + i, StartingCell.Index.y];
                    Width += 1;

                }
                break;
            case ESides.Up:
                for (int i = 0; i < _maxPlotSize.y; i++)
                {
                    if (StartingCell.Index.y + i >= _worldGrid.GetLength(1)) break;

                    if (_worldGrid[StartingCell.Index.x, StartingCell.Index.y + i].Tile != replacementTile) break;

                    if (_worldGrid[StartingCell.Index.x + (Width / 2), StartingCell.Index.y + i].Tile != replacementTile) break;

                    if (_worldGrid[EndingCell.Index.x, EndingCell.Index.y + i].Tile != replacementTile) break;

                    SideCell = _worldGrid[StartingCell.Index.x, StartingCell.Index.y + i];
                    Height += 1;

                }
                break;
            case ESides.Down:
                for (int i = 0; i < _maxPlotSize.y; i++)
                {
                    if (StartingCell.Index.y - i <= 0) break;

                    if (_worldGrid[StartingCell.Index.x, StartingCell.Index.y - i].Tile != replacementTile) break;

                    if (_worldGrid[StartingCell.Index.x + (Width / 2), StartingCell.Index.y - i].Tile != replacementTile) break;

                    if (_worldGrid[EndingCell.Index.x, EndingCell.Index.y - i].Tile != replacementTile) break;
                    
                    SideCell = _worldGrid[StartingCell.Index.x, StartingCell.Index.y - i];
                    Height -= 1;
                }
                break;

            default: 
                break;
        }

        if (SideCell == null) return;

        bounds = new Bounds(_worldGrid[StartingCell.Index.x, StartingCell.Index.y].GetWorldSpacePosition(), Vector3.one);
        bounds.Encapsulate(EndingCell.GetWorldSpacePosition());
        bounds.Encapsulate(SideCell.GetWorldSpacePosition());

    }

    /// <summary>
    /// Checks if the plot is of a valid size. 
    /// </summary>
    /// <returns></returns>
    public bool IsValid(Vector2Int minPlotSize)
    {
        if (Mathf.Abs(Width) <= minPlotSize.x || Mathf.Abs(Height) <= minPlotSize.y) return false;

        return true;
    }

    //x = x  !!!!!!!!! z = y !!!!!!!!!!!!!
    public void CreateGrid()
    {
        if (bounds == null) return;

        PlotGrid = new Cell[Mathf.Abs(Width), Mathf.Abs(Height)];

        Vector2Int bottomLeftIndex = Vector2Int.zero; 

        switch (side)
        {
            case ESides.Left:

                bottomLeftIndex = new Vector2Int(SideCell.Index.x, SideCell.Index.y);
                break;
            case ESides.Right:

                bottomLeftIndex = new Vector2Int(StartingCell.Index.x, StartingCell.Index.y);
                break;
            case ESides.Up:

                bottomLeftIndex = new Vector2Int(StartingCell.Index.x, StartingCell.Index.y);
                break;
            case ESides.Down:

                bottomLeftIndex = new Vector2Int(SideCell.Index.x, SideCell.Index.y);
                break;

            default:
                break;
        }

        if (PlotGrid.GetLength(0) - 1 <= Mathf.Abs(Width) - 1) return;
        if (PlotGrid.GetLength(1) - 1 <= Mathf.Abs(Height) - 1) return;
        

        for (int x = 0; x < Mathf.Abs(Width) - 1; x++)
        {
            for (int y = 0; y < Mathf.Abs(Height) - 1; y++)
            {
                PlotGrid[x,y] = _worldGrid[bottomLeftIndex.x + x, bottomLeftIndex.y + y];
            }
        }


    }

    /// <summary>
    /// Returns absolute area of a plot
    /// Note: due to class design the width or height can be negative.
    /// </summary>
    /// <returns></returns>
    public int GetArea()
    {
        return Mathf.Abs(Width * Height);
    }
}
