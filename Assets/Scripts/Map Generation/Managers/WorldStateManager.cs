using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class WorldStateManager : MonoBehaviour
{
    private EWorldState _worldState;
    private int _subscribers = 0;
    private int _subscribersCompleted = 0;

    [SerializeField]
    private GameObject[] _playerUnitsPrefabs;
    [SerializeField]
    private GameObject _playerCarPrefab;

    public static WorldStateManager Instance;
    public Action<EWorldState> OnWorldStateChanged;

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

    private void OnEnable()
    {
        Assert.IsNotNull(_playerCarPrefab, "Player Car prefab not assigned in WorldStateManager");
        Assert.IsNotNull(_playerUnitsPrefabs, "Player unit prefab not assigned in WorldStateManager");
    }

    public void UpdateWorldState(EWorldState newWorldState)
    {
        _worldState = newWorldState;
        DebugUiManager.Instance.AddDebugText(3, "World State: " + _worldState.ToString());
        OnWorldStateChanged?.Invoke(newWorldState);
    }


    public void AddSubscriber()
    {
        _subscribers++;
        DebugUiManager.Instance.AddDebugText(1, "Subscriber Count: " + _subscribers);
    }

    public void ResetSubscribers()
    {
        _subscribers = 0;
        _subscribersCompleted = 0;

        DebugUiManager.Instance.AddDebugText(1, "Subscriber Count: " + _subscribers);
        DebugUiManager.Instance.AddDebugText(2, "Subscriber Completed: " + _subscribersCompleted);
        DebugUiManager.Instance.AddDebugText(3, "World State: " + _worldState.ToString());
    }

    public void SpawnPlayerUnits()
    {
        Vector3 spawnPoint = GameManager.Instance.SpawnPoint;

        Instantiate(_playerCarPrefab, spawnPoint, Quaternion.identity);

        Vector3 spawnDirection = new Vector3(1, 0, 0);

        float spawnOffset = 3.5f;
        foreach (GameObject unit in _playerUnitsPrefabs)
        {
            Instantiate(unit, spawnPoint + (spawnOffset * spawnDirection), Quaternion.identity);
            spawnOffset++;
        }
    }

    public void NotifyComplete()
    {
        _subscribersCompleted++;
        DebugUiManager.Instance.AddDebugText(2, "Subscriber Completed: " + _subscribersCompleted);

        if (_subscribersCompleted == _subscribers && _worldState == EWorldState.PlotsGenerated) UpdateWorldState(EWorldState.ReadyToGenerateNavMesh);

        if (_worldState == EWorldState.NavMeshGenerated) SpawnPlayerUnits();
    }
}
