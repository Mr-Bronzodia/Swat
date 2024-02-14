using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWorldState 
{
    Empty,
    MapGenerated,
    PlotsGenerated,
    HousesGenerated,

    ReadyToGenerateNavMesh,
    NavMeshGenerated,
    Ready
}
