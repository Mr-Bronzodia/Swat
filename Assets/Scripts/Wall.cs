using System;
using System.Collections;
using System.Collections.Generic;
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


}
