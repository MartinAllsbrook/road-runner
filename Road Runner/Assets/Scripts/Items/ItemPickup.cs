using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemPickup : NetworkBehaviour
{
    [SerializeField] protected ItemSO itemSo;

    private const float despawnTime = 300;

    public ItemSO GetScriptableObject()
    {
        return itemSo;
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
        RemoveFromWorldServerRpc();
    }
    
    public override void OnNetworkDespawn()
    {
        gameObject.SetActive(false);
        base.OnNetworkDespawn();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RemoveFromWorldServerRpc()
    {
        NetworkObject.Despawn(false);
    }
}
