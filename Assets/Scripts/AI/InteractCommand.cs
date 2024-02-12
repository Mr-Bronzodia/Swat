using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCommand : Command
{
    Iinteract _interactionObject;
    float _delay;
    float _currentTime;
    bool _isDone = false;
    Vector3 _interactionLocation;


    public InteractCommand(Unit unit, Iinteract interactionObject, float delay, Vector3 interactionLocation) : base(unit)
    {
        _interactionObject = interactionObject;
        _interactionLocation = interactionLocation;
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
        if (Vector3.Distance(Unit.BlackBoard.Position, _interactionLocation) < .5f) _interactionObject.Interact();
    }
}
