using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBlackBoard : MonoBehaviour
{
    public float MaxHealth;
    public float CurrentHealth { get; private set; }
    public Command CurrentCommand { get; private set; }
    public Queue<Command> CommandQueue { get; private set; }
    public Queue<Command> HighPriorityCommandQueue { get; private set; }
    public Team Team { get; private set; }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void SetTeam(Team team)
    {
        Team = team;
    }

    public void SetCurrentCommand(Command command)
    {
        CurrentCommand = command;
    }

    public void ScheduleNormalCommand(Command command)
    {
        CommandQueue.Enqueue(command);
    }

    public void ScheduleHighCommand(Command command)
    {
        HighPriorityCommandQueue.Enqueue(command);
    }
}
