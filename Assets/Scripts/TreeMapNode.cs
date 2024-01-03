using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    public void SortChildren()
    {
        this.Children.Sort((x, y) => y.Size.CompareTo(x.Size));
    }

    public void RandomizeChildren()
    {
        SortChildren();

        bool allowedToSwap = true;
        for (int i = 0; i <= Children.Count - 2; i++)
        {
            if (!allowedToSwap)
            {
                allowedToSwap = true; 
                continue;
            }

            float rand = Random.Range(0f, 1f);

            if (rand < .5f) continue;

            TreeMapNode leftTmp = Children[i];
            TreeMapNode rightTmp = Children[i + 1];
            Children[i] = rightTmp;
            Children[i + 1] = leftTmp;
            allowedToSwap = false;
        }
    }

    public override string ToString()
    {
        string ret = "RoomType: (" + RoomType.ToString() + ") Child Count: " + Children.Count;

        return ret;
    }
}
