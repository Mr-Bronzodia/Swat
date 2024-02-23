using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime.Debug;
using UnityEngine;

public class TemporaryEnemyStateManager : MonoBehaviour
{
    private UnitVision _vision;
    private Unit _unit;
    private Vector3 _initialPosition;

    private int _sinceUpdate = 0;

    private void OnEnable()
    {
        _vision = gameObject.GetComponent<UnitVision>();
        _unit = gameObject.GetComponent<Unit>();
        _initialPosition = transform.position;
    }

    private void Reload()
    {
        ReloadCommand reloadCommand = new ReloadCommand(_unit, 2f);
        _unit.ScheduleHighCommand(reloadCommand);
    }

    private void Wonder()
    {
        Vector3 newTarget = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
        MoveCommand moveCommand = new MoveCommand(_unit, _initialPosition + newTarget);
        _unit.ScheduleNormalCommand(moveCommand);
        WaitForSecoundCommand wait = new WaitForSecoundCommand(_unit, 3f);
        _unit.ScheduleNormalCommand(wait);
    }

    private void Shoot(Unit target)
    {
        if (target.BlackBoard.CurrentHealth <= 0)
        {
            StopCommand stop = new StopCommand(_unit, .01f);
            _unit.ScheduleHighCommand(stop);
        }

        if (_unit.BlackBoard.CurrentCommand.GetType() == typeof(MoveCommand))
        {
            StopCommand stop = new StopCommand(_unit, .01f);
            _unit.ScheduleHighCommand(stop);
        }

        if (_unit.BlackBoard.CurrentCommand.GetType() != typeof(ShootCommand))
        {
            ShootCommand shoot = new ShootCommand(_unit, target);
            _unit.ScheduleHighCommand(shoot);
        }
    }


    private void Update()
    {
        if (_unit.BlackBoard.CurrentCommand == null) return;

        if (_unit.BlackBoard.CurrentCommand.GetType() == typeof(NeutralizedCommand)) return;

        _sinceUpdate++;


        if ( _sinceUpdate <= 2 ) return;

        _sinceUpdate = 0;

        if (_unit.BlackBoard.Weapon.RemainingBullets <= 0)
        {
            Reload();
            return;
        }

        if (_vision._visibleTargetsList.Count > 0)
        {
            foreach (Unit enemy in _vision._visibleTargetsList)
            {
                if (enemy.BlackBoard.CurrentCommand.GetType() == typeof(NeutralizedCommand)) continue;
                Shoot(enemy);
                return;
            }
            
        }

        if (_unit.BlackBoard.CurrentCommand.GetType() == typeof(Idle)) Wonder();

    }
}
