using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Vector3 _unitSpawnPoint;
    private int _rescuedHostages = 0;
    private Bounds[] _enemyControlledAreas;

    public ETeam PlayerTeam;
    public Action OnHostageRescued;
    public Action OnGameEnd;

    public static GameManager Instance { get; private set; }
    public int DeadUnits { get; set; } = 0;
    public int CapturedUnits { get; set; } = 0;
    public int NoCommandsIssued { get; set; } = 0;
    public float GameTime { get => Time.timeSinceLevelLoad; }
    public bool IsGameWon { get; private set; } = true;
    public float CameraTravelDistance { get;  set; }
    public int NoPause {  get; set; }
    public Vector3 SpawnPoint { get => _unitSpawnPoint; }
    public int HostageCount { get; set; }
    public int RescuedHostagesCount { get => _rescuedHostages; }
    public Bounds[] EnemyAreas { get => _enemyControlledAreas; }

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

    public void SetSpawnPoint(Vector3 spawnPoint)
    {
        _unitSpawnPoint = spawnPoint;
    }

    public void SetEnemyControlledAreas(Bounds[] areas)
    {
        _enemyControlledAreas = areas;
    }

    public void SetFailState()
    {
        IsGameWon = false;
        OnGameEnd?.Invoke();
    }

    public void HostageRescued()
    {
        _rescuedHostages++;
        OnHostageRescued?.Invoke();
        if (HostageCount == RescuedHostagesCount) OnGameEnd?.Invoke();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("DeadUnits: " + DeadUnits);
        Debug.Log("CapturedUnits: " + CapturedUnits);
        Debug.Log("NoCommandsIssued: " + NoCommandsIssued);
        Debug.Log("GameTime: " + GameTime);
        Debug.Log("CameraTravelDistance: " + CameraTravelDistance);
    }

    private void OnDrawGizmos()
    {
        
    }

}
