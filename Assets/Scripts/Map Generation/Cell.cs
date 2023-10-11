using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class Cell 
{
    public List<Tile> PossibleTiles;

    public bool IsCollapsed { get; private set; } = false;

    public Tile Tile { get; private set; } = null;

    public Vector2 _position;

    float _entropyModifier;

    GameObject _instance;



    public Cell(Vector2 position)
    {
        PossibleTiles = Resources.LoadAll<Tile>("TileTypes").ToList();
        _position = position;
        _entropyModifier = Random.Range(0, 0.02f);
    }

    public void DestroyCell()
    {
        Object.Destroy(_instance, 0.1f);
        Tile = null;
        _entropyModifier = Random.Range(0, 0.02f);
        IsCollapsed = false;
        PossibleTiles = Resources.LoadAll<Tile>("TileTypes").ToList();
    }

    public void AddInstance(GameObject instance)
    {
        _instance = instance;
    }

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

    public void NotifyNeighbours(Cell[,] parentGrid)
    {
        //Top
        if (_position.y + 1 < parentGrid.GetLength(1))
        {
            if (!parentGrid[(int)_position.x, (int)_position.y + 1].IsCollapsed)
            {
                parentGrid[(int)_position.x, (int)_position.y + 1].PossibleTiles = Tile.GetNeighbors(Sides.Up).ToList();
                Debug.DrawLine(new Vector3(_position.x, 0, _position.y), new Vector3(_position.x, 0, _position.y + 1), Color.green, 0.5f, false);
            }
        }

        //Bottom
        if (_position.y - 1 > 0)
        {
            if (!parentGrid[(int)_position.x, (int)_position.y - 1].IsCollapsed)
            {
                parentGrid[(int)_position.x, (int)_position.y - 1].PossibleTiles = Tile.GetNeighbors(Sides.Down).ToList();
                Debug.DrawLine(new Vector3(_position.x, 0, _position.y), new Vector3(_position.x, 0, _position.y - 1), Color.green, 0.5f, false);

            }
        }

        //Right
        if (_position.x + 1 < parentGrid.GetLength(0))
        {
            if (!parentGrid[(int)_position.x + 1, (int)_position.y].IsCollapsed)
            {
                parentGrid[(int)_position.x + 1, (int)_position.y].PossibleTiles = Tile.GetNeighbors(Sides.Right).ToList();
                Debug.DrawLine(new Vector3(_position.x, 0, _position.y), new Vector3(_position.x + 1, 0, _position.y), Color.green, 0.5f, false);

            }
        }

        //Left
        if (_position.x - 1 > 0)
        {

            if (!parentGrid[(int)_position.x - 1, (int)_position.y].IsCollapsed)
            {

                parentGrid[(int)_position.x - 1, (int)_position.y].PossibleTiles = Tile.GetNeighbors(Sides.Left).ToList();
                Debug.DrawLine(new Vector3(_position.x, 0, _position.y), new Vector3(_position.x - 1, 0, _position.y), Color.green, 0.5f, false);

            }

        }

    }

    public void Collapse(Cell[,] parentGrid)
    {
        List<Tile> collapsePossibilities = PossibleTiles;
        if (IsCollapsed) return;

        //Top
        if (_position.y + 1 < parentGrid.GetLength(1))
        {
            if (parentGrid[(int)_position.x, (int)_position.y + 1].IsCollapsed)
            {
                collapsePossibilities = collapsePossibilities.AsQueryable().Intersect(parentGrid[(int)_position.x, (int)_position.y + 1].Tile.GetNeighbors(Sides.Down)).ToList();
            }
        }

        //Bottom
        if (_position.y - 1 > 0)
        {
            if (parentGrid[(int)_position.x, (int)_position.y - 1].IsCollapsed)
            {
                collapsePossibilities = collapsePossibilities.AsQueryable().Intersect(parentGrid[(int)_position.x, (int)_position.y - 1].Tile.GetNeighbors(Sides.Up)).ToList();
            }
        }

        //Right
        if (_position.x + 1 < parentGrid.GetLength(0))
        {
            if (parentGrid[(int)_position.x + 1, (int)_position.y].IsCollapsed)
            {
                collapsePossibilities = collapsePossibilities.AsQueryable().Intersect(parentGrid[(int)_position.x + 1, (int)_position.y].Tile.GetNeighbors(Sides.Left)).ToList();
            }
        }

        //Left
        if (_position.x - 1 > 0)
        {

            if (parentGrid[(int)_position.x - 1, (int)_position.y].IsCollapsed)
            {
                collapsePossibilities = collapsePossibilities.AsQueryable().Intersect(parentGrid[(int)_position.x - 1, (int)_position.y].Tile.GetNeighbors(Sides.Right)).ToList();
            }

        }


        if (collapsePossibilities.Count > 0)
        {

            float max_prabability = 0.0f;
            foreach (Tile tile in collapsePossibilities)
            {
                max_prabability += tile.GetTileWeight();   
            }

            float diceRoll = Random.Range(0.0f, max_prabability);

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
            Tile = Resources.Load<Tile>("TileTypes/ErrorTile/Error");
        }
        
        IsCollapsed = true;

        NotifyNeighbours(parentGrid);
    }

}
