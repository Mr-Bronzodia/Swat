using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitBlackBoard : MonoBehaviour
{
    private NavMeshAgent _agent;

    [SerializeField]
    private float _maxHealth;
    [SerializeField]
    private Team _team;
    [SerializeField]
    [Range(0f, 1f)]
    private float _accuracy;

    public Team Team { get => _team;  }
    public float Accuracy { get => _accuracy; }
    public float MaxHealth { get => _maxHealth; }
    public float CurrentHealth { get; private set; }
    public Command CurrentCommand { get; private set; }
    public Queue<Command> CommandQueue { get; private set; }
    public Queue<Command> HighPriorityCommandQueue { get; private set; }
    public float MovementSpeed { get => _agent.speed;  set => _agent.speed = value;  }
    public Vector3 Position { get => gameObject.transform.position; }
    public Weapon Weapon { get; private set; }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        CommandQueue = new Queue<Command>();
        HighPriorityCommandQueue = new Queue<Command>();
        _agent = GetComponent<NavMeshAgent>();
        Weapon = gameObject.GetComponentInChildren<Weapon>();
    }

    public void SetCurrent(Command newCommand)
    {
        CurrentCommand = newCommand;
    }

}
