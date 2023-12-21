using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMapNode 
{
    public RoomTypes RoomType { get; set; }

    public float AspectRatio { get; set; }

    public List<TreeMapNode> Children;

    public TreeMapNode(RoomTypes roomType, float aspectRatio)
    {
        RoomType = roomType;
        AspectRatio = aspectRatio;
        Children = new List<TreeMapNode>();
    }


}
