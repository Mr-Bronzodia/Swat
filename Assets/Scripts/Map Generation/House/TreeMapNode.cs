using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TreeMapNode 
{
    public ERoomTypes RoomType { get; set; }

    public float Width;

    public float Height;

    public float Size;

    public List<TreeMapNode> Children;


    public TreeMapNode(ERoomTypes roomType, float width, float height)
    {
        RoomType = roomType;
        Children = new List<TreeMapNode>();
        Width = width;
        Height = height;
        Size = width * height;
    }

    /// <summary>
    /// Sorts children by size in descending order
    /// </summary>
    public void SortChildren()
    {
        this.Children.Sort((x, y) => y.Size.CompareTo(x.Size));
    }

    /// <summary>
    /// Swaps children with right neighbour based on chance. Children are only allowed to swap places once to prevent extreme aspect ratio. 
    /// </summary>
    /// <param name="chance">chance between 0-1.</param>
    public void RandomizeChildren(float chance)
    {
        SortChildren();

        bool allowedToSwap = true;
        for (int i = 0; i < Children.Count - 1; i++)
        {
            if (!allowedToSwap)
            {
                allowedToSwap = true; 
                continue;
            }

            float rand = Random.Range(0f, 1f);

            if (rand < chance) continue;

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
