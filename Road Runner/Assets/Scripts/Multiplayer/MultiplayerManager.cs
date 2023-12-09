using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance { get; private set;  }
    
    private void Awake()
    {
        Instance = this;
    }

    public void TeleportPlayer(Vector3 position, PlayerStats playerStats)
    {
        TeleportPlayerServerRpc(position, playerStats.NetworkObject);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void TeleportPlayerServerRpc(Vector3 position, NetworkObjectReference playerNetworkObjectReference)
    {
        TeleportPlayerClientRpc(position, playerNetworkObjectReference);
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(Vector3 position, NetworkObjectReference playerNetworkObjectReference)
    {
        playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
        PlayerController playerController = playerNetworkObject.GetComponent<PlayerController>();

        playerController.transform.position = position;
    }
}
