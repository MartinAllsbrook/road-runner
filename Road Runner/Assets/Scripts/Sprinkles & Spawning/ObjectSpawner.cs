using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Inventory;

public class ObjectSpawner : NetworkBehaviour
{
    public static ObjectSpawner Instance;

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
            enabled = false;
            return;
        }
    }

    // if we are the server, we can skip the server rpc and just spawn the thing
    public void SpawnObject(SpawnedObject objectToSpawn, Vector3 position, SpawnZone parentSpawnZone, Vector3 sprinkleCenter, float sprinkleRadius)
    {
        SpawnedObject spawnedObject = Instantiate(objectToSpawn, position, Quaternion.identity);
        spawnedObject.Spawn(parentSpawnZone, sprinkleCenter, sprinkleRadius);

        NetworkObject spawnedObjectNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        spawnedObjectNetworkObject.Spawn(true);
    }

    // if we are the server, we can skip the server rpc and just spawn the thing
    public void SpawnObject(SpawnedObject objectToSpawn, Vector3 position)
    {
        SpawnedObject spawnedObject = Instantiate(objectToSpawn, position, Quaternion.identity);

        NetworkObject spawnedObjectNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        spawnedObjectNetworkObject.Spawn(true);
    }

    #region Spawning Enemies For Debugging
    [Header("Debugging")]
    [SerializeField] private SpawnedObject[] enemySpawnedObjectPrefabs;

    public enum EnemyType
    {
        EnemyNPC,
        NavMeshEnemyNPC,
        HostileEnemyNPC
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc(EnemyType enemyType, Vector3 position)
    {
        SpawnedObject spawnedEnemy = enemySpawnedObjectPrefabs[(int)enemyType];

        SpawnObject(spawnedEnemy, position);
    }

    [Command("SpawnEnemyHere")]
    private void SpawnEnemyDebug(EnemyType enemyType)
    {
        SpawnEnemyServerRpc(enemyType, Player.LocalPlayerInstance.transform.position);
    }

    [Command]
    private void SpawnEnemy(EnemyType enemyType, Vector3 positon)
    {
        SpawnEnemyServerRpc(enemyType, positon);
    }
    #endregion
}
