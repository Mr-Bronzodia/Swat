using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

        //int i = 0;
        //foreach(Unit unit in u)
        //{
        //    Vector3 dirToTarget = new Vector3(1,0,0);
        //    Vector3 finalPos = worldPosition + i * dirToTarget;
        //    Command move = new MoveCommand(unit, finalPos);
        //    results.Add(move);

        //    i += 1.5f;
        //}

        List<Command> sequence = new List<Command>();

        Unit lead = u[0];
        Vector3 leadForward = lead.gameObject.transform.forward;

        for (int i = 1; i < u.Count; i++)
        {
            Unit unit = u[i];
            Vector3 target = lead.BlackBoard.Position - i * leadForward;

            MoveCommand moveBehind = new MoveCommand(unit, target);
            sequence.Add(moveBehind);

            WaitUntillCommand leadWait = new WaitUntillCommand(lead, unit, typeof(FollowCommand));
            sequence.Add(leadWait);

            FollowCommand unitFollowLead = new FollowCommand(unit, lead, i);
            sequence.Add(unitFollowLead);
        }

        MoveCommand leadMove = new MoveCommand(lead, worldPosition);
        sequence.Add(leadMove);

        SequencerCommand sequenceCommand = new SequencerCommand(lead, "Team Move", sequence);

        results.Add(sequenceCommand);

        return results;
    }
}
