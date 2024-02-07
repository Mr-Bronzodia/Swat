using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

public class Room : TreeMapNode, IEquatable<Room>
{
    public Bounds Bounds { get; private set; }
    public List<Room> ConnectedRooms { get; private set; }
    public Dictionary<Sides, Wall> Walls { get; private set; }

    private List<Room> _adjustedRooms;

    private readonly int ANGLETHRESHOLD = 35;


    public Room(RoomTypes roomType, float width, float height, Bounds bounds) : base(roomType, width, height)
    {
        ConnectedRooms = new List<Room>();
        Bounds = bounds;
        Walls = new Dictionary<Sides, Wall>();

        _adjustedRooms = new List<Room>();
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

    public bool containsOutsideFacingWalls()
    {
        foreach (KeyValuePair<Sides, Wall> wall in Walls)
        {
            if (IsWallFacingOutside(wall.Value)) return true;
        }

        return false;
    }

    public void FinalizeLayout()
    {
        Vector3 bottomLeft = Bounds.min;
        Vector3 topLeft = new Vector3(Bounds.min.x, Bounds.center.y, Bounds.max.z);
        Vector3 topRight = Bounds.max;
        Vector3 bottomRight = new Vector3(Bounds.max.x, Bounds.center.y, Bounds.min.z);

        Wall topWall = new Wall(topLeft, topRight, Sides.Up);
        Walls.Add(Sides.Up, topWall);
        Wall leftWall = new Wall(bottomLeft, topLeft, Sides.Left);
        Walls.Add(Sides.Left, leftWall);
        Wall rightWall = new Wall(topRight, bottomRight, Sides.Right);
        Walls.Add(Sides.Right, rightWall);
        Wall bottomWall = new Wall(bottomRight, bottomLeft, Sides.Down);
        Walls.Add(Sides.Down, bottomWall);
    }

    public void BuildFloor(GameObject floorPrefab, GameObject parentInstance)
    {
        GameObject floorInstance = UnityEngine.Object.Instantiate(floorPrefab, Bounds.center, Quaternion.identity, parentInstance.transform);
        floorInstance.name = floorInstance.name + " " + RoomType.ToString();

        MeshRenderer floorRenderer;

        if (floorInstance.TryGetComponent<MeshRenderer>(out floorRenderer))
        {
            Bounds floorBounds = floorRenderer.bounds;

            Bounds roomBounds = Bounds;

            float requiredScaleX = floorBounds.size.x / roomBounds.size.x;
            float requiredScaleZ = floorBounds.size.z / roomBounds.size.z;

            floorInstance.transform.localScale = new Vector3(floorInstance.transform.localScale.x / requiredScaleX,
                                                             floorInstance.transform.localScale.y,
                                                             floorInstance.transform.localScale.z / requiredScaleZ);
        }
        else
        {
            Debug.LogError("Can't Find MeshRenderer on " + floorPrefab.name.ToString() + " while generating floor");
        }
    }

    public void BuildRoof(GameObject roofPrefab, float roofHeight, GameObject parentInstance)
    {
        GameObject floorInstance = UnityEngine.Object.Instantiate(roofPrefab, Bounds.center + new Vector3(0, roofHeight, 0), Quaternion.identity, parentInstance.transform);
        floorInstance.name = floorInstance.name + " " + RoomType.ToString();

        MeshRenderer floorRenderer;

        if (floorInstance.TryGetComponent<MeshRenderer>(out floorRenderer))
        {
            Bounds floorBounds = floorRenderer.bounds;

            Bounds roomBounds = Bounds;

            float requiredScaleX = floorBounds.size.x / roomBounds.size.x;
            float requiredScaleZ = floorBounds.size.z / roomBounds.size.z;

            floorInstance.transform.localScale = new Vector3(floorInstance.transform.localScale.x / requiredScaleX,
                                                             floorInstance.transform.localScale.y,
                                                             floorInstance.transform.localScale.z / requiredScaleZ);
        }
        else
        {
            Debug.LogError("Can't Find MeshRenderer on " + roofPrefab.name.ToString() + " while generating floor");
        }
    }

    private bool IsWallFacingOutside(Wall wall)
    {
        Vector3 outsideDirection = (wall.MiddlePoint - Bounds.center).normalized;

        foreach (Room adj in _adjustedRooms)
        {
            if (adj.Bounds.Contains(wall.MiddlePoint + outsideDirection * .2f)) return false;
        }
        
        return true;
    }

    public void BuildFacade(GameObject wallInsidePrefab, GameObject wallOutsidePrefab, List<GameObject> windowPrefab, GameObject insideDoorPrefab, bool shouldContainOutsideDoor, GameObject parentInstance) 
    {
        int outsideConnections = 0;
        foreach (KeyValuePair<Sides, Wall> wall in Walls)
        {
            if (IsWallFacingOutside(wall.Value))
            {
                bool buildDoor = outsideConnections == 0 && shouldContainOutsideDoor ? true : false;

                wall.Value.BuildOutsideWall(Bounds.center, wallOutsidePrefab, windowPrefab, buildDoor, insideDoorPrefab, parentInstance);

                if (buildDoor) outsideConnections++;
            }
            else wall.Value.BuildIndoorsWall(Bounds.center, wallInsidePrefab, insideDoorPrefab, parentInstance);
        }

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
        Vector3 otherDirection = (other.Bounds.center - Bounds.center).normalized;

        Vector3 upVector = new Vector3(0, 0, 1f);
        Vector3 rightVector = new Vector3(1f, 0, 0);

        Wall thisWallSide;
        Wall otherWallSide;

        if (Vector3.Angle(otherDirection, upVector) < ANGLETHRESHOLD)
        {
            thisWallSide = Walls[Sides.Up];
            otherWallSide = other.Walls[Sides.Down];

            Wall shorterWall = thisWallSide.Length < otherWallSide.Length ? thisWallSide : otherWallSide;

            thisWallSide.AddDoorPosition(shorterWall.MiddlePoint);
            otherWallSide.AddDoorPosition(shorterWall.MiddlePoint);
            return;
        }

        if (Vector3.Angle(otherDirection, rightVector) < ANGLETHRESHOLD)
        {
            thisWallSide = Walls[Sides.Right];
            otherWallSide = other.Walls[Sides.Left];

            Wall shorterWall = thisWallSide.Length < otherWallSide.Length ? thisWallSide : otherWallSide;

            thisWallSide.AddDoorPosition(shorterWall.MiddlePoint);
            otherWallSide.AddDoorPosition(shorterWall.MiddlePoint);
            return;
        }

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
