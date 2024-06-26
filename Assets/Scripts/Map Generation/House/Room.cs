using System;
using System.Collections.Generic;
using UnityEngine;


public class Room : TreeMapNode, IEquatable<Room>
{
    public Bounds Bounds { get; private set; }
    public List<Room> ConnectedRooms { get; private set; }
    public Dictionary<ESides, Wall> Walls { get; private set; }

    private List<Room> _adjustedRooms;

    private readonly int ANGLETHRESHOLD = 35;


    public Room(ERoomTypes roomType, float width, float height, Bounds bounds) : base(roomType, width, height)
    {
        ConnectedRooms = new List<Room>();
        Bounds = bounds;
        Walls = new Dictionary<ESides, Wall>();

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
        foreach (KeyValuePair<ESides, Wall> wall in Walls)
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

        Wall topWall = new Wall(topLeft, topRight, ESides.Up);
        Walls.Add(ESides.Up, topWall);
        Wall leftWall = new Wall(bottomLeft, topLeft, ESides.Left);
        Walls.Add(ESides.Left, leftWall);
        Wall rightWall = new Wall(topRight, bottomRight, ESides.Right);
        Walls.Add(ESides.Right, rightWall);
        Wall bottomWall = new Wall(bottomRight, bottomLeft, ESides.Down);
        Walls.Add(ESides.Down, bottomWall);
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

    public void BuildFacade(GameObject wallInsidePrefab, GameObject wallOutsidePrefab, List<GameObject> windowPrefab, GameObject doorFramePrefab, bool shouldContainOutsideDoor,GameObject doorPrefab, GameObject parentInstance) 
    {
        int outsideConnections = 0;
        foreach (KeyValuePair<ESides, Wall> wall in Walls)
        {
            if (IsWallFacingOutside(wall.Value))
            {
                bool buildDoor = outsideConnections == 0 && shouldContainOutsideDoor ? true : false;

                wall.Value.BuildOutsideWall(Bounds.center, wallOutsidePrefab, windowPrefab, buildDoor, doorFramePrefab, parentInstance);

                if (buildDoor) outsideConnections++;
            }
            else wall.Value.BuildIndoorsWall(Bounds.center, wallInsidePrefab, doorFramePrefab, parentInstance);

            wall.Value.BuildDoors(doorPrefab, this, parentInstance);
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
            thisWallSide = Walls[ESides.Up];
            otherWallSide = other.Walls[ESides.Down];

            Wall shorterWall = thisWallSide.Length < otherWallSide.Length ? thisWallSide : otherWallSide;

            thisWallSide.AddDoorPosition(shorterWall.MiddlePoint);
            otherWallSide.AddDoorPosition(shorterWall.MiddlePoint);

            return;
        }

        if (Vector3.Angle(otherDirection, rightVector) < ANGLETHRESHOLD)
        {
            thisWallSide = Walls[ESides.Right];
            otherWallSide = other.Walls[ESides.Left];

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
