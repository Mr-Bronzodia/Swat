using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EvaluateThreatCommand : Command
{
    private bool _shouldSurrender;

    public EvaluateThreatCommand(Unit unit) : base(unit)
    {
    }

    public override bool CheckCommandCompleted()
    {
        return true;
    }

    public override string ToUIString()
    {
        return "Evaluate Threat [DEBUG VIEW ONLY]";
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {
        Collider[] hitCollider = Physics.OverlapSphere(Unit.BlackBoard.Position, 5f);

        float nearAllyMorale = 0;
        float nearEnemyMorale = 0;

        foreach (Collider collider in hitCollider)
        {
            Unit unitInSphere;

            if (!collider.gameObject.TryGetComponent<Unit>(out unitInSphere)) continue;

            if (unitInSphere.BlackBoard.CurrentCommand.GetType() == typeof(NeutralizedCommand)) continue;

            if (unitInSphere.BlackBoard.Team == Unit.BlackBoard.Team)
            {
                nearAllyMorale += unitInSphere.BlackBoard.Morale;
            }
            else
            {
                nearEnemyMorale += unitInSphere.BlackBoard.Morale;
            }
        }

        Debug.Log("nearAllyMorale " + nearAllyMorale);
        Debug.Log("nearEnemyMorale " + nearEnemyMorale);

        if (nearAllyMorale < nearEnemyMorale)
        {
            SurrenderCommand surrender = new SurrenderCommand(Unit);
            Unit.ScheduleNormalCommand(surrender);
        }
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
