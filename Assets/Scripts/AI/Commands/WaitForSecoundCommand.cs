using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForSecoundCommand : Command
{
    private float _timeLeft;

    public WaitForSecoundCommand(Unit unit, float time) : base(unit)
    {
        _timeLeft = time;
    }

    public override bool CheckCommandCompleted()
    {
        return _timeLeft <= 0f;
    }

    public override string ToUIString()
    {
        return "Wait for secounds [DEBUG VIEW ONLY]";
    }

    public override void Update()
    {
        _timeLeft -= Time.deltaTime;
    }

    protected override void OnCommandBeginExecute()
    {

    }

    protected override void OnCommandEndExecute()
    {

    }
}
