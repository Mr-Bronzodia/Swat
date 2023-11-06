using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;


public class WaveFunctionCollapse : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] 
    private bool RegenerateConnections;
    [SerializeField]
    private bool GenerateTileVariants;

    [Header("Grid Size")]
    [SerializeField]
    private int _gridSizeX = 0;

    [SerializeField]
    private int _gridSizeY = 0;

    private Cell[,] _grid;

    private List<Cell> _emptyCells;

    [Header("Cells")]
    [SerializeField]
    private List<Tile> _startingTiles;

    private Tile _generationFailure;

    public Action OnAllCellsCollapsed;



    void Start()
    {
        //Initializes empty cell grid
        _grid = new Cell[_gridSizeX, _gridSizeY];
        _emptyCells = new List<Cell>();

        for (int x = 0;  x < _gridSizeX; x++)
        {
            for (int y = 0;  y < _gridSizeY; y++)
            {
                _grid[x, y] = new Cell(new Vector2(x,y));
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


        startingCell.Collapse(_grid);

        Instantiate(startingCell.Tile.GetPrefab(), new Vector3(startingCell._position.x, 0, startingCell._position.y), Quaternion.identity, gameObject.transform);

        _emptyCells.Remove(startingCell);


        StartCoroutine(SpawnCells());

    }

    /// <summary>
    /// Looks for error tiles, destroys them replacing them and their neighbours. 
    /// </summary>
    /// <returns></returns>
    IEnumerator DestroyFailures()
    {
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                if (_grid[x, y].Tile == _generationFailure)
                {
                    _grid[x, y].DestroyCell();
                    _emptyCells.Add(_grid[x, y]);

                    //Right
                    if (x + 1 < _gridSizeX)
                    {
                        _grid[x + 1, y].DestroyCell();
                        _emptyCells.Add(_grid[x + 1, y]);
                    }

                    //Left
                    if (x - 1 >= 0) 
                    {
                        _grid[x - 1, y].DestroyCell();
                        _emptyCells.Add(_grid[x - 1, y]);
                    }

                    //Up
                    if (y + 1 < _gridSizeY)
                    {
                        _grid[x, y + 1].DestroyCell();
                        _emptyCells.Add(_grid[x, y + 1]);
                    }

                    //Down
                    if (y - 1 >= 0)
                    {
                        _grid[x, y - 1].DestroyCell();
                        _emptyCells.Add(_grid[x, y - 1]);
                    }


                    yield return new WaitForSeconds(0.05f);

                }

            }
        }

        if (_emptyCells.Count > 0)
        {
            StartCoroutine(SpawnCells()); // Regenerates destroyed cells 
        }
        else
        {
            OnAllCellsCollapsed?.Invoke(); // no error tiles detected. Generation is finished.
        }
    }


    /// <summary>
    /// Spawns cells from lowest entropy to highest. Cell is collapsed based on their weight. High weight = more likely to collapse into. 
    /// </summary>
    /// <returns></returns>

    IEnumerator SpawnCells()
    {

        while (_emptyCells.Count > 0)
        {
            _emptyCells.Sort((x, y) => x.GetCellEntropy().CompareTo(y.GetCellEntropy())); //sorts cells based on their entropy. Potential performance concern 

            Cell currentCell = _emptyCells[0];

            currentCell.Collapse(_grid);

            currentCell.AddInstance(Instantiate(currentCell.Tile.GetPrefab(), new Vector3(currentCell._position.x, 0, currentCell._position.y), Quaternion.Euler(new Vector3(0, currentCell.Tile.RotationInDegrees, 0)), gameObject.transform));
       
            _emptyCells.Remove(currentCell);

            yield return new WaitForSeconds(0.005f);
        }

        StartCoroutine(DestroyFailures());
    }

    /// <summary>
    /// Returns world grid.
    /// </summary>
    /// <returns></returns>
    public Cell[,] GetGrid()
    {
        return _grid;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
