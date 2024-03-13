using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class TakeCoverCommand : Command
{
    Vector3 _coverPoint;
    bool _terminateOnNextUpdate = false;

    public TakeCoverCommand(Unit unit, Vector3 _coverPoint) : base(unit)
    {
        this._coverPoint = _coverPoint;
    }

    public override bool CheckCommandCompleted()
    {
        if (Unit.BlackBoard.CommandQueue.Count >= 1) return true;

        if (_terminateOnNextUpdate) return true;

        return false;
    }

    public override string ToUIString()
    {
        return "Take Cover";
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {
        Vector3 unitToWallDir = Vector3.Cross(_coverPoint, Unit.transform.forward);
        Debug.DrawRay(Unit.BlackBoard.Position, unitToWallDir * 2f);
        Unit.transform.forward = unitToWallDir;
        //Debug.Break();
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
