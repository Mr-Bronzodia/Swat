using Codice.Client.BaseCommands.BranchExplorer;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FurnitureGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject empty;

    private List<GameObject> _roomParents;
    private Furniture[] _furniture;

    private static string FURNITUREDIR = "Furniture";
    private static float SMALL = 20f;
    private static float MEDIUM = 30f;
    private static float BIG = 40f;

    private void OnEnable()
    {
        gameObject.GetComponent<InteriorGenerator>().OnRoomsGenerated += GenerateFurniture;
        _furniture = Resources.LoadAll<Furniture>(FURNITUREDIR);
    }

    private void OnDisable()
    {
        gameObject.GetComponent<InteriorGenerator>().OnRoomsGenerated -= GenerateFurniture;
    }

    private void GenerateFurniture()
    {
        List<Room> rooms = gameObject.GetComponent<InteriorGenerator>().Rooms;
        _roomParents = new List<GameObject>();

        foreach (Room room in rooms)
        {
            switch (room.RoomType)
            {
                case RoomTypes.Root:
                    break;
                case RoomTypes.Kitchen:
                    break;
                case RoomTypes.Bedroom:
                    break;
                case RoomTypes.Bathroom:
                    GenerateBathroom(room);
                    break;
                case RoomTypes.Connector:
                    break;
                case RoomTypes.Livingroom:
                    break;
                case RoomTypes.StorageArea:
                    break;
                case RoomTypes.Laundry:
                    break;
                case RoomTypes.Office:
                    break;
            }
        }
    }

    private List<Furniture> FindFurnitureByTag(RoomTypes roomType, ObjectTag objectTag, List<DescriptorTags> descriptorTags, SearchMode mode)
    {
        List<Furniture> results = new List<Furniture>();

        switch (mode)
        {
            case SearchMode.RequireAll:

                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    bool shouldAddFurniture = true;

                    foreach (DescriptorTags tag in descriptorTags)
                    {
                        if (!_furniture[i].DescriptorTags.Contains(tag))
                        {
                            shouldAddFurniture = false;
                        }
                    }

                    if (shouldAddFurniture) results.Add(_furniture[i]);
                }

                    break;
            case SearchMode.RequireOne:
                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    foreach (DescriptorTags tag in descriptorTags)
                    {
                        if (_furniture[i].DescriptorTags.Contains(tag))
                        {
                            results.Add(_furniture[i]);
                            break;
                        }
                    }
                }
                break;
            case SearchMode.BlackList:
                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    bool shouldAddFurniture = true;

                    foreach (DescriptorTags tag in descriptorTags)
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

    private static Furniture GetRandom(List<Furniture> furnitureList)
    {
        return furnitureList[Random.Range(0, furnitureList.Count)];
    }


    private void GenerateBathroom(Room bathroom)
    {
        GameObject parentRoomInstance = Instantiate(empty, bathroom.Bounds.center, Quaternion.identity, gameObject.transform);
        
        Vector3 toiletLocation = Vector3.zero;

        List<Wall> emptyWalls = new List<Wall>();
        foreach (KeyValuePair<Sides, Wall> wall in bathroom.Walls)
        {
            if (!wall.Value.ContainsDoor()) 
            {
                emptyWalls.Add(wall.Value);
            }
        }

        emptyWalls.Sort((x,y) => x.Length.CompareTo(y.Length));

        List<DescriptorTags> descriptors = new List<DescriptorTags>() { DescriptorTags.Small };

        Furniture toilet = GetRandom(FindFurnitureByTag(RoomTypes.Bathroom,
                                                        ObjectTag.Toilet,
                                                        descriptors,
                                                        SearchMode.RequireOne));

        SpawnAdjustedToWall(toilet, bathroom, emptyWalls[0], parentRoomInstance, 0f, .2f);

        Debug.Log(bathroom.Size);

        Furniture bath = GetRandom(FindFurnitureByTag(RoomTypes.Bathroom, ObjectTag.Shower, descriptors, SearchMode.RequireOne));

        SpawnAdjustedToWall(bath, bathroom, bathroom.Walls[Wall.GetOppositeSide(emptyWalls[0].Side)], parentRoomInstance, 0f, .8f);
    }

    private void SpawnAdjustedToWall(Furniture furniture, Room room, Wall wall, GameObject parent, float wallMargin, float wallSlide)
    {
        GameObject furnitureInstance = Instantiate(furniture.Prefab, wall.MiddlePoint, Quaternion.identity, parent.transform);
        float furnitureLength = furnitureInstance.GetComponentInChildren<MeshRenderer>().bounds.size.z;
        furnitureInstance.transform.position = Vector3.Lerp(wall.StartPoint, wall.EndPoint, wallSlide) - ((furnitureLength / 2) + wallMargin) * wall.GetInsideVector(room);

        furnitureInstance.transform.forward = wall.GetInsideVector(room);
    }
}