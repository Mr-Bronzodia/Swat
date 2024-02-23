using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCommand : StopCommand
{
    public FreeCommand(Unit unit, float time) : base(unit, time)
    {
    }

    public override string ToUIString()
    {
        return "Free";
    }

    public override bool CheckCommandCompleted()
    {
        return base.CheckCommandCompleted();
    }

    public override void Update()
    {
        base.Update();
    }

    protected override void OnCommandBeginExecute()
    {
        base.OnCommandBeginExecute();
    }

    protected override void OnCommandEndExecute()
    {
        base.OnCommandEndExecute();
    }
}
