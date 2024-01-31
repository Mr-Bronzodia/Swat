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
        foreach (Transform child in gameObject.transform)
        {
            NavMeshSurface navMeshSurface;

            if (child.TryGetComponent<NavMeshSurface>(out navMeshSurface))
            {
                navMeshSurface.BuildNavMesh();
            }
        }

        WorldStateManager.Instance.UpdateWorldState(WorldState.NavMeshGenerated);
    }

    private void GenerateNavMesh(WorldState state)
    {
        if (state != WorldState.NavMeshReady) return;
        if (!_enableNavGeneration) return;

        foreach (Transform child in gameObject.transform)
        {
            NavMeshSurface navMeshSurface;

            if (child.TryGetComponent<NavMeshSurface>(out navMeshSurface))
            {
                navMeshSurface.BuildNavMesh();
            }
        }

        WorldStateManager.Instance.UpdateWorldState(WorldState.NavMeshGenerated);
    }
}
