using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshGenerator : MonoBehaviour
{
    [SerializeField]
    bool _enableNavGeneration;

    private void Awake()
    {
        WorldStateManager.Instance.OnWorldStateChanged += GenerateNavMesh;
    }

    private void OnDisable()
    {
        WorldStateManager.Instance.OnWorldStateChanged -= GenerateNavMesh;
    }

    public void GenerateEditorOnly()
    {

        NavMeshSurface navMeshSurface;

        if (gameObject.TryGetComponent<NavMeshSurface>(out navMeshSurface))
        {
            navMeshSurface.BuildNavMesh();
        }

        WorldStateManager.Instance.UpdateWorldState(WorldState.NavMeshGenerated);
    }

    private void GenerateNavMesh(WorldState state)
    {
        if (state != WorldState.NavMeshReady) return;
        if (!_enableNavGeneration) return;

        NavMeshSurface navMeshSurface;

        if (gameObject.TryGetComponent<NavMeshSurface>(out navMeshSurface))
        {
            navMeshSurface.BuildNavMesh();
        }


        WorldStateManager.Instance.UpdateWorldState(WorldState.NavMeshGenerated);
    }
}
