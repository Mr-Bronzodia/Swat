using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCommand : Command
{
    private bool _isDone = false;
    private float _timeToElapse;
    private float _currentTimeElapsed = 0;

    public StopCommand(Unit unit, float time) : base(unit)
    {
        _timeToElapse = time;
    }

    public override bool CheckCommandCompleted()
    {
        return _isDone;
    }

    public override string ToUIString()
    {
        return "Stop";
    }

    public override void Update()
    {
        _currentTimeElapsed += Time.deltaTime;
        if (_currentTimeElapsed >= _timeToElapse) _isDone = true;
    }

    protected override void OnCommandBeginExecute()
    {
        if (!Unit.NavAgent.isStopped) Unit.NavAgent.isStopped = true;
        if (Unit.NavAgent.remainingDistance > 0) Unit.NavAgent.SetDestination(Unit.BlackBoard.Position);

        Unit.BlackBoard.CommandQueue.Clear();
    }

    protected override void OnCommandEndExecute()
    {
        Unit.NavAgent.isStopped = false;
    }
}
