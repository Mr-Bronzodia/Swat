using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMapNode 
{
    public RoomTypes RoomType { get; set; }

    public float Width;

    public float Height;

    public float Size;

    public List<TreeMapNode> Children;


    public TreeMapNode(RoomTypes roomType, float width, float height)
    {
        RoomType = roomType;
        Children = new List<TreeMapNode>();
        Width = width;
        Height = height;
        Size = width * height;
    }

    public override string ToString()
    {
        string ret = "RoomType: (" + RoomType.ToString() + ") Child Count: " + Children.Count;

        return ret;
    }
}
