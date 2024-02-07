using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCommand : Command
{
    Unit _unitToFollow;
    float _followDistance;

    public FollowCommand(Unit unit, Unit unitToFollow, float distance = 0) : base(unit)
    {
        _unitToFollow = unitToFollow;
        _followDistance = distance;
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

        float distance;
        if (Mathf.Approximately(_followDistance, 0)) distance = 1f;
        else distance = _followDistance;

        if (Vector3.Distance(myPosition, otherPosition) < distance + 0.1f) return;

        Vector3 dirToTarget = (otherPosition - myPosition).normalized;
        Unit.NavAgent.SetDestination(otherPosition - distance * dirToTarget);
    }

    protected override void OnCommandBeginExecute()
    {
        
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
