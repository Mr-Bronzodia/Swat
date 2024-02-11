using Codice.CM.WorkspaceServer.Tree.GameUI.Checkin.Updater;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;

public class MoveCommand : Command
{
    Vector3 _target;

    public bool _terminateOnNextUpdate = false;

    public MoveCommand(Unit unit, Vector3 target) : base(unit)
    {
        _target = target;
        //Unit.OnStopImmediately += ImmediateStop;
    }

    ~MoveCommand() 
    {
        //Unit.OnStopImmediately -= ImmediateStop;
    }

    private void ImmediateStop()
    {
        _terminateOnNextUpdate = true;
        Unit.NavAgent.velocity = Vector3.zero;
        Unit.NavAgent.isStopped = true;
    } 


    public override bool CheckCommandCompleted()
    {
        if (_terminateOnNextUpdate) return true;


        return Vector3.Distance(Unit.BlackBoard.Position, _target) <= .2f;
    }

    public override string ToUIString()
    {
        return "Move to " + _target.ToString();
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {
        if (Unit.NavAgent.isStopped) Unit.NavAgent.isStopped = false;

        //NavMeshPath path = new NavMeshPath();   

        //if (N)


        NavMeshHit navMeshHit;
        Vector3 nearestPoint;
        if (NavMesh.SamplePosition(_target, out navMeshHit, 3.2f, 1))
        {
            nearestPoint = navMeshHit.position;
            Unit.NavAgent.SetDestination(nearestPoint);
        }
        else
        {
            Debug.Log("Cant find near navmesh point in move command terminating eraly");
            nearestPoint = Unit.BlackBoard.Position;
            _terminateOnNextUpdate = true;
        }


    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
