using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;
using UnityEngine.Profiling;
using UnityEngine.AI;
using Unity.AI.Navigation;



public class WaveFunctionCollapse : MonoBehaviour, ISubscriber
{
    [Header("Settings")]
    [SerializeField] 
    private bool RegenerateConnections;
    [SerializeField]
    private bool GenerateTileVariants;
    [SerializeField]
    private bool RegenrateOnPlay;
    [SerializeField]
    private bool BuildNavMeshOnPlay;

    [Header("Grid Size")]
    [SerializeField]
    private int _gridSizeX = 0;

    [SerializeField]
    private int _gridSizeY = 0;

    [Header("Cells")]
    [SerializeField]
    private List<Tile> _startingTiles;

    private Tile _generationFailure;

    [SerializeField]
    public int CellSize;

    private Cell[,] _grid;
    private List<Cell> _emptyCells;
    private List<Cell> _touchedCells;



    void Start()
    {
        if (RegenrateOnPlay) GenerateTilemap();
    }

    public void GenerateTilemap()
    {
        if (_grid != null) DestroyGrid();

        Subscribe();
        Profiler.BeginSample("Generation Setup");
        //Initializes empty cell grid
        _grid = new Cell[_gridSizeX, _gridSizeY];
        _emptyCells = new List<Cell>();
        _touchedCells = new List<Cell>(450);

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                _grid[x, y] = new Cell(new Vector2Int(x, y), CellSize);
                _grid[x, y] = new Cell(new Vector2Int(x, y), CellSize);
                _emptyCells.Add(_grid[x, y]);
            }
        }

        _generationFailure = Resources.Load<Tile>("TileTypes/ErrorTile/Error");


        PatternExtractor patternExtractor = new PatternExtractor(_startingTiles);

        if (GenerateTileVariants) patternExtractor.GenerateRotationVariants();

        if (RegenerateConnections)
        {
            foreach (Tile tile in _startingTiles) tile.Clear();

            patternExtractor.Extract();
        }

        //Collapses first cell to start the propagation. Cell position is random 
        Cell startingCell = _grid[UnityEngine.Random.Range(0, _gridSizeX), UnityEngine.Random.Range(0, _gridSizeY)];


        startingCell.Collapse(_grid, _touchedCells);

        Instantiate(startingCell.Tile.GetPrefab(), new Vector3(startingCell.Index.x * CellSize, 0, startingCell.Index.y * CellSize), Quaternion.identity, gameObject.transform);

        _emptyCells.Remove(startingCell);

        Profiler.EndSample();

        Profiler.BeginSample("Spawning Cells");
        SpawnCells();
        Profiler.EndSample();
    }

    /// <summary>
    /// Looks for error tiles, destroys them replacing them and their neighbours. 
    /// </summary>
    /// <returns></returns>
    private void DestroyFailures(List<Cell> touchedCells)
    {
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                if (_grid[x, y].Tile == _generationFailure)
                {
                    _grid[x, y].DestroyCell();
                    _emptyCells.Add(_grid[x, y]);
                    touchedCells.Add(_grid[x, y]);

                    //Right
                    if (x + 1 < _gridSizeX)
                    {
                        _grid[x + 1, y].DestroyCell();
                        _emptyCells.Add(_grid[x + 1, y]);
                        touchedCells.Add(_grid[x + 1, y]);
                    }

                    //Left
                    if (x - 1 >= 0) 
                    {
                        _grid[x - 1, y].DestroyCell();
                        _emptyCells.Add(_grid[x - 1, y]);
                        touchedCells.Add(_grid[x - 1, y]);
                    }

                    //Up
                    if (y + 1 < _gridSizeY)
                    {
                        _grid[x, y + 1].DestroyCell();
                        _emptyCells.Add(_grid[x, y + 1]);
                        touchedCells.Add(_grid[x, y + 1]);
                    }

                    //Down
                    if (y - 1 >= 0)
                    {
                        _grid[x, y - 1].DestroyCell();
                        _emptyCells.Add(_grid[x, y - 1]);
                        touchedCells.Add(_grid[x, y - 1]);
                    }
                }
            }
        }


        if (touchedCells.Count > 0)
        {
            SpawnCells(); // Regenerates destroyed cells 
        }
        else
        {
            NotifyTaskCompleted();
        }
    }


    public void DestroyGrid()
    {
        if (_emptyCells != null)_emptyCells.Clear();
        if (_touchedCells != null) _touchedCells.Clear();
        if (_grid != null) _grid = null;

        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        WorldStateManager.Instance.UpdateWorldState(WorldState.Empty);
    }



    /// <summary>
    /// Spawns cells from lowest entropy to highest. Cell is collapsed based on their weight. High weight = more likely to collapse into. 
    /// </summary>
    /// <returns></returns>

    private void SpawnCells()
    {

        Profiler.BeginSample("sorting");
        _touchedCells.Sort((x, y) => x.GetCellEntropy().CompareTo(y.GetCellEntropy()));
        Profiler.EndSample();

        while (_touchedCells.Count > 0)
        {
            if (_touchedCells[0].Tile != null)
            {
                _touchedCells.RemoveAt(0);
                continue;
            }

            Cell currentCell = _touchedCells[0];

            currentCell.Collapse(_grid, _touchedCells);

            currentCell.AddInstance(Instantiate(currentCell.Tile.GetPrefab(), new Vector3(currentCell.Index.x * CellSize, 0, currentCell.Index.y * CellSize), Quaternion.Euler(new Vector3(0, currentCell.Tile.RotationInDegrees, 0)), gameObject.transform));

            Profiler.BeginSample("removing cell");
            _emptyCells.Remove(currentCell);
            _touchedCells.Remove(currentCell);
            Profiler.EndSample();
        }


        _touchedCells.Clear();

        Profiler.BeginSample("Destroying failures");
        DestroyFailures(_touchedCells);
        Profiler.EndSample();
    }

    /// <summary>
    /// Returns world grid.
    /// </summary>
    /// <returns></returns>
    public Cell[,] GetGrid()
    {
        return _grid;
    }

    public void Subscribe()
    {
        WorldStateManager.Instance.AddSubscriber();
    }

    public void NotifyTaskCompleted()
    {
        WorldStateManager.Instance.NotifyComplete();
        WorldStateManager.Instance.UpdateWorldState(WorldState.MapGenerated);
    }
}
