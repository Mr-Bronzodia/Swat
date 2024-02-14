using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public ETeam PlayerTeam;

    public static GameManager Instance { get; private set; }
    public int DeadUnits { get; set; } = 0;
    public int CapturedUnits { get; set; } = 0;
    public int NoCommandsIssued { get; set; } = 0;
    public float GameTime { get => Time.timeSinceLevelLoad; }
    public bool IsGameWon { get; private set; } = false;
    public float CameraTravelDistance { get;  set; }
    public int NoPause {  get; set; }

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

    private void OnApplicationQuit()
    {
        Debug.Log("DeadUnits: " + DeadUnits);
        Debug.Log("CapturedUnits: " + CapturedUnits);
        Debug.Log("NoCommandsIssued: " + NoCommandsIssued);
        Debug.Log("GameTime: " + GameTime);
        Debug.Log("CameraTravelDistance: " + CameraTravelDistance);
    }

}
