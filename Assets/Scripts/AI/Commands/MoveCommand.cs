using UnityEngine;
using UnityEngine.AI;


public class MoveCommand : Command
{
    Vector3 _target;

    public bool _terminateOnNextUpdate = false;

    public MoveCommand(Unit unit, Vector3 target) : base(unit)
    {
        _target = target;
        //Unit.OnStopImmediately += ImmediateStop;
    }

    ~MoveCommand() 
    {
        //Unit.OnStopImmediately -= ImmediateStop;
    }

    private void ImmediateStop()
    {
        _terminateOnNextUpdate = true;
        Unit.NavAgent.velocity = Vector3.zero;
        Unit.NavAgent.isStopped = true;
    } 


    public override bool CheckCommandCompleted()
    {
        if (_terminateOnNextUpdate) return true;


        return Unit.NavAgent.remainingDistance <= 0.1f;
    }

    public override string ToUIString()
    {
        return "Move";
    }

    public override void Update()
    {
        
    }

    protected override void OnCommandBeginExecute()
    {
        if (Unit.NavAgent.isStopped) Unit.NavAgent.isStopped = false;

        NavMeshHit navMeshHit;
        Vector3 nearestPoint;
        if (NavMesh.SamplePosition(_target, out navMeshHit, 1.5f, 1))
        {
            nearestPoint = navMeshHit.position;
        }
        else
        {
            Debug.Log("Cant find near navmesh point in move command terminating eraly");
            nearestPoint = Unit.BlackBoard.Position;
            _terminateOnNextUpdate = true;
        }

        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(Unit.BlackBoard.Position, nearestPoint, 1, path))
        {
            if (path.status != NavMeshPathStatus.PathInvalid)
            {
                Unit.NavAgent.SetPath(path);
            }
        }
        else
        {
            _terminateOnNextUpdate = true;
        }

    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
