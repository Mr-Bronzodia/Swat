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

        WorldStateManager.Instance.UpdateWorldState(EWorldState.NavMeshGenerated);
    }

    private void GenerateNavMesh(EWorldState state)
    {
        if (state != EWorldState.ReadyToGenerateNavMesh) return;
        if (!_enableNavGeneration) return;

        NavMeshSurface navMeshSurface;

        if (gameObject.TryGetComponent<NavMeshSurface>(out navMeshSurface))
        {
            navMeshSurface.BuildNavMesh();
        }


        WorldStateManager.Instance.UpdateWorldState(EWorldState.NavMeshGenerated);
    }
}
