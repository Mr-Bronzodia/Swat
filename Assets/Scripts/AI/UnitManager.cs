using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<Unit> _blueTeam = new List<Unit>();
    private List<Unit> _redTeam = new List<Unit>();
    private List<Unit> _hostage = new List<Unit>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;

        }
    }

    public void AddUnit(Unit unit)
    {
        if (unit.BlackBoard.Team == ETeam.Red) _redTeam.Add(unit);
        else if (unit.BlackBoard.Team == ETeam.Blue)_blueTeam.Add(unit);
        else _hostage.Add(unit);
    }

    public void InflictMoraleDamageTeam(Unit source, float amount)
    {
        if (source.BlackBoard.Team == ETeam.Red)
        {
            foreach (Unit unit in _redTeam)
            {
                unit.ReceiveMoraleDamage(amount);
            }
        }
        else
        {
            foreach (Unit unit in _blueTeam)
            {
                unit.ReceiveMoraleDamage( amount);
            }
        }
    }


    public int GetTeamSize(ETeam team)
    {
        if (team == ETeam.Red) return _redTeam.Count;

        if (team == ETeam.Blue) return _blueTeam.Count;

        Debug.LogError("Inquiring about non existing team Size");
        return 0;
    }

    public Unit GetUnitAtIndex(int index, ETeam team)
    {
        if (team == ETeam.Red) return _redTeam[index];

        if (team == ETeam.Blue) return _blueTeam[index];

        Debug.LogError("Out of Index Team call");
        return null;
    }
}
