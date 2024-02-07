using Codice.CM.WorkspaceServer.Tree.GameUI.Checkin.Updater;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

public class TeamMoveSequence : Command
{
    private List<Unit> _team;
    private Vector3 _target;

    public TeamMoveSequence(Unit _, List<Unit> team, Vector3 target) : base(_)
    {
        _target = target;
        _team = team;
    }

    public override bool CheckCommandCompleted()
    {
        return true;
    }

    public override string ToUIString()
    {
        return "Team move sequence";
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {
        Unit lead = _team[0];
        Vector3 leadForward = lead.gameObject.transform.forward;

        for (int i = 1; i < _team.Count; i++)
        {
            Unit unit = _team[i];
            Vector3 target = lead.transform.position - i * leadForward;

            MoveCommand moveBehind = new MoveCommand(unit, target);
            unit.ScheduleNormalCommand(moveBehind);

            WaitUntillCommand leadWait = new WaitUntillCommand(lead, unit, typeof(FollowCommand));
            lead.ScheduleNormalCommand(leadWait);

            FollowCommand unitFollowLead = new FollowCommand(unit, lead, i);
            unit.ScheduleNormalCommand(unitFollowLead);
        }

        MoveCommand leadMove = new MoveCommand(lead, _target);
        lead.ScheduleNormalCommand(leadMove);
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
