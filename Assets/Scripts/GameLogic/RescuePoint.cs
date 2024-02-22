using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescuePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Unit unit;
        if (!other.TryGetComponent<Unit>(out unit)) return;

        if (!unit.IsHostage) return;

        unit.gameObject.SetActive(false);
        GameManager.Instance.RescuedHostagesCount++;

    }
}
