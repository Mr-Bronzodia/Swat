using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCommand : Command
{
    Unit _unitToFollow;

    public FollowCommand(Unit unit, Unit other) : base(unit)
    {
        _unitToFollow = other;
    }

    public override bool CheckCommandCompleted()
    {
        if (Unit.BlackBoard.CommandQueue.Count >= 1) return true;

        return false;
    }

    public override string ToUIString()
    {
        return "Follow";
    }

    public override void Update()
    {
        Vector3 otherPosition = _unitToFollow.transform.position;
        Vector3 myPosition = Unit.gameObject.transform.position;
        if (Vector3.Distance(myPosition, otherPosition) < .9f) return;

        Vector3 dirToTarget = (otherPosition - myPosition).normalized;
        Unit.NavAgent.SetDestination(otherPosition - .8f * dirToTarget);
    }

    protected override void OnCommandBeginExecute()
    {
        
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
