using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command 
{
    protected Unit _unit;

    public Command(Unit unit)
    {
        _unit = unit;
    }
    
    public abstract void Update();

    protected abstract void OnCommandBeginExecute();

    protected abstract void OnCommandEndExecute();

    protected abstract bool CheckCommandCompleted();

    public abstract string ToUIString();

    protected void ExecuteNext(Command nextCommand)
    {
        OnCommandEndExecute();

        nextCommand.OnCommandBeginExecute();
    }
}
