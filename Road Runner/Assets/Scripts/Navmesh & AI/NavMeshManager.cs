using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using QFSW.QC;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Command]    
    public void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    public void OnDestroy()
    {
        Destroy(navMeshSurface);
    }
}
