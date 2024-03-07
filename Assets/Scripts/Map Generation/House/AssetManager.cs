using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance;

    private static string FURNITUREDIR = "Furniture";
    private Furniture[] _furniture;

    private void Awake()
    {
        _furniture = Resources.LoadAll<Furniture>(FURNITUREDIR);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private List<Furniture> FindFurnitureByTag(ERoomTypes roomType, EObjectTag objectTag, List<EDescriptorTags> descriptorTags, ESearchMode mode)
    {
        List<Furniture> results = new List<Furniture>();

        switch (mode)
        {
            case ESearchMode.RequireAll:

                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    bool shouldAddFurniture = true;

                    foreach (EDescriptorTags tag in descriptorTags)
                    {
                        if (!_furniture[i].DescriptorTags.Contains(tag))
                        {
                            shouldAddFurniture = false;
                        }
                    }

                    if (shouldAddFurniture) results.Add(_furniture[i]);
                }

                break;
            case ESearchMode.RequireOne:
                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    foreach (EDescriptorTags tag in descriptorTags)
                    {
                        if (_furniture[i].DescriptorTags.Contains(tag))
                        {
                            results.Add(_furniture[i]);
                            break;
                        }
                    }
                }
                break;
            case ESearchMode.BlackList:
                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    bool shouldAddFurniture = true;

                    foreach (EDescriptorTags tag in descriptorTags)
                    {
                        if (_furniture[i].DescriptorTags.Contains(tag))
                        {
                            shouldAddFurniture = false;
                        }
                    }

                    if (shouldAddFurniture) results.Add(_furniture[i]);
                }
                break;
        }

        return results;
    }

    private List<Furniture> FindAllForRoom(ERoomTypes roomType, EObjectTag objectTag)
    {
        List<Furniture> results = new List<Furniture>();

        for (int i = 0; i < _furniture.Length; i++)
        {
            if (!_furniture[i].RoomTags.Contains(roomType)) continue;
            if (_furniture[i].ObjectTag != objectTag) continue;

            results.Add(_furniture[i]);
        }

        return results;
    }

    public List<Furniture> FindAssetByTag(ERoomTypes roomType, EObjectTag objectTag, EDescriptorTags descriptorTags, ESearchMode mode)
    {
        List<Furniture> results = new List<Furniture>();

        switch (mode)
        {
            case ESearchMode.RequireAll:

                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(ERoomTypes.Any) && !_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    if (_furniture[i].DescriptorTags.Contains(descriptorTags)) results.Add(_furniture[i]);
                }

                break;
            case ESearchMode.RequireOne:
                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(ERoomTypes.Any) && !_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    if (_furniture[i].DescriptorTags.Contains(descriptorTags)) results.Add(_furniture[i]);
                }
                break;
            case ESearchMode.BlackList:
                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(ERoomTypes.Any) && !_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    if (!_furniture[i].DescriptorTags.Contains(descriptorTags)) results.Add(_furniture[i]);
                }
                break;
        }

        return results;
    }

    public static Furniture GetRandom(List<Furniture> furnitureList)
    {
        return furnitureList[UnityEngine.Random.Range(0, furnitureList.Count)];
    }
}
