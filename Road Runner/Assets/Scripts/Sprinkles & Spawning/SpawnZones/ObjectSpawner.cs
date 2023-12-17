using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Inventory;

public class ObjectSpawner : NetworkBehaviour
{
    public static ObjectSpawner Instance;

    public enum ObjectType
    {
        Item,
        EnemyNPC,
        Vehicle
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            Debug.LogWarning("Disableing Object Spawner on Client");
            return;
        }
    }

    // if we are the server, we can skip the server rpc and just spawn the thing
    public void SpawnObject(SpawnedObject objectToSpawn, Vector3 position, SpawnZone parentSpawnZone)
    {
        SpawnedObject spawnedObject = Instantiate(objectToSpawn, position, Quaternion.identity);
        spawnedObject.Spawn(parentSpawnZone);

        NetworkObject spawnedObjectNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        spawnedObjectNetworkObject.Spawn(true);
    }
}
