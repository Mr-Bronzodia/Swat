using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitBlackBoard BlackBoard { get; private set; }

    public Action<Command> OnNewCommand;

    [SerializeField]
    private int _framesPerAIUpdate;

    private int _sinceLastAIUpdate;

    private void Awake()
    {
        UnitBlackBoard unitBlackBoard;
        if (gameObject.TryGetComponent<UnitBlackBoard>(out unitBlackBoard)) BlackBoard = unitBlackBoard;
        else DebugUiManager.Instance.AddDebugText(GetHashCode(), "Unit " + gameObject.name + " does not have Backboard component attached");

        UnitManager.Instance.AddUnit(this);

        Idle idle = new Idle(this);
        BlackBoard.ScheduleNormalCommand(idle);

        _sinceLastAIUpdate = _framesPerAIUpdate;
    }

    private void Update()
    {
        _sinceLastAIUpdate++;

        if (_framesPerAIUpdate > _sinceLastAIUpdate) return;

        if (BlackBoard.CurrentCommand == null)
        {
            Command nextCommand = BlackBoard.CommandQueue.Dequeue();
            BlackBoard.SetCurrentCommand(nextCommand);
            OnNewCommand?.Invoke(nextCommand);
        }

        BlackBoard.CurrentCommand.Update();

        DebugUiManager.Instance.AddDebugText(3, "Current Selected AI state: " + BlackBoard.CurrentCommand.ToUIString());

        if (BlackBoard.CurrentCommand.CheckCommandCompleted())
        {
            Command nextCommand = BlackBoard.CommandQueue.Dequeue();
            BlackBoard.SetCurrentCommand(nextCommand);
            OnNewCommand?.Invoke(nextCommand);
        }

        _sinceLastAIUpdate = 0;
    }
}
