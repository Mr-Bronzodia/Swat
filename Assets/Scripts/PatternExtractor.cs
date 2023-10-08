using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEditor;
using UnityEngine;

public class PatternExtractor 
{
    Sprite _patternMap;
    List<Color2Tile> _translationList;

    Dictionary<string, Tile> _tileDictionary;

    public PatternExtractor(Sprite patternMap, List<Color2Tile> translationList)
    {
        _patternMap = patternMap;
        _translationList = translationList;
        _tileDictionary = new Dictionary<string, Tile>();

        foreach (Color2Tile translation in translationList)
        {
            _tileDictionary[UnityEngine.ColorUtility.ToHtmlStringRGB(translation.color)] = translation.tile;  
            translation.tile.Clear();
        }

        Texture2D texture = _patternMap.texture;

        float pixelWeight  = 1.0f / (texture.width * texture.height);

        Tile soy = ScriptableObject.CreateInstance<Tile>();

        string path = "Assets/Resources/TileTypes/soy.asset";
        AssetDatabase.CreateAsset(soy, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        for (int i  = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                Color inspectedPixel = texture.GetPixel(i, j);
                string interactedHEX = UnityEngine.ColorUtility.ToHtmlStringRGB(inspectedPixel);


                //Right
                if (i + 1 < texture.width)
                {
                    //if (!_tileDictionary.ContainsKey(texture.GetPixel(i + 1, j))) continue;
                    _tileDictionary[interactedHEX].AddNeighbors(Sides.Right, _tileDictionary[UnityEngine.ColorUtility.ToHtmlStringRGB(texture.GetPixel(i + 1, j))]);
                }

                //Left
                if (i - 1 > 0)
                {
                    //if (!_tileDictionary.ContainsKey(texture.GetPixel(i - 1, j))) continue;
                    _tileDictionary[interactedHEX].AddNeighbors(Sides.Left, _tileDictionary[UnityEngine.ColorUtility.ToHtmlStringRGB(texture.GetPixel(i - 1, j))]);
                }

                //Up
                if (j + 1 < texture.height)
                {
                    //if (!_tileDictionary.ContainsKey(texture.GetPixel(i, j + 1))) continue;
                    _tileDictionary[interactedHEX].AddNeighbors(Sides.Up, _tileDictionary[UnityEngine.ColorUtility.ToHtmlStringRGB(texture.GetPixel(i, j + 1))]);
                }

                //Down
                if (j - 1  > 0)
                {
                    //if (!_tileDictionary.ContainsKey(texture.GetPixel(i, j - 1))) continue;
                    _tileDictionary[interactedHEX].AddNeighbors(Sides.Down, _tileDictionary[UnityEngine.ColorUtility.ToHtmlStringRGB(texture.GetPixel(i, j - 1))]);
                }

                _tileDictionary[interactedHEX].AddWeight(pixelWeight);

            }
        }
    }


}
