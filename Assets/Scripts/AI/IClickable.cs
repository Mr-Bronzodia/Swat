using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable
{
    public List<Command> GetAvailableCommands(Unit unit);
}
