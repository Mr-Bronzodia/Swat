using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(UnitBlackBoard))]
public class Unit : MonoBehaviour, IClickable
{
    public UnitBlackBoard BlackBoard { get; private set; }

    public Action<Command> OnNewCommand;

    public Action OnStopImmediately;

    public NavMeshAgent NavAgent { get; private set; }

    [SerializeField]
    private GameObject _selectionVisual;

    [SerializeField]
    private int _framesPerAIUpdate;

    private int _sinceLastAIUpdate;


    private void OnEnable()
    {
        UnitBlackBoard unitBlackBoard;
        if (gameObject.TryGetComponent<UnitBlackBoard>(out unitBlackBoard)) BlackBoard = unitBlackBoard;
        else DebugUiManager.Instance.AddDebugText(GetHashCode(), "Unit " + gameObject.name + " does not have Backboard component attached");

        NavAgent = gameObject.GetComponent<NavMeshAgent>();

        UnitManager.Instance.AddUnit(this);

        _sinceLastAIUpdate = 0;
    }

    public void SetSelectionVisual(bool enabled)
    {
        _selectionVisual.SetActive(enabled);
    }

    public void RotateTowardPoint(Vector3 point)
    {
        transform.LookAt(point);
    }

    private void Update()
    {
        _sinceLastAIUpdate++;

        if (_framesPerAIUpdate > _sinceLastAIUpdate) return;

        // Execute High Priority Command Above else
        if (BlackBoard.HighPriorityCommandQueue.Count > 0)
        {
            Command command = BlackBoard.HighPriorityCommandQueue.Dequeue();
            BlackBoard.CurrentCommand.ExecuteNext(command);
            SetCurrentCommand(command);
            OnNewCommand?.Invoke(command);
        }

        //If unit just created make idle
        if (BlackBoard.CurrentCommand == null && BlackBoard.CommandQueue.Count <= 0)
        {
            Idle idle = new Idle(this);
            ScheduleNormalCommand(idle);
            SetCurrentCommand(idle);
        }

        BlackBoard.CurrentCommand.Update();

        DebugUiManager.Instance.AddDebugText(GetHashCode(), gameObject.name + " AI command: " + BlackBoard.CurrentCommand.ToUIString());

        // if current command finished process next
        if (BlackBoard.CurrentCommand.CheckCommandCompleted())
        {
            Command nextCommand;
            if (BlackBoard.CommandQueue.Count == 0) nextCommand = new Idle(this);
            else nextCommand = BlackBoard.CommandQueue.Dequeue();


            BlackBoard.CurrentCommand.ExecuteNext(nextCommand);
            OnNewCommand?.Invoke(nextCommand);
        }

        _sinceLastAIUpdate = 0;
    }

    public void SetAIUpdateRate(int framesPreUpdate)
    {
        _framesPerAIUpdate = framesPreUpdate;
    }

    public void SetCurrentCommand(Command command)
    {
        BlackBoard.SetCurrent(command);
    }

    public void ScheduleNormalCommand(Command command)
    {
        BlackBoard.CommandQueue.Enqueue(command);
    }

    public void ScheduleHighCommand(Command command)
    {
        BlackBoard.HighPriorityCommandQueue.Enqueue(command);
    }

    public List<Command> GetAvailableCommands(Unit other)
    {
        List<Command> commands = new List<Command>();

        //Ally
        if (BlackBoard.Team == other.BlackBoard.Team)
        {
            FollowCommand followCommand = new FollowCommand(other, this);

            if (other == this)
            {
                StopCommand stop = new StopCommand(this, .1f);
                commands.Add(stop);

                ReloadCommand reload = new ReloadCommand(this, 2f);
                commands.Add(reload);
            }

            commands.Add(followCommand);
        }
        //Enemy
        else
        {
            NeutralizeEnemyCommand neutralize = new NeutralizeEnemyCommand(other, this, 0.3f);
            commands.Add(neutralize);

            ShootCommand shoot = new ShootCommand(other, this);
            commands.Add(shoot);
        }


        return commands;
    }

    public List<Command> GetAvailableCommands(List<Unit> units)
    {
        throw new NotImplementedException();
    }
}
