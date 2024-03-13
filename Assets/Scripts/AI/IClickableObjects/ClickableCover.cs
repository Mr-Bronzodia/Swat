using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;

public class ClickableCover : MonoBehaviour, IClickable
{

    public List<Command> GetAvailableCommands(Unit unit)
    {

        NavMeshHit hit;
        Vector3 nearestPoint;

        Vector3 toCoverDir = (gameObject.transform.position - unit.BlackBoard.Position).normalized;

        if (NavMesh.SamplePosition(gameObject.transform.position - 1f * toCoverDir, out hit, .5f, NavMesh.AllAreas))
        {
            nearestPoint = hit.position;
        }
        else
        {
            Debug.Log("Cant find near point in take cover command terminating eraly");
            nearestPoint = unit.BlackBoard.Position;
        }


        Vector3 nearsEdge;
        if (NavMesh.FindClosestEdge(nearestPoint, out hit, NavMesh.AllAreas))
        {
            nearsEdge = hit.position;
        }
        else
        {
            Debug.Log("Cant find near edge in take cover terminating early");
            nearsEdge = unit.BlackBoard.Position;
        }

        MoveCommand moveToCover = new MoveCommand(unit, nearsEdge);
        TakeCoverCommand takeCover = new TakeCoverCommand(unit, nearsEdge);
        SequencerCommand takeCoverSeq = new SequencerCommand(unit, "Take Cover",new List<Command> { moveToCover, takeCover});

        return new List<Command>() { takeCoverSeq };
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
        List<Command> teamTakeCover = new List<Command>();
        foreach (Unit unit in units)
        {
            nextSpot = nextSpot + i * directionVector;
            MoveCommand moveToCover = new MoveCommand(unit, nextSpot);
            TakeCoverCommand takeCover = new TakeCoverCommand(unit, nextSpot);
            teamTakeCover.Add(moveToCover);
            teamTakeCover.Add(takeCover);
            i += .1f;
        }

        SequencerCommand takeCoverSeq = new SequencerCommand(units[0], "Take Cover", teamTakeCover);

        commands.Add(takeCoverSeq);

        return commands;
    }
}
