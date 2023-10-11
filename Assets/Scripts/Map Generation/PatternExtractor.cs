using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEditor;
using UnityEngine;
using System.IO;

public class PatternExtractor 
{
    private List<Tile> _protoTiles;
    private Tile fillTile;

    public PatternExtractor(List<Tile> protoTiles)
    {
        _protoTiles = protoTiles;

        foreach (Tile tile in _protoTiles)
        {
            if (!tile.TopConnection && !tile.BottomConnection && !tile.RightConnection && !tile.LeftConnection)  fillTile = tile;
        }
    }

    public void GenerateRotationVariants()
    {
        List<Tile> newTiles = new List<Tile>();

        foreach (Tile tile in _protoTiles)
        {
            if (!tile.AllowForRoatationVariants) continue;

            for (int i = 1; i <= 3; i++)
            {
                Tile rotationVariant = ScriptableObject.CreateInstance<Tile>();
                string path = "Assets/Resources/TileTypes/" + tile.name + (i * 90) + ".asset";

                rotationVariant.SetPrefab(tile.GetPrfab());

                rotationVariant.AddWeight(tile.GetTileWeight());

                rotationVariant.RotationInDegrees = i * 90.0f;

                rotationVariant.AllowForRoatationVariants = false;
                rotationVariant.AllowSelfConnection = tile.AllowSelfConnection;

                switch (i * 90.0f)
                {
                    case 90:
                        if (tile.TopConnection) rotationVariant.RightConnection = true;
                        if (tile.RightConnection) rotationVariant.BottomConnection = true;
                        if (tile.BottomConnection) rotationVariant.LeftConnection = true;
                        if (tile.LeftConnection) rotationVariant.TopConnection = true;
                        break;
                    case 180:
                        if (tile.TopConnection) rotationVariant.BottomConnection = true;
                        if (tile.RightConnection) rotationVariant.LeftConnection = true;
                        if (tile.BottomConnection) rotationVariant.TopConnection = true;
                        if (tile.LeftConnection) rotationVariant.RightConnection = true;
                        break;
                    case 270:
                        if (tile.TopConnection) rotationVariant.LeftConnection = true;
                        if (tile.LeftConnection) rotationVariant.BottomConnection = true;
                        if (tile.BottomConnection) rotationVariant.RightConnection = true;
                        if (tile.RightConnection) rotationVariant.TopConnection = true;
                        break;
                }

                AssetDatabase.CreateAsset(rotationVariant, path);
                newTiles.Add(rotationVariant);
            }

        }

        _protoTiles = _protoTiles.Concat(newTiles).ToList();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void Extract()
    {

        foreach (Tile tile in _protoTiles)
        {

            foreach (Tile other in _protoTiles)
            {

                if (other == tile && !tile.AllowSelfConnection) continue;

                //Top Connection Match
                if (tile.TopConnection && other.BottomConnection)
                {
                    tile.AddNeighbors(Sides.Up, other);
                    other.AddNeighbors(Sides.Down, tile);
                }

                //if (!tile.TopConnection && !other.BottomConnection)
                //{
                //    tile.AddNeighbors(Sides.Up, other);
                //    other.AddNeighbors(Sides.Down, tile);
                //}

                if (!tile.TopConnection)
                {
                    tile.AddNeighbors(Sides.Up, fillTile);
                    fillTile.AddNeighbors(Sides.Down, tile);
                }

                //Bottom Connection Match
                if (tile.BottomConnection && other.TopConnection)
                {
                    tile.AddNeighbors(Sides.Down, other);
                    other.AddNeighbors(Sides.Up, tile);
                }

                //if (!tile.BottomConnection && !other.TopConnection)
                //{
                //    tile.AddNeighbors(Sides.Down, other);
                //    other.AddNeighbors(Sides.Up, tile);
                //}

                if (!tile.BottomConnection)
                {
                    tile.AddNeighbors(Sides.Down, fillTile);
                    fillTile.AddNeighbors(Sides.Up, tile);
                }


                //Right Connection Match
                if (tile.RightConnection && other.LeftConnection)
                {
                    tile.AddNeighbors(Sides.Right, other);
                    other.AddNeighbors(Sides.Left, tile);
                }

                //if (!tile.RightConnection && !other.LeftConnection)
                //{
                //    tile.AddNeighbors(Sides.Right, other);
                //    other.AddNeighbors(Sides.Left, tile);
                //}


                if (!tile.RightConnection)
                {
                    tile.AddNeighbors(Sides.Right, fillTile);
                    fillTile.AddNeighbors(Sides.Left, tile);
                }

                //Left Connection Match
                if (tile.LeftConnection && other.RightConnection)
                {
                    tile.AddNeighbors(Sides.Left, other);
                    other.AddNeighbors(Sides.Right, tile);
                }


                //if (!tile.LeftConnection && !other.RightConnection)
                //{
                //    tile.AddNeighbors(Sides.Left, other);
                //    other.AddNeighbors(Sides.Right, tile);
                //}

                if (!tile.LeftConnection)
                {
                    tile.AddNeighbors(Sides.Left, fillTile);
                    fillTile.AddNeighbors(Sides.Right, tile);
                }
            }
        }
    }


}
