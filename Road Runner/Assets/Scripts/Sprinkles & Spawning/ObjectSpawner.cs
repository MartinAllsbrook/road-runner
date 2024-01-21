using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static GlobalItemDictionary;

/// <summary>
/// Spawns objects, enemies, and eventually vehicles accoss the network
/// </summary>
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

    #region Item Spawning (from invetory rework)

    public void ItemSpawnRequest(UniqueItemID uniqueItemID, Vector3 position)
    {
        if (IsServer)
            SpawnItem(uniqueItemID, position);
        else
            SpawnItemServerRpc(uniqueItemID, position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemServerRpc(UniqueItemID uniqueItemID, Vector3 position)
    {
        SpawnItem(uniqueItemID, position);
    }

    private void SpawnItem(UniqueItemID uniqueItemID, Vector3 position)
    {
        ItemPickup itemPickupPrefab = ItemSODictionary[uniqueItemID.BaseItemID].ItemPickupPrefab;
        ItemPickup instantiatedItemPickup = Instantiate(itemPickupPrefab, position, Quaternion.identity);

        NetworkObject itemNetworkObject = instantiatedItemPickup.GetComponent<NetworkObject>();
        itemNetworkObject.Spawn(true);

        instantiatedItemPickup.UniqueItemID = uniqueItemID; // OK
    }

    #endregion

    #region General Object Spawning 

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

    #endregion

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
        SpawnEnemyServerRpc(enemyType, Player.LocalInstance.transform.position);
    }

    [Command]
    private void SpawnEnemy(EnemyType enemyType, Vector3 positon)
    {
        SpawnEnemyServerRpc(enemyType, positon);
    }
    #endregion

    #region Spawning Items For Debuging
    [Command]
    public void SpawnItemDebug(int x, int y, int z, ItemID itemEnum)
    {
        Vector3 position = new Vector3(x, y, z);

        if (!IsServer)
        {
            SpawnItemServerRpc(itemEnum, position);
            return;
        }

        SpawnedObject itemSpawnedObject = ItemSODictionary[itemEnum].ItemPickupPrefab.GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }

    [Command]
    public void SpawnItemDebug(ItemID itemEnum)
    {
        Transform playerTransform = Player.LocalInstance.transform;
        Vector3 position = playerTransform.position + playerTransform.forward * 2;

        if (!IsServer)
        {
            SpawnItemServerRpc(itemEnum, position);
            return;
        }

        SpawnedObject itemSpawnedObject = ItemSODictionary[itemEnum].ItemPickupPrefab.GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemServerRpc(ItemID itemEnum, Vector3 position)
    {
        SpawnedObject itemSpawnedObject = ItemSODictionary[itemEnum].ItemPickupPrefab.GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }
    #endregion
}
