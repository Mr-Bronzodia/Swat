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
        //if (!rooms.ContainsKey(parentNode)) rooms.Add(parentNode, parentBounds);

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
                Debug.Log(parentBounds);
                PlaceRow(parentNode, ref parentBounds, currentRow, rooms);
                Debug.Log(parentBounds);
                currentRow.Clear();
                RecursiveSquarify(parentNode, parentBounds, childList, currentRow, rooms);
            }
        }
        else
        {
            Debug.Log(parentBounds);
            PlaceRow(parentNode, ref parentBounds, currentRow, rooms);
            Debug.Log(parentBounds);
        }
    }

    private void PlaceRow(TreeMapNode parent, ref Bounds parentBounds, Dictionary<TreeMapNode, Bounds> currentRow, Dictionary<TreeMapNode, Bounds> rooms)
    {
        bool horizontalSplit = parent.width >= parent.height;

        float totalSize = 0;
        foreach (KeyValuePair<TreeMapNode, Bounds> room in currentRow)
        {
            totalSize += room.Key.size;
        }

        float proportion = totalSize / GetLenghtOfShorterSide(parentBounds);

        float offset = 0;

        foreach (KeyValuePair<TreeMapNode, Bounds> room in currentRow)
        {
            float rectangleWidth = room.Key.size / proportion;
            Vector3 roomCentre;
            Vector3 roomSize;

            if (horizontalSplit)
            {
                roomSize = new Vector3(rectangleWidth, parentBounds.size.y, parentBounds.size.z);
                roomCentre = new Vector3(parentBounds.min.x + offset + rectangleWidth / 2f, parentBounds.center.y, parentBounds.center.z);
            }
            else
            {
                roomSize = new Vector3(parentBounds.size.x, parentBounds.size.y, rectangleWidth);
                roomCentre = new Vector3(parentBounds.center.x, parentBounds.center.y, parentBounds.min.z + offset + rectangleWidth / 2f);
            }

            rooms.Add(room.Key, new Bounds(roomCentre, roomSize));
            offset += rectangleWidth;
        }

        if (horizontalSplit)
        {
            parentBounds.min = new Vector3(parentBounds.min.x + proportion, parentBounds.min.y, parentBounds.min.z);
        }
        else
        {
            parentBounds.min = new Vector3(parentBounds.min.x, parentBounds.min.y, parentBounds.min.z + proportion);
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

        Debug.Log("currentStep: " + currentStepWorstAspectRatio);
        Debug.Log("nextStep: " + nextStepWorstAspectRatio);
        Debug.Log("Shoud: " + (currentStepWorstAspectRatio >= nextStepWorstAspectRatio));

        return currentStepWorstAspectRatio >= nextStepWorstAspectRatio;
    }

    private float CalculateHighestAspectRatio(float biggestArea, float smallestArea, float totalArea, float parentShortestSideLength)
    {
        float biggerRatio = (MathF.Pow(parentShortestSideLength,2) * biggestArea) / MathF.Pow(totalArea,2);
        float smallerRation = MathF.Pow(totalArea, 2) / (MathF.Pow(parentShortestSideLength, 2) * smallestArea);

        return Math.Max(biggerRatio, smallerRation);


    }

    private float GetLenghtOfShorterSide(Bounds bounds)
    {
        return MathF.Min(bounds.size.x, bounds.size.z);
    }
}
