using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableCover : MonoBehaviour, IClickable
{

    public List<Command> GetAvailableCommands(Unit unit)
    {

        return new List<Command>() { new TakeCoverCommand(unit, gameObject.transform.position) };
    }

    public List<Command> GetAvailableCommands(List<Unit> units)
    {
        List<Command> commands = new List<Command>();

        Vector3 directionVector;

        RaycastHit hit;

        float leftSpace = 0;
        float rightSpace = 0;

        if (Physics.Raycast(transform.position, transform.right, out hit, Mathf.Infinity))
        {
            rightSpace = Vector3.Distance(transform.position, hit.point);
        }

        if (Physics.Raycast(transform.position, -transform.right, out hit, Mathf.Infinity))
        {
            leftSpace = Vector3.Distance(transform.position, hit.point);
        }

        if (rightSpace > leftSpace) directionVector = transform.right;
        else directionVector = -transform.right;

        Vector3 nextSpot = gameObject.transform.position - (units.Count / 2f) * directionVector;
        float i = 1;
        foreach (Unit unit in units)
        {
            nextSpot = nextSpot + i * directionVector;
            TakeCoverCommand takeCover = new TakeCoverCommand(unit, nextSpot);

            commands.Add(takeCover);
            i += .1f;
        }

        return commands;
    }
}
