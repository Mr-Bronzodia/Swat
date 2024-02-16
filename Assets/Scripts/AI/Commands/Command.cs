using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command 
{
    public Unit Unit {  get; private set; }

    public Command(Unit unit)
    {
        Unit = unit;
    }
    
    public abstract void Update();

    protected abstract void OnCommandBeginExecute();

    protected abstract void OnCommandEndExecute();

    public abstract bool CheckCommandCompleted();

    public abstract string ToUIString();

    public void ExecuteNext(Command nextCommand)
    {
        OnCommandEndExecute();

        Unit.SetCurrentCommand(nextCommand);

        nextCommand.OnCommandBeginExecute();
    }
}
