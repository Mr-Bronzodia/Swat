using Codice.Client.BaseCommands.Merge;
using Codice.CM.SEIDInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralizeEnemyCommand : Command
{
    Unit _other;
    private float _requiredTime;
    private float _initialMovementSpeed;
    private bool _isDone;

    public NeutralizeEnemyCommand(Unit unit, Unit other, float timeRequired) : base(unit)
    {
        _other = other;
        _requiredTime = timeRequired;
    }

    public override bool CheckCommandCompleted()
    {
        return _isDone;
    }

    public override string ToUIString()
    {
        return "Neutralize Enemy";
    }

    public override void Update()
    {
        Vector3 otherPosition = _other.BlackBoard.Position;
        Vector3 myPosition = Unit.BlackBoard.Position;

        float distance = .8f;

        if ( Vector3.Distance(myPosition, otherPosition) <= distance + .3f) 
        {
            NeutralizedCommand enemyNeutralize = new NeutralizedCommand(_other);
            _other.ScheduleHighCommand(enemyNeutralize);

            StopCommand stopSelf = new StopCommand(Unit, _requiredTime);
            Unit.ScheduleHighCommand(stopSelf);

            _isDone = true;
            return;
        }

        Vector3 dirToTarget = (otherPosition - myPosition).normalized;
        Unit.NavAgent.SetDestination(otherPosition - distance * dirToTarget);
    }

    protected override void OnCommandBeginExecute()
    {
        _initialMovementSpeed = Unit.BlackBoard.MovementSpeed;
        Unit.BlackBoard.MovementSpeed = _other.BlackBoard.MovementSpeed + 0.75f;
    }

    protected override void OnCommandEndExecute()
    {
        Unit.BlackBoard.MovementSpeed = _initialMovementSpeed;
    }
}
