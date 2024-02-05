using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableCover : MonoBehaviour, IClickable
{
    [SerializeField]
    List<Transform> _coverSpots;

    public List<Command> GetAvailableCommands(Unit unit)
    {
        float lowestDistance = float.MaxValue;
        Transform closestPosition = null;

        foreach (Transform cover in _coverSpots)
        {
            float distance = Vector3.Distance(unit.gameObject.transform.position, cover.position);
            if (distance > lowestDistance) continue;

            lowestDistance = distance;
            closestPosition = cover;
        }

        return new List<Command>() { new MoveCommand(unit, closestPosition.position) };
    }

    public List<Command> GetAvailableCommands(List<Unit> units)
    {
        throw new System.NotImplementedException();
    }
}
