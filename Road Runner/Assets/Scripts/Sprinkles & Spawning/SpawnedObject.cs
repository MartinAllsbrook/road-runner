using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnedObject : NetworkBehaviour
{
    private SpawnZone _parentSpawnZone;

    protected bool _freshlySpawned = false;
    protected bool _removed = false;

    public virtual void Spawn(SpawnZone parentSpawnZone)
    {
        _freshlySpawned = true;
        _removed = false;
        _parentSpawnZone = parentSpawnZone;
    }

    protected virtual void RemoveFromSpawnZone()
    {
        if (_removed)
        { 
            Debug.LogWarning("Trying to remove object that has already been removed");
            return;
        }
        if (!_freshlySpawned)
        {
            Debug.LogWarning("Trying to remove object is not freshly spawned");
            return;
        }

        _parentSpawnZone.RemoveSpawnedObject();
        _removed = true;
    }
}
