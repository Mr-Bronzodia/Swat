using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBlackBoard : MonoBehaviour
{
    public float MaxHealth;
    public Team Team;

    public float CurrentHealth { get; private set; }
    public Command CurrentCommand { get; private set; }
    public Queue<Command> CommandQueue { get; private set; }
    public Queue<Command> HighPriorityCommandQueue { get; private set; }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        CommandQueue = new Queue<Command>();
        HighPriorityCommandQueue = new Queue<Command>();
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
