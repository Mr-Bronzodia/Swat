using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurrenderCommand : Command
{
    public SurrenderCommand(Unit unit) : base(unit)
    {
    }

    public override bool CheckCommandCompleted()
    {
        return false;
    }

    public override string ToUIString()
    {
        return "Surrender [DEBUG VIEW ONLY]";
    }

    public override void Update()
    {

    }

    protected override void OnCommandBeginExecute()
    {
        GameManager.Instance.CapturedUnits++;
    }

    protected override void OnCommandEndExecute()
    {

    }
}
