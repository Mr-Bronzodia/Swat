using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitToFinishCommand : Command
{
    Unit _other;
    bool _isOtherDone = false;


    public WaitToFinishCommand(Unit unit, Unit other) : base(unit)
    {
        _other = other;
        _other.OnNewCommand += OtherHasFinished;
    }

    ~WaitToFinishCommand() 
    {
        _other.OnNewCommand -= OtherHasFinished;
    }

    private void OtherHasFinished(Command _) { _isOtherDone = true; }

    public override bool CheckCommandCompleted()
    {
        return _isOtherDone;
    }

    public override string ToUIString()
    {
        return "Wait To finish current task";
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {
        
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
