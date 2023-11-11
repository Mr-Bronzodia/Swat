using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot 
{
    public Bounds bounds;
    public Sides side;

    public Cell StartingCell;

    public Cell EndingCell;

    public Cell SideCell;

    public int Width = 0;

    public int Height = 0;


    private Vector2Int _maxPlotSize;
    private Cell[,] _plotGrid;
    private Cell[,] _worldGrid;

    public Plot(Cell[,] worldGrid, Vector2Int maxPlotSize)
    {
        _worldGrid = worldGrid;
        _maxPlotSize = maxPlotSize;
    }

    public void Grow(Tile replacementTile)
    {
        if (StartingCell == null) return;
        if (StartingCell.Index.x + Width > _worldGrid.GetLength(0) || StartingCell.Index.x + Width < 0) return;
        if (StartingCell.Index.y + Height > _worldGrid.GetLength(1) || StartingCell.Index.y + Height < 0) return;

        EndingCell = _worldGrid[StartingCell.Index.x + Width, StartingCell.Index.y + Height];

        switch (side)
        {
            case Sides.Left:
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
            case Sides.Right:
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
            case Sides.Up:
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
            case Sides.Down:
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

        bounds = new Bounds(_worldGrid[StartingCell.Index.x, StartingCell.Index.y].GetWorldSpacePosition(), Vector3.one);
        bounds.Encapsulate(EndingCell.GetWorldSpacePosition());
        bounds.Encapsulate(SideCell.GetWorldSpacePosition());

    }

    public int GetArea()
    {
        return Mathf.Abs(Width * Height);
    }
}
