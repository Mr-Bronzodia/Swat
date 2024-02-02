using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour ,IClickable
{
    public List<Command> GetAvailableCommands(Unit u)
    {
        return new List<Command>() { new MoveCommand(u, Camera.main.ScreenToWorldPoint(Input.mousePosition))};
    }
}
