using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static PlasticGui.LaunchDiffParameters;

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
        return new List<Command> { new OpenDoorSequence(unit, this, gameObject.transform.position) };
    }

    public List<Command> GetAvailableCommands(List<Unit> units)
    {
        return new List<Command>();
    }

    public void Interact()
    {
        ToggleNearNavLinks(true);
        Debug.Log("soy");

        _desiredRotation = transform.localRotation * Quaternion.Euler(0, 90f, 0);
    }
}
