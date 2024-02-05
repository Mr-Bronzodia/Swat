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

    public List<Command> GetAvailableCommands(List<Unit> u)
    {
        List<Command> results = new List<Command>();

        Plane plane = new Plane(Vector3.up, 0);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 worldPosition = Vector3.zero;

        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }

        int i = 0;
        foreach(Unit unit in u)
        {
            Vector3 dirToTarget = new Vector3(1,0,0);
            Vector3 finalPos = worldPosition + i * dirToTarget;
            Command move = new MoveCommand(unit, finalPos);
            results.Add(move);

            i += 3;
        }

        return results;
    }
}
