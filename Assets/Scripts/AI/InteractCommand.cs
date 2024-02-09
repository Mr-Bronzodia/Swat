using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCommand : Command
{
    Iinteract _interactionObject;
    float _delay;
    float _currentTime;
    bool _isDone = false;


    public InteractCommand(Unit unit, Iinteract interactionObject, float delay) : base(unit)
    {
       _interactionObject = interactionObject;
        _delay = delay;
    }

    public override bool CheckCommandCompleted()
    {
        return _isDone;
    }

    public override string ToUIString()
    {
        return "Interact DEBUG SEE ONLY";
    }

    public override void Update()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= _delay) _isDone = true;
    }

    protected override void OnCommandBeginExecute()
    {
        
    }

    protected override void OnCommandEndExecute()
    {
        _interactionObject.Interact();
    }
}
