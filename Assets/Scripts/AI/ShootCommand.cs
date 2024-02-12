using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCommand : Command
{
    private Unit _other;
    private float _delayPerShot = 0;
    private float _currentSotDelay = 0;
    private bool _terminateEarly = false;

    public ShootCommand(Unit unit, Unit other) : base(unit)
    {
        _other = other;
        if (Unit.BlackBoard.Weapon != null) _delayPerShot = 1f / Unit.BlackBoard.Weapon.FireRate;
    }

    public override bool CheckCommandCompleted()
    {
        if (_terminateEarly) return true; 

        return Unit.BlackBoard.CommandQueue.Count >= 1 || Unit.BlackBoard.Weapon.RemainingBullets == 0;
    }

    public override string ToUIString()
    {
        return "Shoot";
    }

    public override void Update()
    {
        if (Unit.BlackBoard.Weapon == null) { _terminateEarly = true; return; }

        _currentSotDelay += Time.deltaTime;
        if (_currentSotDelay > _delayPerShot )
        {
            Unit.BlackBoard.Weapon.Shoot((_other.BlackBoard.Position - Unit.BlackBoard.Position).normalized, Unit.BlackBoard.Accuracy); 
            _currentSotDelay = 0;
        }
    }

    protected override void OnCommandBeginExecute()
    {
        Unit.RotateTowardPoint(_other.BlackBoard.Position);
    }

    protected override void OnCommandEndExecute()
    {
        
    }
}
