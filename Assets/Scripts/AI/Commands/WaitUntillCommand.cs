using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitUntillCommand : Command
{
    Unit _other;
    bool _isOtherDone = false;
    System.Type _type;

    public WaitUntillCommand(Unit unit, Unit waitTarget, System.Type commandType) : base(unit)
    {
        _other = waitTarget;
        _type = commandType;
        _other.OnNewCommand += WaitForFollowingCommand;
    }

    ~WaitUntillCommand()
    {
        _other.OnNewCommand -= WaitForFollowingCommand;
    }

    private void WaitForFollowingCommand(Command next)
    {
        
        if (next.GetType() == _type) _isOtherDone = true;
    }

    public override bool CheckCommandCompleted()
    {
        if (_isOtherDone) return true;

        if (Unit.BlackBoard.CommandQueue.Count < 0) return true;

        return false;
    }

    public override string ToUIString()
    {
        return "wait until command DEBUG SEE ONLY";
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
