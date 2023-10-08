using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;


public class WaveFunctionCollapse : MonoBehaviour
{
    [SerializeField]
    private int _gridSizeX = 0;

    [SerializeField]
    private int _gridSizeY = 0;

    private Cell[,] grid;

    private List<Cell> _emptyCells;

    [SerializeField]
    private Sprite _patternImage;

    [SerializeField]
    private List<Color2Tile> _colorTiles;

    private Tile _generationFailure;



    // Start is called before the first frame update
    void Start()
    {
        grid = new Cell[_gridSizeX, _gridSizeY];
        _emptyCells = new List<Cell>();

        for (int x = 0;  x < _gridSizeX; x++)
        {
            for (int y = 0;  y < _gridSizeY; y++)
            {
                grid[x, y] = new Cell(new Vector2(x,y));
                _emptyCells.Add(grid[x, y]);
            }
        }

        List<Tile> allPossibleTiles = new List<Tile>();
       
        _generationFailure = Resources.Load<Tile>("TileTypes/ErrorTile/Error");

        foreach (Color2Tile colorTile in _colorTiles)
        {
            allPossibleTiles.Add(colorTile.tile);
        }

        PatternExtractor patternExtractor = new PatternExtractor(_patternImage, _colorTiles);

        Cell startingCell = grid[Random.Range(0, _gridSizeX), Random.Range(0, _gridSizeY)];

        startingCell.Collapse(grid);
        
        Instantiate(startingCell.Tile.GetPrfab(), new Vector3(startingCell._position.x, 0, startingCell._position.y), Quaternion.identity, gameObject.transform);

        _emptyCells.Remove(startingCell);


        StartCoroutine(SpawnCells());

    }

    IEnumerator DestroyFailures()
    {
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                if (grid[x, y].Tile == _generationFailure)
                {
                    grid[x, y].DestroyCell();
                    _emptyCells.Add(grid[x, y]);

                    //Right
                    if (x + 1 < _gridSizeX)
                    {
                        grid[x + 1, y].DestroyCell();
                        _emptyCells.Add(grid[x + 1, y]);
                    }

                    //Left
                    if (x - 1 >= 0) 
                    {
                        grid[x - 1, y].DestroyCell();
                        _emptyCells.Add(grid[x - 1, y]);
                    }

                    //Up
                    if (y + 1 < _gridSizeY)
                    {
                        grid[x, y + 1].DestroyCell();
                        _emptyCells.Add(grid[x, y + 1]);
                    }

                    //Down
                    if (y - 1 >= 0)
                    {
                        grid[x, y - 1].DestroyCell();
                        _emptyCells.Add(grid[x, y - 1]);
                    }


                    yield return new WaitForSeconds(0.05f);

                }

            }
        }

        if (_emptyCells.Count > 0)
        {
            StartCoroutine(SpawnCells());
        }
    }

    IEnumerator SpawnCells()
    {

        while (_emptyCells.Count > 0)
        {
            _emptyCells.Sort((x, y) => x.GetCellEntropy().CompareTo(y.GetCellEntropy()));

            Cell currentCell = _emptyCells[0];

            currentCell.Collapse(grid);

            currentCell.AddInstance(Instantiate(currentCell.Tile.GetPrfab(), new Vector3(currentCell._position.x, 0, currentCell._position.y), Quaternion.Euler(new Vector3(0, currentCell.Tile.RotationInDegrees, 0)), gameObject.transform));
       
            _emptyCells.Remove(currentCell);

            yield return new WaitForSeconds(0.05f);
        }

        StartCoroutine(DestroyFailures());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
