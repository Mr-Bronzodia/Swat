using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Room : TreeMapNode, IEquatable<Room>
{
    public Bounds Bounds { get; private set; }
    public List<Room> ConnectedRooms { get; private set; }

    public List<Vector3> DoorPositions { get; private set; }

    private List<Room> _adjustedRooms;

    public Room(RoomTypes roomType, float width, float height, Bounds bounds) : base(roomType, width, height)
    {
        ConnectedRooms = new List<Room>();
        DoorPositions = new List<Vector3>();
        _adjustedRooms = new List<Room>();
        Bounds = bounds;
    }

    public void AddRoomConnection(Room room)
    {
        ConnectedRooms.Add(room);
        FindDoorPosition(room);
    }

    public void AddAdjustedRoom(Room room)
    {
        _adjustedRooms.Add(room);
    }

    public void BuildFloor(GameObject floorPrefab, GameObject parentInstance)
    {
        GameObject floorInstance = UnityEngine.Object.Instantiate(floorPrefab, Bounds.center, Quaternion.identity, parentInstance.transform);
        floorInstance.name = floorInstance.name + " " + RoomType.ToString();

        MeshRenderer flooRenderer;

        if (floorInstance.TryGetComponent<MeshRenderer>(out flooRenderer))
        {
            Bounds floorBounds = flooRenderer.bounds;

            Bounds roomBounds = Bounds;

            float requiredScaleX = floorBounds.size.x / roomBounds.size.x;
            float requiredScaleZ = floorBounds.size.z / roomBounds.size.z;

            floorInstance.transform.localScale = new Vector3(floorInstance.transform.localScale.x / requiredScaleX,
                                                             floorInstance.transform.localScale.y,
                                                             floorInstance.transform.localScale.z / requiredScaleZ);
        }
        else
        {
            Debug.LogError("Can't Find MeshRenderer on " + floorPrefab.name.ToString() + " while genereting floor");
        }
    }

    private bool IsWallFacingOutside(Vector3 wallStart, Vector3 wallEnd)
    {
        Vector3 wallMiddelPoint = Vector3.Lerp(wallStart, wallEnd, .5f);
        Vector3 outsideDirection = FindWallOutsideDirection(wallMiddelPoint);

        foreach (Room adj in _adjustedRooms)
        {
            if (adj.Bounds.Contains(wallMiddelPoint + outsideDirection * .2f)) return false;
        }
        
        return true;
    }

    public void BuildFacade(GameObject wallInsidePrefab, GameObject wallOutsidePrefab, GameObject windowPrefab, GameObject doorPrefab, GameObject parentInstance) 
    {
        foreach (Vector3 doorPosition in DoorPositions)
        {
            GameObject doorInstance = UnityEngine.Object.Instantiate(doorPrefab, doorPosition, Quaternion.identity, parentInstance.transform);

            doorInstance.transform.forward = FindWallOutsideDirection(doorInstance.transform);

            doorInstance.name = doorInstance.name + " " + RoomType.ToString();
        }

        Vector3 bottomLeft = Bounds.min;
        Vector3 topLeft = new Vector3(Bounds.min.x, Bounds.center.y, Bounds.max.z);
        Vector3 topRight = Bounds.max;
        Vector3 bottomRight = new Vector3(Bounds.max.x, Bounds.center.y, Bounds.min.z);

        //Left Wall
        if (IsWallFacingOutside(bottomLeft, topLeft)) BuildOutsideWall(bottomLeft, topLeft, wallOutsidePrefab, windowPrefab, parentInstance);
        else BuildIndoorsWall(bottomLeft, topLeft, wallInsidePrefab, parentInstance);

        //Top Wall
        if (IsWallFacingOutside(topLeft, topRight)) BuildOutsideWall(topLeft, topRight, wallOutsidePrefab, windowPrefab, parentInstance);
        else BuildIndoorsWall(topLeft, topRight, wallInsidePrefab, parentInstance);

        //Right Wall
        if (IsWallFacingOutside(topRight, bottomRight)) BuildOutsideWall(topRight, bottomRight, wallOutsidePrefab, windowPrefab, parentInstance);
        else BuildIndoorsWall(topRight, bottomRight, wallInsidePrefab, parentInstance);

        //Bottom Wall
        if (IsWallFacingOutside(bottomRight, bottomLeft)) BuildOutsideWall(bottomRight, bottomLeft, wallOutsidePrefab, windowPrefab, parentInstance);
        else BuildIndoorsWall(bottomRight, bottomLeft, wallInsidePrefab, parentInstance);
    }

    private void BuildIndoorsWall(Vector3 startPoint, Vector3 endPoint, GameObject wallPrefab, GameObject parentInstance)
    {
        float wallDistance = Vector3.Distance(startPoint, endPoint);

        MeshRenderer wallRenderer = wallPrefab.GetComponentInChildren<MeshRenderer>();

        float wallWidth = wallRenderer.bounds.size.x;
        int noWalls = (int)(wallDistance / wallWidth);

        float reminder = (wallDistance % wallWidth) / wallWidth;
        float reminderPerInstance = reminder / noWalls;

        Vector3 wallDirection = (endPoint - startPoint).normalized;

        wallWidth += reminderPerInstance;

        for (int i = 0; i < noWalls; i++)
        {
            Vector3 nextWallPos = startPoint + (((wallWidth * i) + wallWidth / 2) * wallDirection);
            GameObject nextWall = UnityEngine.Object.Instantiate(wallPrefab, nextWallPos, Quaternion.identity, parentInstance.transform);
            nextWall.transform.forward = FindWallOutsideDirection(Vector3.Lerp(startPoint, endPoint, .5f));
            Vector3 newWallScale = new Vector3(nextWall.transform.localScale.x + (nextWall.transform.localScale.x * reminderPerInstance),
                                               nextWall.transform.localScale.y,
                                               nextWall.transform.localScale.z);

            if (reminder != 0) nextWall.transform.localScale = newWallScale;
        }
    }

    private void BuildOutsideWall(Vector3 startPoint, Vector3 endPoint, GameObject wallPrefab, GameObject windowPrefab, GameObject parentInstance)
    {
        MeshRenderer windowRenderer = windowPrefab.GetComponentInChildren<MeshRenderer>();
        MeshRenderer wallRenderer = wallPrefab.GetComponentInChildren<MeshRenderer>();

        float windowWidth = windowRenderer.bounds.size.x;
        float wallWidth = wallRenderer.bounds.size.x;

        float wallDistance = Vector3.Distance(startPoint, endPoint);
        Vector3 wallDirection = (endPoint - startPoint).normalized;

        int noWindows = (int)(wallDistance / (windowWidth + (wallWidth * 2)));
        int noWalls = (int)((wallDistance - noWindows * windowWidth) / wallWidth);

        float reminder = (wallDistance % wallWidth) / wallWidth;
        float reminderPerInstance = reminder / noWalls;

        wallWidth += reminderPerInstance;

        int windowSpacing = 0;
        if (noWindows != 0) windowSpacing = (int)(noWalls / noWindows);

        float distancePointer = wallWidth / 2;
        int sinceLastWindow = Mathf.CeilToInt(windowSpacing / 2);
        for (int i = 0; i < noWalls + noWindows; i++)
        {
            Vector3 nextWallPos = startPoint + distancePointer * wallDirection;
            GameObject nextWall = UnityEngine.Object.Instantiate(wallPrefab, nextWallPos, Quaternion.identity, parentInstance.transform);
            Vector3 newWallScale = new Vector3(nextWall.transform.localScale.x + (nextWall.transform.localScale.x * reminderPerInstance),
                                               nextWall.transform.localScale.y,
                                               nextWall.transform.localScale.z);

            if (reminder != 0) nextWall.transform.localScale = newWallScale;
            nextWall.transform.forward = FindWallOutsideDirection(Vector3.Lerp(startPoint, endPoint, .5f));

            distancePointer += wallWidth;
            sinceLastWindow++;

            if (noWindows == 0) continue;
            if (noWalls + noWalls == i) break;

            if (windowSpacing <= sinceLastWindow) 
            {
                sinceLastWindow = 0;

                distancePointer += (wallWidth / 2) - reminderPerInstance;

                Vector3 nextWindowPos = startPoint + (distancePointer + wallWidth - reminderPerInstance) * wallDirection;
                GameObject nextWindow = UnityEngine.Object.Instantiate(windowPrefab, nextWindowPos, Quaternion.identity, parentInstance.transform);
                nextWindow.transform.forward = FindWallOutsideDirection(Vector3.Lerp(startPoint, endPoint, .5f));

                distancePointer += (windowWidth - wallWidth / 2) + reminderPerInstance;
                i++;
            }
        }
    }


    private Vector3 FindWallOutsideDirection(Transform transform)
    {

        Vector3 upWall = new Vector3(Bounds.center.x, 0f, Bounds.max.z);
        Vector3 downWall = new Vector3(Bounds.center.x, 0f, Bounds.min.z);
        Vector3 leftWall = new Vector3(Bounds.min.x, 0f, Bounds.center.z);
        Vector3 rightWall = new Vector3(Bounds.max.x, 0f, Bounds.center.z);

        List<Vector3> walls = new List<Vector3>() { upWall, downWall, leftWall, rightWall };

        Vector3 closestPoint = Vector3.zero;
        float smallestDistance = float.MaxValue;

        foreach (Vector3 wall in walls)
        {
            float distance = Vector3.Distance(wall, transform.position);

            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestPoint = wall;
            }
        }

        return (closestPoint - Bounds.center).normalized;
    }


    private Vector3 FindWallOutsideDirection(Vector3 wallMiddlePoint)
    {
        return (wallMiddlePoint - Bounds.center).normalized;
    }

    /// <summary>
    /// Checks if two rooms share a wall
    /// </summary>
    public bool IsAdjusted(Room other)
    {
        float extrudeAmount = .08f;

        Vector3 upPoint = new Vector3(Bounds.center.x, 0, Bounds.max.z + extrudeAmount);
        Vector3 downPoint = new Vector3(Bounds.center.x, 0, Bounds.min.z - extrudeAmount);
        Vector3 rightPoint = new Vector3(Bounds.max.x + extrudeAmount, 0, Bounds.center.z);
        Vector3 leftPoint = new Vector3(Bounds.min.x - extrudeAmount, 0, Bounds.center.z);

        if (other.Bounds.Contains(upPoint)) return true;
        if (other.Bounds.Contains(downPoint)) return true;
        if (other.Bounds.Contains(leftPoint)) return true;
        if (other.Bounds.Contains(rightPoint)) return true;

        return false;
    }


    private void FindDoorPosition(Room other)
    {
        Room smallerRoom;
        Room biggerRoom;

        if (this.Size < other.Size)
        {
            smallerRoom = this;
            biggerRoom = other;
        }
        else
        {
            smallerRoom = other;
            biggerRoom = this;
        }


        Vector3 upWall = new Vector3(smallerRoom.Bounds.center.x, 0f, smallerRoom.Bounds.max.z);
        Vector3 downWall = new Vector3(smallerRoom.Bounds.center.x, 0f, smallerRoom.Bounds.min.z);
        Vector3 leftWall = new Vector3(smallerRoom.Bounds.min.x, 0f, smallerRoom.Bounds.center.z);
        Vector3 rightWall = new Vector3(smallerRoom.Bounds.max.x, 0f, smallerRoom.Bounds.center.z);

        List<Vector3> walls = new List<Vector3>() { upWall, downWall, leftWall, rightWall};

        Vector3 closestPoint = Vector3.zero;
        float smallestDistance = float.MaxValue;

        foreach (Vector3 wall in walls)
        {
            float distance = Vector3.Distance(wall, biggerRoom.Bounds.center);

            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestPoint = wall;
            }
        }

        if (other.DoorPositions.Contains(closestPoint)) return;

        if (closestPoint != Vector3.zero && !DoorPositions.Contains(closestPoint)) DoorPositions.Add(closestPoint);
    }


    public override string ToString()
    {
        return "[" + RoomType.ToString() + " Conencted Rooms Count: " + ConnectedRooms.Count + " Size: " + Bounds.size + "]";
    }

    public bool Equals(Room other)
    {
        return this.Bounds == other.Bounds;
    }

}
