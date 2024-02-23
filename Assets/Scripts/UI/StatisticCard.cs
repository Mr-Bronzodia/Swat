using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatisticCard : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _outcome;
    [SerializeField]
    private TMP_Text _killed;
    [SerializeField]
    private TMP_Text _captured;
    [SerializeField]
    private TMP_Text _rescued;
    [SerializeField]
    private TMP_Text _time;

    public void UpdateStatistics()
    {
        if (GameManager.Instance.IsGameWon) _outcome.text = "Mission Status Success";
        else _outcome.text = "Mission Status Failed";

        _killed.text = "Killed " + GameManager.Instance.DeadUnits;
        _captured.text = "Captured " + GameManager.Instance.CapturedUnits + " combatants";
        _rescued.text = "rescued " + GameManager.Instance.RescuedHostagesCount + " members";
        _time.text = Mathf.RoundToInt(GameManager.Instance.GameTime / 60f) + " min";

    }
}
