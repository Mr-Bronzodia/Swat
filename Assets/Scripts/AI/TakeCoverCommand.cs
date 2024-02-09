using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class TakeCoverCommand : Command
{
    Vector3 _initialPosition;
    bool _terminateOnNextUpdate = false;

    public TakeCoverCommand(Unit unit, Vector3 initialPosition) : base(unit)
    {
        _initialPosition = initialPosition;
    }

    public override bool CheckCommandCompleted()
    {
        if (Unit.BlackBoard.CommandQueue.Count >= 1) return true;

        if (_terminateOnNextUpdate) return true;

        return false;
    }

    public override string ToUIString()
    {
        return "Take Cover";
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {

        NavMeshHit hit;
        Vector3 nearestPoint;
        if (NavMesh.SamplePosition(_initialPosition, out hit, 3.2f, NavMesh.AllAreas))
        {
            nearestPoint = hit.position;
        }
        else
        {
            Debug.Log("Cant find near point in take cover command terminating eraly");
            nearestPoint = Unit.BlackBoard.Position;
            _terminateOnNextUpdate = true;
        }

        
        Vector3 nearsEdge;
        if (NavMesh.FindClosestEdge(nearestPoint, out hit, NavMesh.AllAreas))
        {
            nearsEdge = hit.position;
        }
        else
        {
            Debug.Log("Cant find near edge in take cover terminating early");
            nearsEdge = Unit.BlackBoard.Position;
            _terminateOnNextUpdate = true;
        }

        Unit.NavAgent.SetDestination(nearsEdge);
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
