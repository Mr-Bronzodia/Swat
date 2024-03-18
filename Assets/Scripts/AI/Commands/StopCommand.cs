using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCommand : Command
{
    protected bool _isDone = false;
    protected float _timeToElapse;
    protected float _currentTimeElapsed = 0;
    protected string _uiText = "Stop";

    public StopCommand(Unit unit, float time, string uiText = "Stop") : base(unit)
    {
        _timeToElapse = time;
        _uiText = uiText;
    }

    public override bool CheckCommandCompleted()
    {
        return _isDone;
    }

    public override string ToUIString()
    {
        return _uiText;
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
