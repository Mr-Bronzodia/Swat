using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour, IClickable
{
    public List<Command> GetAvailableCommands(Unit u)
    {
        Plane plane = new Plane(Vector3.up, 0);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 worldPosition = Vector3.zero;

        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }

        return new List<Command>() { new MoveCommand(u, worldPosition)};
    }
}
