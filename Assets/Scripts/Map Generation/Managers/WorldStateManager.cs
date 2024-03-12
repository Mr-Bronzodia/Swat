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
    [SerializeField]
    private GameObject[] _enemyUnitPrefabs;
    [SerializeField]
    private GameObject[] _hostagePrefabs;

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

    private void SpawnPlayerUnits()
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

     private void SpawnHostages()
     {
        int upperBound = 0;
        int lowerBound = 0; 

        //normal
        if (SettingsManager.Instance.Settings.Difficulty == 1)
        {
            upperBound = 2;
            lowerBound = 1;
        }
        //hard
        else
        {
            upperBound = 5;
            lowerBound = 2;
        }


        int hostagesCount = Random.Range(lowerBound, upperBound);
        Bounds[] controlledAreas = GameManager.Instance.EnemyAreas;

        for (int i = 0; i < hostagesCount; i++)
        {
            Vector3 spawnPosition = RandomPointInBounds(controlledAreas[Random.Range(0, controlledAreas.Length - 1)]);
            GameObject prefab = _hostagePrefabs[Random.Range(0, _hostagePrefabs.Length - 1)];
            GameObject hostage = Instantiate(prefab, spawnPosition, Quaternion.identity);
        }

     }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private void SpawnEnemyUnits()
    {
        int upperBound = 0;
        int lowerBound = 0;

        //normal
        if (SettingsManager.Instance.Settings.Difficulty == 1)
        {
            upperBound = 3;
            lowerBound = 2;
        }
        //hard
        else
        {
            upperBound = 5;
            lowerBound = 3;
        }

        int enemyCount = Random.Range(lowerBound, upperBound);
        Bounds[] controlledAreas = GameManager.Instance.EnemyAreas;

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = RandomPointInBounds(controlledAreas[Random.Range(0, controlledAreas.Length - 1)]);
            GameObject prefab = _enemyUnitPrefabs[Random.Range(0, _enemyUnitPrefabs.Length - 1)];
            GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }

    public void NotifyComplete()
    {
        _subscribersCompleted++;
        DebugUiManager.Instance.AddDebugText(2, "Subscriber Completed: " + _subscribersCompleted);

        if (_subscribersCompleted == _subscribers && _worldState == EWorldState.PlotsGenerated) UpdateWorldState(EWorldState.ReadyToGenerateNavMesh);

        if (_worldState == EWorldState.NavMeshGenerated)
        {
            SpawnPlayerUnits();
            SpawnEnemyUnits();
            SpawnHostages();
        }
    }
}
