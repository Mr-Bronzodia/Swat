using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Transactions;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

public class SquerifiedTreeMap
{
    private TreeMapNode _root;
    private Bounds _rootBounds;
    private bool _randomizeChildren;
    private float _randomizeChance;

    public SquerifiedTreeMap(TreeMapNode root, Bounds rootBounds) 
    {
        _root = root;
        _rootBounds = rootBounds;
    }

    /// <summary>
    /// Generates Squerified TreeMap based on root node.
    /// </summary>
    /// <returns>Dictionary of room and their bounding boxes</returns>
    public List<Room> GenerateTreemap(bool randomizeChildren, float chance = .5f)
    {
        List<Room> rooms = new List<Room>();
        Dictionary<TreeMapNode, Bounds> currentRow = new Dictionary<TreeMapNode, Bounds>();

        _randomizeChance = chance;
        _randomizeChildren = randomizeChildren;

        _root.SortChildren();
        if (_randomizeChildren) _root.RandomizeChildren(_randomizeChance);

        RecursiveSquarify(_root, _rootBounds, _root.Children, currentRow, rooms);

        return rooms;
    }

    /// <summary>
    /// Splits parent rectangle vertically and horizontally based on children size
    /// </summary>
    private void RecursiveSquarify(TreeMapNode parentNode, Bounds parentBounds, List<TreeMapNode>childList, Dictionary<TreeMapNode, Bounds>currentRow, List<Room> rooms)
    {
        if (childList.Count > 0)
        {

            if (ShouldContinueRow(currentRow, childList[0].Size, childList[childList.Count - 1].Size, GetLenghtOfShorterSide(parentBounds)))
            {
                currentRow.Add(childList[0], new Bounds(Vector3.zero, Vector3.zero));
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

    /// <summary>
    /// Place row in place. Shrinks parent to be outside of current row bounds.
    /// </summary>
    private void PlaceRow(TreeMapNode parent, ref Bounds parentBounds, Dictionary<TreeMapNode, Bounds> currentRow, List<Room> rooms)
    {
        bool horizontalSplit = parentBounds.size.x >= parentBounds.size.z;

        float totalSize = 0;
        foreach (KeyValuePair<TreeMapNode, Bounds> room in currentRow)
        {
            totalSize += room.Key.Size;
        }

        float proportion = totalSize / GetLenghtOfShorterSide(parentBounds);

        float offset = 0;

        foreach (KeyValuePair<TreeMapNode, Bounds> room in currentRow)
        {
            float rectangleWidth = room.Key.Size / proportion;
            Vector3 roomCentre;
            Vector3 roomSize;
            Bounds newRoomBounds;

            if (horizontalSplit)
            {
                roomSize = new Vector3(proportion, parentBounds.size.y, rectangleWidth);
                roomCentre = new Vector3(parentBounds.min.x + roomSize.x / 2, parentBounds.center.y, parentBounds.min.z + (roomSize.z / 2) + offset);
            }
            else
            {
                roomSize = new Vector3(rectangleWidth, parentBounds.size.y, proportion);
                roomCentre = new Vector3(parentBounds.min.x + (roomSize.x / 2) + offset, parentBounds.center.y, parentBounds.min.z + roomSize.z / 2f);
            }

            newRoomBounds = new Bounds(roomCentre, roomSize);
            Room newRoom = new Room(room.Key.RoomType, room.Key.Width, room.Key.Height, newRoomBounds);
            rooms.Add(newRoom);

            room.Key.SortChildren();
            if (_randomizeChildren) room.Key.RandomizeChildren(_randomizeChance);

            RecursiveSquarify(room.Key, newRoomBounds, room.Key.Children, new Dictionary<TreeMapNode, Bounds>(), rooms);

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

    /// <summary>
    /// Decides if current row should continue or not.
    /// </summary>
    private bool ShouldContinueRow(Dictionary<TreeMapNode, Bounds> currentRow, float largestChildSize, float smallestChildSize, float parentShortestSideLength)
    {
        if (currentRow.Count == 0) return true;

        float totalArea = 0;
        foreach(KeyValuePair<TreeMapNode, Bounds> room in currentRow) totalArea += room.Key.Size;

        float currentStepWorstAspectRatio = CalculateHighestAspectRatio(currentRow.ElementAt(0).Key.Size, currentRow.ElementAt(currentRow.Count - 1).Key.Size, totalArea, parentShortestSideLength);

        totalArea += largestChildSize;

        float nextStepWorstAspectRatio = CalculateHighestAspectRatio(largestChildSize, smallestChildSize, totalArea, parentShortestSideLength);


        return currentStepWorstAspectRatio >= nextStepWorstAspectRatio;
    }

    /// <summary>
    /// Return aspec ratio of current row given biggest and smallest bounds. 
    /// </summary>
    private float CalculateHighestAspectRatio(float rPlus, float rMinus, float totalArea, float parentShorterSideLength)
    {
        float biggerRatio = (MathF.Pow(parentShorterSideLength,2) * rPlus) / MathF.Pow(totalArea,2);
        float smallerRation = MathF.Pow(totalArea, 2) / (MathF.Pow(parentShorterSideLength, 2) * rMinus);

        return Math.Max(biggerRatio, smallerRation);
    }

    /// <summary>
    /// Returns shorter side of given bounds.
    /// </summary>
    private float GetLenghtOfShorterSide(Bounds bounds)
    {
        return MathF.Min(bounds.size.x, bounds.size.z);
    }
}
