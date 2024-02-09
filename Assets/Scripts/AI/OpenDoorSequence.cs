using PlasticGui.WorkspaceWindow.PendingChanges;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorSequence : Command
{
    Iinteract _door;
    Vector3 _doorPos;


    public OpenDoorSequence(Unit unit, Iinteract door, Vector3 doorPosition) : base(unit)
    {
        _door = door;
        _doorPos = doorPosition;
    }

    public override bool CheckCommandCompleted()
    {
        return true;
    }

    public override string ToUIString()
    {
        return "Open Door";
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {
        MoveCommand moveToDoor = new MoveCommand(Unit, _doorPos);
        InteractCommand openDoor = new InteractCommand(Unit, _door, 0.5f);

        Unit.ScheduleNormalCommand(moveToDoor);
        Unit.ScheduleNormalCommand(openDoor);
        
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
