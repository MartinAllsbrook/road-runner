using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemPickup : SpawnedObject
{
    [SerializeField] protected Inventory.ItemID baseItemID;
    [SerializeField] protected UniqueItemModel uniqueItemModel;

    private const float despawnTime = 300;

    protected UniqueItemID _uniqueItemID;
    public UniqueItemID UniqueItemID
    {
        get 
        {
            if (_uniqueItemID == null)
                _uniqueItemID = new UniqueItemID(baseItemID);
            
            return _uniqueItemID;
        }

        set 
        { 
            SetUniqueItemIDServerRpc(value); 
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetUniqueItemIDServerRpc(UniqueItemID uniqueItemID)
    {
        SetUniqueItemIDClientRpc(uniqueItemID);
    }

    [ClientRpc]
    private void SetUniqueItemIDClientRpc(UniqueItemID uniqueItemID)
    {
        _uniqueItemID = uniqueItemID;
        uniqueItemModel.BuildModel(_uniqueItemID);
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
