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

    public bool AllowForRoatationVariants;
    public bool AllowSelfConnection;
    public bool TopConnection;
    public bool BottomConnection;
    public bool LeftConnection;
    public bool RightConnection;


    public void Clear()
    {
        _upNeighbors.Clear();
        _downNeighbors.Clear();
        _rightNeighbors.Clear();
        _leftNeighbors.Clear();
        //Weight = 0f;
    }

    public GameObject GetPrfab()
    {
        return _prefab;
    }

    public void SetPrefab(GameObject prefab)
    {
        _prefab = prefab;
    }

    public void AddWeight(float amount)
    {
        Weight += amount;
    }

    public float GetTileWeight()
    {
        return Weight;
    }

    public void AddNeighbors(Sides side,Tile Neighbors)
    {
        switch (side)
        {
            case Sides.Up:
                if (!_upNeighbors.Contains(Neighbors)) _upNeighbors.Add(Neighbors);
                break;
            case Sides.Down:
                if (!_downNeighbors.Contains(Neighbors)) _downNeighbors.Add(Neighbors);
                break;
            case Sides.Left:
                if (!_leftNeighbors.Contains(Neighbors)) _leftNeighbors.Add(Neighbors);
                break;
            case Sides.Right:
                if (!_rightNeighbors.Contains(Neighbors)) _rightNeighbors.Add(Neighbors);
                break;
            default:
                Debug.LogError("Invalid side passed in Tile.cs");
                throw new System.Exception("Invalid argument passed to Tile");

        }
    }

    public List<Tile> GetNeighbors(Sides side)
    {
        List<Tile> result = new List<Tile>();

        switch (side)
        {
            case Sides.Up:
                result = _upNeighbors;
                break;
            case Sides.Down:
                result = _downNeighbors;
                break;
            case Sides.Left:
                result = _leftNeighbors;
                break;
            case Sides.Right:
                result = _rightNeighbors;
                break;
            default:
                Debug.LogError("Invalid side passed in Tile.cs");
                throw new System.Exception("Invalid argument passed to Tile");
                
        }

        return result;
    }
}
