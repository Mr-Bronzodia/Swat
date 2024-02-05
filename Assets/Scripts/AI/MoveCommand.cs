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

        Vector3 unitPos = Unit.transform.position;
        return Unit.NavAgent.remainingDistance <= 0.1f;
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

        Unit.NavAgent.SetDestination(_target);
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
