using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerCommand : Command
{
    string _name;
    List<Command> _commands;

    public SequencerCommand(Unit unit, string name, List<Command> commands) : base(unit)
    {
        _name = name;
        _commands = commands;

    }

    public override bool CheckCommandCompleted()
    {
        return true;
    }

    public override string ToUIString()
    {
        return _name;
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {
        foreach (var command in _commands)
        {
            command.Unit.ScheduleNormalCommand(command);
        }
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
