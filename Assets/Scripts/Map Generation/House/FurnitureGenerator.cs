using Codice.Client.BaseCommands.BranchExplorer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FurnitureGenerator : MonoBehaviour, ISubscriber
{
    [SerializeField]
    private GameObject empty;

    private List<GameObject> _roomParents;
    private Furniture[] _furniture;

    private static string FURNITUREDIR = "Furniture";
    private static float SMALL = 10f;
    private static float MEDIUM = 20f;
    private static float BIG = 30f;

    [Header("Debug Settings")]
    [SerializeField] 
    private bool _attachCameraToRoom;

    public Action EmergencyRegenerate;

    private void OnEnable()
    {
        gameObject.GetComponent<InteriorGenerator>().OnRoomsGenerated += GenerateFurniture;
        _furniture = Resources.LoadAll<Furniture>(FURNITUREDIR);
        Subscribe();
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
                case ERoomTypes.Root:
                    break;
                case ERoomTypes.Kitchen:
                    break;
                case ERoomTypes.Bedroom:
                    break;
                case ERoomTypes.Bathroom:
                    GenerateBathroom(room);
                    break;
                case ERoomTypes.Connector:
                    break;
                case ERoomTypes.Livingroom:
                    break;
                case ERoomTypes.StorageArea:
                    break;
                case ERoomTypes.Laundry:
                    break;
                case ERoomTypes.Office:
                    break;
            }
        }

        NotifyTaskCompleted();
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

    private List<Furniture> FindFurnitureByTag(ERoomTypes roomType, EObjectTag objectTag, EDescriptorTags descriptorTags, ESearchMode mode)
    {
        List<Furniture> results = new List<Furniture>();

        switch (mode)
        {
            case ESearchMode.RequireAll:

                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    if (_furniture[i].DescriptorTags.Contains(descriptorTags))
                    {
                        results.Add(_furniture[i]);
                        break;
                    }
                }

                break;
            case ESearchMode.RequireOne:
                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                        if (_furniture[i].DescriptorTags.Contains(descriptorTags))
                        {
                            results.Add(_furniture[i]);
                            break;
                        }
                }
                break;
            case ESearchMode.BlackList:
                for (int i = 0; i < _furniture.Length; i++)
                {
                    if (!_furniture[i].RoomTags.Contains(roomType)) continue;

                    if (_furniture[i].ObjectTag != objectTag) continue;

                    if (!_furniture[i].DescriptorTags.Contains(descriptorTags)) results.Add(_furniture[i]);
                }
                break;
        }

        return results;
    }

    private static Furniture GetRandom(List<Furniture> furnitureList)
    {
        return furnitureList[UnityEngine.Random.Range(0, furnitureList.Count)];
    }


    private void GenerateBathroom(Room bathroom)
    {
        GameObject parentRoomInstance = Instantiate(empty, bathroom.Bounds.center, Quaternion.identity, gameObject.transform);
        
        Vector3 toiletLocation = Vector3.zero;

        List<Wall> doorlessWall = new List<Wall>();
        foreach (KeyValuePair<ESides, Wall> wall in bathroom.Walls)
        {
            if (!wall.Value.ContainsDoor()) 
            {
                doorlessWall.Add(wall.Value);
            }
        }

        List<Wall> emptyWalls = new List<Wall>();
        foreach (Wall wall in doorlessWall)
        {
            if (wall.IsWallEmpty()) emptyWalls.Add(wall);
        }

        doorlessWall.Sort((x,y) => x.Length.CompareTo(y.Length));

        List<EDescriptorTags> descriptors = new List<EDescriptorTags>();

        if (bathroom.Size >= SMALL)
        {

            Furniture toilet = GetRandom(FindFurnitureByTag(ERoomTypes.Bathroom,
                                                EObjectTag.Toilet,
                                                EDescriptorTags.Small,
                                                ESearchMode.RequireOne));

            Furniture bath = GetRandom(FindFurnitureByTag(ERoomTypes.Bathroom, EObjectTag.Shower, EDescriptorTags.Small, ESearchMode.RequireOne));

            Furniture basin = GetRandom(FindFurnitureByTag(ERoomTypes.Bathroom, EObjectTag.Sink, EDescriptorTags.Small, ESearchMode.RequireOne));

            Wall startWall = null;
            if (emptyWalls.Count <= 0) startWall = doorlessWall[0];
            else startWall = emptyWalls[0];


            GameObject showerInstance = SpawnAdjustedToWall(bath, bathroom, startWall, parentRoomInstance, 0f, 1f);

            float toiletWallSlide = startWall.Length > 3 ? .3f : .1f;

            GameObject toiletInstance = SpawnAdjustedToWall(toilet, bathroom, startWall, parentRoomInstance, 0f, toiletWallSlide);

            Wall basinWall = null;
            foreach (Wall wall in doorlessWall)
            {
                if (wall != startWall && Vector3.Distance(wall.MiddlePoint, showerInstance.transform.position) > 1.5f) 
                { 
                    basinWall = wall;
                    GameObject basinInstance = SpawnAdjustedToWall(basin, bathroom, basinWall, parentRoomInstance, 0f, .5f);
                    break;
                }
            }

            Furniture showermat = GetRandom(FindFurnitureByTag(ERoomTypes.Bathroom, EObjectTag.Carpet, EDescriptorTags.Small, ESearchMode.RequireOne));
            SpawnOpposite(showermat, showerInstance, parentRoomInstance, .1f, .5f);
        }
        else if (bathroom.Size >= MEDIUM) descriptors.Add(EDescriptorTags.Medium);
        else if (bathroom.Size >= BIG) descriptors.Add(EDescriptorTags.Big);

        if (_attachCameraToRoom) Camera.main.transform.position = bathroom.Bounds.center + new Vector3(0, 5.8f, 0f);

    }

    private GameObject SpawnAdjustedToWall(Furniture furniture, Room room, Wall wall, GameObject parent, float wallMargin, float wallSlide, float chance = 1f)
    {

        float rng = UnityEngine.Random.Range(0f, 1f);

        if (rng >= chance)
        {
            return null;
        }

        GameObject furnitureInstance = Instantiate(furniture.Prefab, wall.MiddlePoint, Quaternion.identity, parent.transform);
        MeshRenderer furnitureMesh = furnitureInstance.GetComponentInChildren<MeshRenderer>();

        float furnitureLength = furnitureMesh.bounds.size.z;
        float furnitureWidth = furnitureMesh.bounds.size.x;
        float wallCoveragePercentage = (furnitureWidth / 2) / wall.Length;

        float adjustedWallSlide = wallSlide > .5f ? wallSlide - wallCoveragePercentage : wallSlide + wallCoveragePercentage;
        adjustedWallSlide = wallSlide == .5f ? wallSlide : adjustedWallSlide;

        furnitureInstance.transform.position = Vector3.Lerp(wall.StartPoint, wall.EndPoint, adjustedWallSlide) - ((furnitureLength / 2) + wallMargin) * wall.GetInsideVector(room);
        furnitureInstance.transform.forward = wall.GetInsideVector(room);

        return furnitureInstance;
    }

    private GameObject SpawnOpposite(Furniture furniture, GameObject other, GameObject parentInstance, float objectMargin, float chance = 1f)
    {
        float rng = UnityEngine.Random.Range(0f, 1f);

        if (rng >= chance)
        {
            return null;
        }

        GameObject furnitureInstance = Instantiate(furniture.Prefab, other.transform.position, Quaternion.identity, parentInstance.transform);

        float meshLength = furnitureInstance.GetComponentInChildren<MeshRenderer>().bounds.size.z;

        furnitureInstance.transform.position = other.transform.position - (objectMargin + meshLength) * other.transform.forward;
        furnitureInstance.transform.forward = -other.transform.forward;

        return furnitureInstance;
    }

    public void Subscribe()
    {
        WorldStateManager.Instance.AddSubscriber();
    }

    public void NotifyTaskCompleted()
    {
        WorldStateManager.Instance.NotifyComplete();
    }
}