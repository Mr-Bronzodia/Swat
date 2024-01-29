using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    private WorldState _worldState;
    private int _subscribers = 0;
    private int _subscribersCompleted = 0;

    public static WorldStateManager Instance;
    public Action<WorldState> OnWorldStateChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateWorldState(WorldState newWorldState)
    {
        _worldState = newWorldState;
        Debug.Log("World State Updated to : " +  _worldState);
        OnWorldStateChanged?.Invoke(newWorldState);
    }


    public void AddSubscriber()
    {
        _subscribers++;
        Debug.Log("Added subscriber. Subscriber Count: " + _subscribers);
        DebugUiManager.Instance.AddDebugText(1, "Subscriber Count: " + _subscribers);
    }

    public void ResetSubscribers()
    {
        _subscribers = 0;
        _subscribersCompleted = 0;

        DebugUiManager.Instance.AddDebugText(1, "Subscriber Count: " + _subscribers);
        DebugUiManager.Instance.AddDebugText(2, "Subscriber Completed: " + _subscribersCompleted);
    }

    public void NotifyComplete()
    {
        _subscribersCompleted++;
        Debug.Log("Subscribed Notified Completion. Currently completed: " + _subscribersCompleted);

        DebugUiManager.Instance.AddDebugText(2, "Subscriber Completed: " + _subscribersCompleted);
        if (_subscribersCompleted == _subscribers && _worldState == WorldState.PlotsGenerated) UpdateWorldState(WorldState.NavMeshReady);
    }
}
