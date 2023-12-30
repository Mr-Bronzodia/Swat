using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Transactions;
using System.Xml.Serialization;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class SquerifiedTreeMap
{
    //    public Dictionary<TreeMapNode ,Bounds> GenerateTreeMap(TreeMapNode root, Bounds bounds)
    //    {
    //        Dictionary<TreeMapNode,Bounds> rooms = new Dictionary<TreeMapNode, Bounds>();
    //        RecursiveSquarify(root, bounds, rooms);
    //        return rooms;
    //    }

    //    private void RecursiveSquarify(TreeMapNode node, Bounds bounds, Dictionary<TreeMapNode, Bounds> rooms)
    //    {
    //        if (node == null || node.AspectRatio == 0) return;

    //        rooms.Add(node, bounds);

    //        if (node.Children.Count == 0) return;

    //        node.Children.Sort((x,y) => y.AspectRatio.CompareTo(x.AspectRatio));

    //        List<float> normalizedAspect = new List<float>();
    //        float sum = 0;

    //        foreach (TreeMapNode child in node.Children)
    //        {
    //            normalizedAspect.Add(child.AspectRatio);
    //            sum += child.AspectRatio;
    //        }

    //        Dictionary<TreeMapNode, Bounds> currentRow = new Dictionary<TreeMapNode, Bounds>();
    //        for (int i = 0; i < normalizedAspect.Count; i++)
    //        {
    //            float ratio = normalizedAspect[i] / sum;
    //            Bounds childBounds = CalculateChildBounds(bounds, ratio, GetShorterSide(bounds));
    //            currentRow.Add(node.Children[i], childBounds);
    //        }
    //    }

    //    private bool GetShorterSide(Bounds bounds)
    //    {
    //        if (bounds.size.x > bounds.size.z) return true;
    //        else return false;
    //    }

    //    private Bounds CalculateChildBounds(Bounds parentBounds, float ratio, bool horizontal)
    //    {
    //        Bounds childBounds;

    //        if (horizontal)
    //        {
    //            float width = parentBounds.size.x * ratio;

    //            Vector3 childCentre = new Vector3((parentBounds.center.x - parentBounds.extents.x) + (width / 2), parentBounds.center.y, parentBounds.center.z);
    //            Vector3 childSize = new Vector3(width, parentBounds.size.y, parentBounds.size.z);
    //            childBounds = new Bounds(childCentre, childSize);

    //            //Vector3 parentCentre = new Vector3(parentBounds.center.x + width, parentBounds.center.y, parentBounds.center.z);
    //            //Vector3 parentSize = new Vector3(parentBounds.size.x - width, parentBounds.size.y, parentBounds.size.z);
    //            //parentBounds = new Bounds(parentCentre, parentSize);
    //        }
    //        else
    //        {
    //            float height = parentBounds.size.z * ratio;

    //            Vector3 childCentre = new Vector3(parentBounds.center.x, parentBounds.center.y, (parentBounds.center.z - parentBounds.extents.z) + (height / 2));
    //            Vector3 childSize = new Vector3(parentBounds.size.x, parentBounds.size.y, height);
    //            childBounds = new Bounds(childCentre, childSize);

    //            //Vector3 parentCentre = new Vector3(parentBounds.center.x, parentBounds.center.y, parentBounds.center.z + height);
    //            //Vector3 parentSize = new Vector3(parentBounds.size.x, parentBounds.size.y, parentBounds.size.z - height);
    //            //parentBounds = new Bounds(parentCentre, parentSize);
    //        }


    //        return childBounds;
    //    }
    private TreeMapNode _root;
    private Bounds _rootBounds;

    public SquerifiedTreeMap(TreeMapNode root, Bounds rootBounds) 
    {
        _root = root;
        _rootBounds = rootBounds;
    }

    public Dictionary<TreeMapNode, Bounds> GenerateTreemap()
    {
        Dictionary<TreeMapNode, Bounds> rooms = new Dictionary<TreeMapNode, Bounds>();
        Dictionary<TreeMapNode, Bounds> currentRow = new Dictionary<TreeMapNode, Bounds>();
        RecursiveSquarify(_root, _rootBounds, _root.Children, currentRow, rooms);
        return rooms;
    }


    private void RecursiveSquarify(TreeMapNode parentNode, Bounds parentBounds, List<TreeMapNode>childList, Dictionary<TreeMapNode, Bounds>currentRow, Dictionary<TreeMapNode, Bounds> rooms)
    {
        if (childList.Count > 0)
        {
            parentNode.Children.Sort((x, y) => y.size.CompareTo(x.size));

            if (ShouldContinueRow(currentRow, childList[0].size, childList[childList.Count - 1].size, GetLenghtOfShorterSide(parentBounds)))
            {
                currentRow.Add(childList[0], new Bounds(Vector3.zero, new Vector3(childList[0].width, 0, childList[0].height)));
                childList.RemoveAt(0);
                RecursiveSquarify(parentNode, parentBounds, childList, currentRow, rooms);
            }
            else
            {
                PlaceRow(parentNode, ref parentBounds, currentRow, rooms);
                currentRow.Clear();
                RecursiveSquarify(parentNode, parentBounds, childList, currentRow, rooms);
            }
        }
        else
        {
            PlaceRow(parentNode, ref parentBounds, currentRow, rooms);
        }
    }

    private void PlaceRow(TreeMapNode parent, ref Bounds parentBounds, Dictionary<TreeMapNode, Bounds> currentRow, Dictionary<TreeMapNode, Bounds> rooms)
    {
        bool horizontalSplit = parent.width >= parent.height;

        float cumSum = 0;
        foreach(KeyValuePair<TreeMapNode, Bounds> room in currentRow)
        {
            if (horizontalSplit)
            {
                Vector3 roomSize = new Vector3(room.Value.size.x, room.Value.size.y, room.Value.size.z);
                Vector3 roomCentre = new Vector3((parentBounds.center.x - parentBounds.extents.x) + (roomSize.x / 2),
                                 parentBounds.center.y,
                                 parentBounds.center.z);

                cumSum += roomSize.z;

                rooms.Add(room.Key, new Bounds(roomCentre, roomSize));

                Vector3 newParentSize = new Vector3(parentBounds.size.x - roomSize.x, parentBounds.size.y, parentBounds.size.z);
                Vector3 newParentCentre = new Vector3(parentBounds.center.x + (roomSize.x / 2), parentBounds.center.y, parentBounds.center.z);
                parentBounds = new Bounds(newParentCentre, newParentSize);
            }
            else
            {
                Vector3 roomSize = new Vector3(room.Value.size.x, room.Value.size.y, room.Value.size.z);
                Vector3 roomCentre = new Vector3((parentBounds.center.x - parentBounds.extents.x + cumSum) + (roomSize.x / 2),
                                                 parentBounds.center.y,
                                                 (parentBounds.center.z - parentBounds.extents.z) + (roomSize.z / 2));
                cumSum += roomSize.x;

                rooms.Add(room.Key, new Bounds(roomCentre, roomSize));

                Vector3 newParentSize = new Vector3(parentBounds.size.x, parentBounds.size.y, parentBounds.size.z - roomSize.z);
                Vector3 newParentCentre = new Vector3(parentBounds.center.x, parentBounds.center.y, parentBounds.center.z - (roomSize.z / 2));
                parentBounds = new Bounds(newParentCentre, newParentSize);
            }
        }

    }

    private bool ShouldContinueRow(Dictionary<TreeMapNode, Bounds> currentRow, float largestChildSize, float smallestChildSize, float parentShortestSideLength)
    {
        if (currentRow.Count == 0) return true;

        float totalArea = 0;
        foreach(KeyValuePair<TreeMapNode, Bounds> room in currentRow)
        {
            totalArea += room.Key.size;
        }

        float currentStepWorstAspectRatio = CalculateHighestAspectRatio(currentRow.ElementAt(0).Key.size, currentRow.ElementAt(currentRow.Count - 1).Key.size, totalArea, parentShortestSideLength);

        totalArea += largestChildSize;

        float nextStepWorstAspectRatio = CalculateHighestAspectRatio(largestChildSize, smallestChildSize, totalArea, parentShortestSideLength);

        return currentStepWorstAspectRatio >= nextStepWorstAspectRatio;
    }

    private float CalculateHighestAspectRatio(float size1, float size2, float totalArea, float parentShortestSideLength)
    {
        float aspectRatio1 = Math.Max(size1 / (totalArea - size1), (totalArea - size1) / size1);
        float aspectRatio2 = Math.Max(size2 / (totalArea + size2), (totalArea + size2) / size2);

        return Math.Max(aspectRatio1, aspectRatio2) * (parentShortestSideLength * parentShortestSideLength);
    }

    private float GetLenghtOfShorterSide(Bounds bounds)
    {
        return MathF.Min(bounds.size.x, bounds.size.z);
    }
}
