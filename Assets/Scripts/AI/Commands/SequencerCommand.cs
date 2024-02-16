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
        Dictionary<Unit, List<Command>> commandsByUnit = new Dictionary<Unit, List<Command>>();

        foreach (Command command in _commands)
        {
            if (!commandsByUnit.ContainsKey(command.Unit)) commandsByUnit.Add(command.Unit, new List<Command>());

            commandsByUnit[command.Unit].Add(command);
        }

        foreach (var entry in commandsByUnit)
        {
            Command[] queueCopy = entry.Key.BlackBoard.CommandQueue.ToArray();
            entry.Key.BlackBoard.CommandQueue.Clear();

            foreach (Command command in entry.Value)
            {
                entry.Key.ScheduleNormalCommand(command);
            }

            foreach(var command in queueCopy)
            {
                entry.Key.ScheduleNormalCommand(command);
            }


        }
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
