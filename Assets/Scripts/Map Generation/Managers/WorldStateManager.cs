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
        Instance = this;
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
    }

    public void NotifyComplete()
    {
        _subscribersCompleted++;
        Debug.Log("Subscribed Notified Completion. Currently completed: " + _subscribersCompleted);

        //if (_subscribersCompleted == _subscribers) UpdateWorldState(WorldState.NavMeshReady);
    }
}
