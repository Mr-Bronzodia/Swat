using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

[RequireComponent(typeof(NavMeshAgent), typeof(UnitBlackBoard))]
public class Unit : MonoBehaviour, IClickable, IDamageable
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

    private bool _isGamePaused;

    [SerializeField]
    private bool _isHostage;


    private void Start()
    {
        if (!_isHostage) return;

        SurrenderCommand hostageSurrender = new SurrenderCommand(this);
        SetCurrentCommand(hostageSurrender);
    }

    private void OnEnable()
    {

        BlackBoard = gameObject.GetComponent<UnitBlackBoard>();
        NavAgent = gameObject.GetComponent<NavMeshAgent>();

        UnitManager.Instance.AddUnit(this);

        PauseManager.Instance.OnPauseStart += Pause;
        PauseManager.Instance.OnPauseEnd += UnPause;

        _sinceLastAIUpdate = 0;
    }

    private void OnDisable()
    {
        PauseManager.Instance.OnPauseStart -= Pause;
        PauseManager.Instance.OnPauseEnd -= UnPause;
    }

    private void Pause() 
    {
        _isGamePaused = true;
        NavAgent.isStopped = true;
    }

    private void UnPause()
    {
        _isGamePaused = false;
        NavAgent.isStopped = false;
    }

    public void SetSelectionVisual(bool enabled)
    {
        _selectionVisual.SetActive(enabled);
    }

    public void RotateTowardPoint(Vector3 point)
    {
        transform.LookAt(point);
    }

    public void ReceiveMoraleDamage(float damage)
    {
        float adjustedAmount = damage - (damage * BlackBoard.MoraleResistance);

        Debug.Log(adjustedAmount);
        Debug.Log(BlackBoard.Morale);

        BlackBoard.SetMorale(BlackBoard.Morale - adjustedAmount);
    }

    private void Update()
    {
        if (_isGamePaused) return;

        _sinceLastAIUpdate++;

        if (_framesPerAIUpdate > _sinceLastAIUpdate) return;

        if (BlackBoard.CurrentHealth <= 0 && BlackBoard.CurrentCommand.GetType() != typeof(NeutralizedCommand))
        {
            Command dead = new NeutralizedCommand(this);
            BlackBoard.CurrentCommand.ExecuteNext(dead);
            SetCurrentCommand(dead);
            OnNewCommand?.Invoke(dead);
        }

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

        if (GameManager.Instance.PlayerTeam == BlackBoard.Team) GameManager.Instance.NoCommandsIssued++;
    }

    public void ScheduleHighCommand(Command command)
    {
        BlackBoard.HighPriorityCommandQueue.Enqueue(command);
    }

    public List<Command> GetAvailableCommands(Unit other)
    {
        List<Command> commands = new List<Command>();



        //Ally
        if (BlackBoard.Team == ETeam.Blue)
        {
            FollowCommand followCommand = new FollowCommand(other, this);
            commands.Add(followCommand);

            if (other == this)
            {
                StopCommand stop = new StopCommand(this, .1f);
                commands.Add(stop);

                //unit has a weapon
                if (BlackBoard.Weapon != null)
                {
                    ReloadCommand reload = new ReloadCommand(this, 2f);
                    commands.Add(reload);
                }   
            }
        }
        //Enemy
        else if (BlackBoard.Team == ETeam.Red)
        {
            NeutralizeEnemyCommand neutralize = new NeutralizeEnemyCommand(other, this, 0.3f);
            commands.Add(neutralize);

            ShootCommand shoot = new ShootCommand(other, this);
            commands.Add(shoot);

            IntimidateCommand intimidate = new IntimidateCommand(other, this, .3f);
            commands.Add(intimidate);
            
        }

        else if (BlackBoard.Team == ETeam.Hostage)
        {
            FollowCommand followTeam = new FollowCommand(this, other);
            commands.Add(followTeam);

            StopCommand stopHostage = new StopCommand(this, .1f);
            commands.Add(stopHostage);

        }


        return commands;
    }

    public List<Command> GetAvailableCommands(List<Unit> units)
    {
        throw new NotImplementedException();
    }

    public void ReceiveDamage(float damage)
    {
        BlackBoard.CurrentHealth -= damage;
    }
}
