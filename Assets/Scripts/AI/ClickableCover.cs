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

        Vector3 nextSpot = gameObject.transform.position - ((units.Count * 1.2f) / 2) * gameObject.transform.right;

        float i = 1;
        foreach (Unit unit in units)
        {
            nextSpot = nextSpot + i * gameObject.transform.right;
            TakeCoverCommand takeCover = new TakeCoverCommand(unit, nextSpot);

            commands.Add(takeCover);
            i += .1f;
        }

        return commands;
    }
}
