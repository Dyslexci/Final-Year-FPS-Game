using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// Bakes NavMeshes on a given NavMeshSurface - probably deprecated due to use of prefabs
/// </summary>
public class NavMeshBaker : MonoBehaviour
{
    public void BakeNavMesh(NavMeshSurface originSurface)
    {
        originSurface.BuildNavMesh();
    }
}
