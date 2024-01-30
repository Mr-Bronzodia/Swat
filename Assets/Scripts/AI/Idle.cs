using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : Command
{
    public Idle(Unit unit) : base(unit)
    {
    }

    public override string ToUIString()
    {
        return "Idle";
    }

    public override void Update()
    {
    }

    public override bool CheckCommandCompleted()
    {
        if (_unit.BlackBoard.CommandQueue.Count > 1) return true;

        return false;
    }

    protected override void OnCommandBeginExecute()
    {
        
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
