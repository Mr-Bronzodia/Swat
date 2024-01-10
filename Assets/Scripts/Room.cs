using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Room : TreeMapNode, IEquatable<Room>
{
    public Bounds Bounds { get; private set; }
    public List<Room> ConnectedRooms { get; private set; }

    public List<Vector3> DoorPositions { get; private set; }

    public Room(RoomTypes roomType, float width, float height, Bounds bounds) : base(roomType, width, height)
    {
        ConnectedRooms = new List<Room>();
        DoorPositions = new List<Vector3>();
        Bounds = bounds;
    }

    public void AddRoomConnection(Room room)
    {
        ConnectedRooms.Add(room);
        FindDoorPosition(room);
    }

    /// <summary>
    /// Checks if two rooms share a wall
    /// </summary>
    public bool IsAdjusted(Room other)
    {
        if (Bounds.Intersects(other.Bounds)) return true;

        float extrudeAmount = .08f;

        //Vector3 UpPoint = Bounds.max + new Vector3(0, 0, extrudeAmount);
        Vector3 UpPoint = new Vector3(Bounds.center.x, 0, Bounds.max.z + extrudeAmount);
        //Vector3 DownPoint = Bounds.min - new Vector3(0, 0, extrudeAmount);
        Vector3 DownPoint = new Vector3(Bounds.center.x, 0, Bounds.min.z - extrudeAmount);
        //Vector3 RightPoint = Bounds.max + new Vector3(extrudeAmount, 0, 0);
        Vector3 RightPoint = new Vector3(Bounds.max.x + extrudeAmount, 0, Bounds.center.z);
        //Vector3 LedtPoint = Bounds.min - new Vector3(extrudeAmount, 0, 0);
        Vector3 LedtPoint = new Vector3(Bounds.min.x - extrudeAmount, 0, Bounds.center.z);

        //Debug.DrawLine(Bounds.center, UpPoint);
        //Debug.DrawLine(Bounds.center, DownPoint);
        //Debug.DrawLine(Bounds.center, RightPoint);
        //Debug.DrawLine(Bounds.center, LedtPoint);
        //Debug.Break();

        if (other.Bounds.Contains(UpPoint)) return true;
        if (other.Bounds.Contains(DownPoint)) return true;
        if (other.Bounds.Contains(LedtPoint)) return true;
        if (other.Bounds.Contains(RightPoint)) return true;

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

        if (closestPoint != Vector3.zero) DoorPositions.Add(closestPoint);
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
