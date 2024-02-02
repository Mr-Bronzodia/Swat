using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    Vector3 _target;

    public MoveCommand(Unit unit, Vector3 target) : base(unit)
    {
        _target = target;
    }

    public override bool CheckCommandCompleted()
    {
        Vector3 unitPos = _unit.transform.position;

        return Mathf.Approximately((_target - unitPos).magnitude, 0f);
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
        _unit.NavAgent.SetDestination(_target);
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
