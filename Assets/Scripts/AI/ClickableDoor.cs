using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static PlasticGui.LaunchDiffParameters;
using static UnityEngine.UI.CanvasScaler;

public class ClickableDoor : MonoBehaviour, IClickable, Iinteract
{
    Quaternion _desiredRotation;

    private void ToggleNearNavLinks(bool enabled)
    {
        Collider[] hits = Physics.OverlapSphere(gameObject.transform.position, 0.5f);

        foreach (Collider c in hits)
        {
            OffMeshLink link;
            if (!c.gameObject.TryGetComponent<OffMeshLink>(out link)) continue;

            link.activated = enabled;
        }

    }
    private void Update()
    {
        if (transform.localRotation == _desiredRotation) return;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, _desiredRotation, Time.deltaTime * 3f);
    }

    private void OnEnable()
    {
        ToggleNearNavLinks(false);
        _desiredRotation = transform.localRotation;
    }

    public List<Command> GetAvailableCommands(Unit unit)
    {
        List<Command> commands = new List<Command>();

        MoveCommand moveToDoor = new MoveCommand(unit, gameObject.transform.position);
        commands.Add(moveToDoor);

        InteractCommand openDoor = new InteractCommand(unit, this, 0.5f);
        commands.Add(openDoor);

        SequencerCommand sequenceCommand = new SequencerCommand(unit,"Open Door", commands);

        return new List<Command> { sequenceCommand };
    }

    public List<Command> GetAvailableCommands(List<Unit> units)
    {
        List<Command> commands =  new List<Command>();
        Unit lead = units[0];


        for (int i = 1; i < units.Count; i++)
        {
            
        }

        return commands;
    }

    public void Interact()
    {
        ToggleNearNavLinks(true);

        _desiredRotation = transform.localRotation * Quaternion.Euler(0, -90f, 0);
    }
}
