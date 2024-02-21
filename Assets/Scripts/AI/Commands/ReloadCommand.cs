using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadCommand : Command
{
    private float _remainingTime;

    public ReloadCommand(Unit unit, float time) : base(unit)
    {
        _remainingTime = time;
    }

    public override bool CheckCommandCompleted()
    {
        return _remainingTime <= 0;
    }

    public override string ToUIString()
    {
        return "Reload";
    }

    public override void Update()
    {
        _remainingTime -= Time.deltaTime;
    }

    protected override void OnCommandBeginExecute()
    {
        
    }

    protected override void OnCommandEndExecute()
    {
        Unit.BlackBoard.Weapon.Reload();
        UIManager.Instance.UpdateAmmoUIAmmoCount(Unit.GetHashCode(), Unit.BlackBoard.Weapon.MagazineSize, Unit.BlackBoard.Weapon.RemainingBullets);
    }
}
