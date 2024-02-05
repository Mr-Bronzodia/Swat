using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private void OnCollisionEnter(Collision collision)
    {
        Unit other;
        if (!collision.gameObject.TryGetComponent<Unit>(out other)) return;

        OnStopImmediately?.Invoke();
    }

    private void Update()
    {
        if (BlackBoard.CurrentCommand == null && BlackBoard.CommandQueue.Count <= 0)
        {
            Idle idle = new Idle(this);
            BlackBoard.ScheduleNormalCommand(idle);
            BlackBoard.SetCurrentCommand(idle);
        }

        _sinceLastAIUpdate++;

        if (_framesPerAIUpdate > _sinceLastAIUpdate) return;

        if (BlackBoard.CurrentCommand == null)
        {
            Command nextCommand = BlackBoard.CommandQueue.Dequeue();
            BlackBoard.CurrentCommand.ExecuteNext(nextCommand);
            OnNewCommand?.Invoke(nextCommand);
        }

        BlackBoard.CurrentCommand.Update();

        DebugUiManager.Instance.AddDebugText(GetHashCode(), gameObject.name + " AI command: " + BlackBoard.CurrentCommand.ToUIString());

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

    public List<Command> GetAvailableCommands(Unit other)
    {
        List<Command> commands = new List<Command>();

        //Ally
        if (BlackBoard.Team == other.BlackBoard.Team)
        {
            FollowCommand followCommand = new FollowCommand(other, this);
            WaitToFinishCommand wait = new WaitToFinishCommand(other, this);

            commands.Add(followCommand);
            commands.Add(wait);
        }
        //Enemy
        else
        {

        }


        return commands;
    }

    public List<Command> GetAvailableCommands(List<Unit> units)
    {
        throw new NotImplementedException();
    }
}
