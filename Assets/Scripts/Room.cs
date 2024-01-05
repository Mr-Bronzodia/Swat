using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : TreeMapNode
{
    public Bounds Bounds { get; private set; }
    public List<Room> ConnectedRooms { get; private set; }

    public Room(RoomTypes roomType, float width, float height, Bounds bounds) : base(roomType, width, height)
    {
        ConnectedRooms = new List<Room>();
        Bounds = bounds;
    }

    public void AddRoomConnection(Room room)
    {
        ConnectedRooms.Add(room);
    }

    public bool IsAdjusted(Room other)
    {
        if (Bounds.Intersects(other.Bounds)) return true;

        float extrudeAmount = .05f;

        Vector3 UpPoint = Bounds.max + new Vector3(0, 0, extrudeAmount);
        Vector3 DownPoint = Bounds.min - new Vector3(0, 0, extrudeAmount);
        Vector3 RightPoint = Bounds.max + new Vector3(extrudeAmount, 0, 0);
        Vector3 LedtPoint = Bounds.min - new Vector3(extrudeAmount, 0, 0);

        if (other.Bounds.Contains(UpPoint)) return true;
        if (other.Bounds.Contains(DownPoint)) return true;
        if (other.Bounds.Contains(LedtPoint)) return true;
        if (other.Bounds.Contains(RightPoint)) return true;

        return false;
    }

    public override string ToString()
    {
        return "[" + RoomType.ToString() + " Conencted Rooms Count: " + ConnectedRooms.Count + " Size: " + Bounds.size + "]";
    }

}
