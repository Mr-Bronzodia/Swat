using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Wall 
{
    public Vector3 StartPoint {  get; private set; }
    public Vector3 EndPoint { get; private set; }
    public Sides Side { get; private set; }
    public Vector3 MiddlePoint { get { return Vector3.Lerp(StartPoint, EndPoint, .5f); } private set { MiddlePoint = value; } }
    
    public float Length { get {return Vector3.Distance(StartPoint, EndPoint); } }

    public List<Vector3> _doorPositions;
    private List<Vector3> _windowPositions;

    public Wall(Vector3 startPoint, Vector3 endPoint, Sides side)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Side = side;

        _doorPositions = new List<Vector3>();
        _windowPositions = new List<Vector3>();
    }

    public static Sides GetOppositeSide(Sides sides)
    {
        switch (sides)
        {
            case Sides.Up:
                return Sides.Down;
            case Sides.Down:
                return Sides.Up;
            case Sides.Left:
                return Sides.Right;
            case Sides.Right:
                return Sides.Left;
        }

        throw new SystemException("Can't find opposite side of " + sides.ToString() + " in Walls.cs");
    }

    public void AddDoorPosition(Vector3 pos)
    {
        _doorPositions.Add(pos);
    }

    private bool IsWallEmpty()
    {
        bool isEmpty = _doorPositions.Count == 0 && _windowPositions.Count == 0;

        return isEmpty;
    }

    public void BuildIndoorsWall(Vector3 roomCentre, GameObject wallPrefab, GameObject doorPrefab, GameObject parentInstance)
    {
        if (_doorPositions.Count == 0)
        {
            BuildWallSegment(StartPoint, EndPoint, roomCentre, wallPrefab, parentInstance);
            return;
        }

        List<Vector3> sortedDoors = _doorPositions.OrderBy(x => Vector3.Distance(StartPoint, x)).Distinct().ToList();
        Vector3 wallDirection = (EndPoint - StartPoint).normalized;

        float doorWidth = doorPrefab.GetComponentInChildren<MeshRenderer>().bounds.size.x;

        BuildWallSegment(StartPoint, sortedDoors[0] + (doorWidth / 2) * -wallDirection, roomCentre, wallPrefab, parentInstance);

        GameObject door = UnityEngine.Object.Instantiate(doorPrefab, sortedDoors[0], Quaternion.identity, parentInstance.transform);
        door.transform.forward = (MiddlePoint - roomCentre).normalized;

        for (int i = 0; i < sortedDoors.Count - 1; i++)
        {
            BuildWallSegment(sortedDoors[i] + (doorWidth / 2) * wallDirection,
                             sortedDoors[i + 1] + (doorWidth / 2) * -wallDirection,
                             roomCentre,
                             wallPrefab,
                             parentInstance);

            GameObject nextDoor = UnityEngine.Object.Instantiate(doorPrefab, sortedDoors[i + 1], Quaternion.identity, parentInstance.transform);
            nextDoor.transform.forward = (MiddlePoint - roomCentre).normalized;

            Debug.DrawLine(sortedDoors[i] + (doorWidth / 2) * wallDirection, sortedDoors[i + 1] + (doorWidth / 2) * -wallDirection, Color.black);
            Debug.Break();  
        }

        BuildWallSegment(sortedDoors[sortedDoors.Count - 1] + (doorWidth / 2) * wallDirection, EndPoint, roomCentre, wallPrefab, parentInstance);
    }

    private void BuildWallSegment(Vector3 startPoint, Vector3 endPoint, Vector3 roomCentre, GameObject wallPrefab, GameObject parentInstance)
    {
        float wallDistance = Vector3.Distance(startPoint, endPoint);

        MeshRenderer wallRenderer = wallPrefab.GetComponentInChildren<MeshRenderer>();

        float wallWidth = wallRenderer.bounds.size.x;
        int noWalls = (int)(wallDistance / wallWidth);

        float reminder = (wallDistance % wallWidth) / wallWidth;
        float reminderPerInstance = reminder / noWalls;

        Vector3 wallDirection = (endPoint - startPoint).normalized;

        if (noWalls == 0)
        {
            float scale = wallDistance / wallWidth;
            wallWidth *= scale;
            Vector3 nextWallPos = startPoint + (wallWidth / 2) * wallDirection;
            GameObject nextWall = UnityEngine.Object.Instantiate(wallPrefab, nextWallPos, Quaternion.identity, parentInstance.transform);
            nextWall.transform.forward = (MiddlePoint - roomCentre).normalized;
            Vector3 newWallScale = new Vector3(nextWall.transform.localScale.x  * scale,
                                               nextWall.transform.localScale.y,
                                               nextWall.transform.localScale.z);

            nextWall.transform.localScale = newWallScale;
            return;
        }

        wallWidth += reminderPerInstance;

        for (int i = 0; i < noWalls; i++)
        {
            Vector3 nextWallPos = startPoint + (((wallWidth * i) + wallWidth / 2) * wallDirection);
            GameObject nextWall = UnityEngine.Object.Instantiate(wallPrefab, nextWallPos, Quaternion.identity, parentInstance.transform);
            nextWall.transform.forward = (MiddlePoint - roomCentre).normalized;
            Vector3 newWallScale = new Vector3(nextWall.transform.localScale.x + (nextWall.transform.localScale.x * reminderPerInstance),
                                               nextWall.transform.localScale.y,
                                               nextWall.transform.localScale.z);

            if (reminder != 0) nextWall.transform.localScale = newWallScale;
        }
    }

    private void BuildOutsideWall(Vector3 startPoint, Vector3 endPoint, Vector3 roomCentre, GameObject wallPrefab, List<GameObject> windowPrefabs, GameObject parentInstance)
    {
        float wallDistance = Vector3.Distance(startPoint, endPoint);

        GameObject windowPrefab = null;
        float lastBiggest = 0;
        foreach (GameObject window in windowPrefabs)
        {
            MeshRenderer renderer = window.GetComponentInChildren<MeshRenderer>();
            float windowSize = renderer.bounds.size.x;

            if (lastBiggest > windowSize && windowPrefab != null) continue;
            lastBiggest = windowSize;

            if (windowSize < wallDistance / 1.5f) windowPrefab = window;
        }

        if (windowPrefab == null) windowPrefab = windowPrefabs[0];

        MeshRenderer windowRenderer = windowPrefab.GetComponentInChildren<MeshRenderer>();
        MeshRenderer wallRenderer = wallPrefab.GetComponentInChildren<MeshRenderer>();

        float windowWidth = windowRenderer.bounds.size.x;
        float wallWidth = wallRenderer.bounds.size.x;

        Vector3 wallDirection = (endPoint - startPoint).normalized;

        int noWindows = (int)(wallDistance / (windowWidth + (wallWidth * 2)));
        int noWalls = (int)((wallDistance - noWindows * windowWidth) / wallWidth);

        float reminder = (wallDistance % wallWidth) / wallWidth;
        float reminderPerInstance = reminder / noWalls;

        wallWidth += reminderPerInstance;

        int windowSpacing = 0;
        if (noWindows != 0) windowSpacing = (int)(noWalls / noWindows);

        float distancePointer = wallWidth / 2;
        int sinceLastWindow = Mathf.CeilToInt(windowSpacing / 2);
        for (int i = 0; i < noWalls + noWindows; i++)
        {
            Vector3 nextWallPos = startPoint + distancePointer * wallDirection;
            GameObject nextWall = UnityEngine.Object.Instantiate(wallPrefab, nextWallPos, Quaternion.identity, parentInstance.transform);
            Vector3 newWallScale = new Vector3(nextWall.transform.localScale.x + (nextWall.transform.localScale.x * reminderPerInstance),
                                               nextWall.transform.localScale.y,
                                               nextWall.transform.localScale.z);

            if (reminder != 0) nextWall.transform.localScale = newWallScale;
            nextWall.transform.forward = (MiddlePoint - roomCentre).normalized;

            distancePointer += wallWidth;
            sinceLastWindow++;

            if (noWindows == 0) continue;
            if (noWalls + noWalls == i) break;

            if (windowSpacing <= sinceLastWindow) 
            {
                sinceLastWindow = 0;

                distancePointer += (wallWidth / 2) - reminderPerInstance;

                Vector3 nextWindowPos = startPoint + (distancePointer + wallWidth - reminderPerInstance) * wallDirection;
                GameObject nextWindow = UnityEngine.Object.Instantiate(windowPrefab, nextWindowPos, Quaternion.identity, parentInstance.transform);
                nextWindow.transform.forward = (MiddlePoint - roomCentre).normalized;

                distancePointer += (windowWidth - wallWidth / 2) + reminderPerInstance;
                i++;
            }
        }
    }

}
