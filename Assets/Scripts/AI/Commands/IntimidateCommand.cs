using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntimidateCommand : Command
{
    private float _remainingTime;
    private Unit _other;

    public IntimidateCommand(Unit unit, Unit other, float time) : base(unit)
    {
        _remainingTime = time;
        _other = other;
    }

    public override bool CheckCommandCompleted()
    {
        return _remainingTime <= 0f;
    }

    public override string ToUIString()
    {
        return "Intimidate";
    }

    public override void Update()
    {
        _remainingTime -= Time.deltaTime;
    }

    protected override void OnCommandBeginExecute()
    {
        
    }

    protected override void OnCommandEndExecute()
    {
        EvaluateThreatCommand enemyEvaluate = new EvaluateThreatCommand(_other);
        _other.ScheduleHighCommand(enemyEvaluate);
    }
}
