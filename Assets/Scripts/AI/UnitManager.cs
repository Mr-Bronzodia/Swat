using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<Unit> _blueTeam = new List<Unit>();
    private List<Unit> _redTeam = new List<Unit>();


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddUnit(Unit unit)
    {
        if (unit.BlackBoard.Team == Team.Red) _redTeam.Add(unit);
        else _blueTeam.Add(unit);
    }


    public int GetTeamSize(Team team)
    {
        if (team == Team.Red) return _redTeam.Count;

        if (team == Team.Blue) return _blueTeam.Count;

        Debug.LogError("Inquiring about non existing team Size");
        return 0;
    }

    public Unit GetUnitAtIndex(int index, Team team)
    {
        if (team == Team.Red) return _redTeam[index];

        if (team == Team.Blue) return _blueTeam[index];

        Debug.LogError("Out of Index Team call");
        return null;
    }
}
