using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemPickup : SpawnedObject
{
    [SerializeField] protected UniqueItemID uniqueItemID;

    private const float despawnTime = 300;

    public UniqueItemID GetUniqueItemID()
    {
        return uniqueItemID;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(DespawnRoutine());
    }

    private IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(despawnTime);
        RemoveFromWorld();
    }

    public void RemoveFromWorld()
    {
        // Do Stuff
        RemoveFromWorldServerRpc(); // TODO: This does not need to be a server rpc because the item will exist on the server and the client
    }
    
    public override void OnNetworkDespawn()
    {
        Destroy(gameObject);
        base.OnNetworkDespawn();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RemoveFromWorldServerRpc()
    {
        if (_freshlySpawned)
        {
            RemoveFromSpawnZone();
        }
        NetworkObject.Despawn(false);
    }
}
