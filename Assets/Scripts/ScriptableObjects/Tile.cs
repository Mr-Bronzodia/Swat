using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tiles")]
public class Tile : ScriptableObject
{
    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private List<Tile> _upNeighbors;

    [SerializeField]
    private List<Tile> _downNeighbors;

    [SerializeField]
    private List<Tile> _rightNeighbors;

    [SerializeField]
    private List<Tile> _leftNeighbors;

    [SerializeField]
    [Range(0f, 1f)]
    private float Weight;

    public float RotationInDegrees = 0;

    public bool AllowForRotationVariants;
    public bool AllowSelfConnection;
    public bool TopConnection;
    public bool BottomConnection;
    public bool LeftConnection;
    public bool RightConnection;


    ///<summary>
    ///Clears connection setting for a tile.
    ///This will change the asset permanently.
    ///</summary>
    public void Clear()
    {
        _upNeighbors.Clear();
        _downNeighbors.Clear();
        _rightNeighbors.Clear();
        _leftNeighbors.Clear();
        //Weight = 0f;
    }


    ///<summary>
    ///Returns prefab associated with the tile
    ///</summary>
    public GameObject GetPrefab()
    {
        return _prefab;
    }

    ///<summary>
    ///Sets prefab for a tile.
    ///This will change the asset permanently.
    ///</summary>
    public void SetPrefab(GameObject prefab)
    {
        _prefab = prefab;
    }


    ///<summary>
    ///Adds amount to tile weight 
    ///This will change the asset permanently.
    ///</summary>
    public void AddWeight(float amount)
    {
        Weight += amount;
    }

    ///<summary>
    ///Returns tile weight.
    ///Weight is a likelihood the tile is selected during the collapse of the cell
    ///</summary>
    public float GetTileWeight()
    {
        return Weight;
    }

    ///<summary>
    ///Adds possible neighbour connection to a tile if its not already there.
    ///This will change the asset permanently 
    ///</summary>
    ///<param name="side">Which side to add the neighbour to.</param>
    ///<param name="Neighbors">Tile type</param>
    public void AddNeighbors(ESides side,Tile Neighbors)
    {
        switch (side)
        {
            case ESides.Up:
                if (!_upNeighbors.Contains(Neighbors)) _upNeighbors.Add(Neighbors);
                break;
            case ESides.Down:
                if (!_downNeighbors.Contains(Neighbors)) _downNeighbors.Add(Neighbors);
                break;
            case ESides.Left:
                if (!_leftNeighbors.Contains(Neighbors)) _leftNeighbors.Add(Neighbors);
                break;
            case ESides.Right:
                if (!_rightNeighbors.Contains(Neighbors)) _rightNeighbors.Add(Neighbors);
                break;
            default:
                Debug.LogError("Invalid side passed in Tile.cs");
                throw new System.Exception("Invalid argument passed to Tile");

        }
    }
    ///<summary>
    ///Returns all possible neighbours connections for this tile.
    ///</summary>
    public List<Tile> GetNeighbors(ESides side)
    {
        List<Tile> result = new List<Tile>();

        switch (side)
        {
            case ESides.Up:
                result = _upNeighbors;
                break;
            case ESides.Down:
                result = _downNeighbors;
                break;
            case ESides.Left:
                result = _leftNeighbors;
                break;
            case ESides.Right:
                result = _rightNeighbors;
                break;
            default:
                Debug.LogError("Invalid side passed in Tile.cs");
                throw new System.Exception("Invalid argument passed to Tile");
                
        }

        return result;
    }
}
