using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMapNode 
{
    public RoomTypes RoomType { get; set; }

    public float width;

    public float height;

    public float size;

    public List<TreeMapNode> Children;


    public TreeMapNode(RoomTypes roomType, float width, float height)
    {
        RoomType = roomType;
        Children = new List<TreeMapNode>();
        this.width = width;
        this.height = height;
        size = width * height;
    }

    public float GetShorterSide()
    {
        return Mathf.Min(width, height);
    }

    public override string ToString()
    {
        string ret = "RoomType: (" + RoomType.ToString() + ") Child Count: " + Children.Count;

        return ret;
    }
}
