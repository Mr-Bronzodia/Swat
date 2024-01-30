using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private Team _team;
    public UnitBlackBoard BlackBoard { get; private set; }

    public Action<Command> OnNewCommand;

    private void Awake()
    {
        UnitBlackBoard unitBlackBoard;
        if (gameObject.TryGetComponent<UnitBlackBoard>(out unitBlackBoard)) BlackBoard = unitBlackBoard;
        else DebugUiManager.Instance.AddDebugText(GetHashCode(), "Unit " + gameObject.name + " does not have Backboard component attached");

        BlackBoard.SetTeam(_team);
    }
}
