using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class Cell : IComparable<Cell>
{
    public List<Tile> PossibleTiles;

    public bool IsCollapsed { get; private set; } = false;

    public Tile Tile { get; private set; } = null;

    public Vector2Int Index { get; private set; }

    private int _size;

    float _entropyModifier;

    GameObject _instance;



    public Cell(Vector2Int index, int size)
    {
        PossibleTiles = Resources.LoadAll<Tile>("TileTypes").ToList();
        Index = index;
        _size = size;
        _entropyModifier = UnityEngine.Random.Range(0, 0.02f);
    }

    ///<summary>
    ///Uncollapses the cell and removes instance form the game.
    ///</summary>
    public void DestroyCell()
    {
        UnityEngine.Object.Destroy(_instance, 0.1f);
        Tile = null;
        _entropyModifier = UnityEngine.Random.Range(0, 0.02f);
        IsCollapsed = false;
        PossibleTiles = Resources.LoadAll<Tile>("TileTypes").ToList();
    }

    public void DestroyCellImmediate()
    {
        UnityEngine.Object.DestroyImmediate(_instance);
        Tile = null;
        _entropyModifier = UnityEngine.Random.Range(0, 0.02f);
        IsCollapsed = false;
        PossibleTiles = Resources.LoadAll<Tile>("TileTypes").ToList();
    }



    ///<summary>
    ///Adds instance to a cell. Does not create the instance to the game
    ///</summary>
    public void AddInstance(GameObject instance)
    {
        _instance = instance;
    }

    ///<summary>
    ///Calculates and returns cell Shannon entropy.
    ///</summary>
    public float GetCellEntropy()
    {
        float entropy = 0.0f;

        foreach (Tile tile in PossibleTiles)
        {
            if (tile.GetTileWeight() > 0)
            {
                entropy -= tile.GetTileWeight() * Mathf.Log(tile.GetTileWeight(), 2);
            }
        }

        //return entropy + _entropyModifier;
        return entropy;
    }

    public Vector3 GetWorldSpacePosition()
    {
        Vector3 worldSpacePosition = new Vector3(Index.x * _size, 0, Index.y * _size);

        return worldSpacePosition; 
    }

    ///<summary>
    ///Returns string with cell information in human readable form 
    ///</summary>
    public override string ToString()
    {
        return "Position(" + Index.x + ":" + Index.y + ") Tile(" + Tile + ")";
    }

    ///<summary>
    ///Updates cell neighbours with which tiles they collapse into.
    ///</summary>
    public void NotifyNeighbours(Cell[,] parentGrid)
    {
        //Top neighbour update
        if (Index.y + 1 < parentGrid.GetLength(1))
        {
            if (!parentGrid[Index.x, Index.y + 1].IsCollapsed)
            {
                parentGrid[Index.x, Index.y + 1].PossibleTiles = Tile.GetNeighbors(ESides.Up).ToList();
                Debug.DrawLine(GetWorldSpacePosition(), parentGrid[Index.x, Index.y + 1].GetWorldSpacePosition(), Color.green, 0.5f, false);
            }
        }

        //Bottom neighbour update
        if (Index.y - 1 > 0)
        {
            if (!parentGrid[Index.x, Index.y - 1].IsCollapsed)
            {
                parentGrid[Index.x, Index.y - 1].PossibleTiles = Tile.GetNeighbors(ESides.Down).ToList();
                Debug.DrawLine(GetWorldSpacePosition() , parentGrid[Index.x, Index.y - 1].GetWorldSpacePosition(), Color.green, 0.5f, false);

            }
        }

        //Right neighbour update
        if (Index.x + 1 < parentGrid.GetLength(0))
        {
            if (!parentGrid[Index.x + 1, Index.y].IsCollapsed)
            {
                parentGrid[Index.x + 1, Index.y].PossibleTiles = Tile.GetNeighbors(ESides.Right).ToList();
                Debug.DrawLine(GetWorldSpacePosition(), parentGrid[Index.x + 1, Index.y].GetWorldSpacePosition(), Color.green, 0.5f, false);

            }
        }

        //Left neighbour update
        if (Index.x - 1 > 0)
        {

            if (!parentGrid[Index.x - 1, Index.y].IsCollapsed)
            {

                parentGrid[Index.x - 1, Index.y].PossibleTiles = Tile.GetNeighbors(ESides.Left).ToList();
                Debug.DrawLine(GetWorldSpacePosition(), parentGrid[Index.x - 1, Index.y].GetWorldSpacePosition(), Color.green, 0.5f, false);

            }

        }

    }



    ///<summary>
    ///Collapses the cell with the tile based on probability. 
    ///</summary>
    public void Collapse(Cell[,] parentGrid, List<Cell> touchedCells)
    {
        List<Tile> collapsePossibilities = PossibleTiles;
        if (IsCollapsed) return;

        //Checking neighbours to see how the cell can collapse
        //Top
        if (Index.y + 1 < parentGrid.GetLength(1))
        {
            if (parentGrid[Index.x, Index.y + 1].IsCollapsed)
            {
                collapsePossibilities = collapsePossibilities.AsQueryable().Intersect(parentGrid[Index.x, Index.y + 1].Tile.GetNeighbors(ESides.Down)).ToList();
            }
            else parentGrid[Index.x, Index.y + 1].AddCellToSortedList(touchedCells);
        }

        //Bottom
        if (Index.y - 1 > 0)
        {
            if (parentGrid[Index.x, Index.y - 1].IsCollapsed)
            {
                collapsePossibilities = collapsePossibilities.AsQueryable().Intersect(parentGrid[Index.x, Index.y - 1].Tile.GetNeighbors(ESides.Up)).ToList();
            }
            else parentGrid[Index.x, Index.y - 1].AddCellToSortedList(touchedCells);
        }

        //Right
        if (Index.x + 1 < parentGrid.GetLength(0))
        {
            if (parentGrid[Index.x + 1, Index.y].IsCollapsed)
            {
                collapsePossibilities = collapsePossibilities.AsQueryable().Intersect(parentGrid[Index.x + 1, Index.y].Tile.GetNeighbors(ESides.Left)).ToList();
            }
            else parentGrid[Index.x + 1, Index.y].AddCellToSortedList(touchedCells);
        }

        //Left
        if (Index.x - 1 > 0)
        {

            if (parentGrid[Index.x - 1, Index.y].IsCollapsed)
            {
                collapsePossibilities = collapsePossibilities.AsQueryable().Intersect(parentGrid[Index.x - 1, Index.y].Tile.GetNeighbors(ESides.Right)).ToList();
            }
            else parentGrid[Index.x - 1, Index.y].AddCellToSortedList(touchedCells);

        }


        if (collapsePossibilities.Count > 0)
        {
            //Calculating probability 
            float max_probability = 0.0f;
            foreach (Tile tile in collapsePossibilities)
            {
                max_probability += tile.GetTileWeight();   
            }

            float diceRoll = UnityEngine.Random.Range(0.0f, max_probability);

            float cumulative = 0.0f;

            for (int i = 0; i < collapsePossibilities.Count; i++)
            {
                cumulative += collapsePossibilities[i].GetTileWeight();

                if (diceRoll < cumulative)
                {
                    Tile = collapsePossibilities[i];
                    break;
                }
            }

        }
        else
        {
            //No possible tile to collapse. Spawns error tile to be dealt with later. 
            Tile = Resources.Load<Tile>("TileTypes/ErrorTile/Error");
        }
        
        IsCollapsed = true;


        NotifyNeighbours(parentGrid);
    }

    private void AddCellToSortedList(List<Cell> sortedList)
    {
        for (int i = 0; i < sortedList.Count; i++)
        {
            Cell otherCell = sortedList[i];

            if (this.CompareTo(otherCell) == 1)
            {
                sortedList.Insert(i, this);
                return;
            }
        }

        sortedList.Add(this);
    }

    public int CompareTo(Cell other)
    {
        float thisEntropy = this.GetCellEntropy();
        float otherEntropy = other.GetCellEntropy();    

        // Object is less if entropy higher 
        if (otherEntropy < thisEntropy) return -1;
        if (Mathf.Approximately(thisEntropy, otherEntropy)) return 0;

        return 1;
    }
}
