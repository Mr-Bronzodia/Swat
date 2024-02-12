using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralizedCommand : Command
{
    public NeutralizedCommand(Unit unit) : base(unit)
    {
    }

    public override bool CheckCommandCompleted()
    {
        return false;
    }

    public override string ToUIString()
    {
        return "Neutralized [DEBUG VIEW ONLY]";
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {
        Unit.NavAgent.isStopped = true;
        Unit.NavAgent.SetDestination(Unit.BlackBoard.Position);
        Unit.BlackBoard.CommandQueue.Clear();
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
