using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using QFSW.QC;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.UIElements;

public class EnemyNPCSpawner : NetworkBehaviour
{
    [SerializeField] private EnemyNPC[] enemyPrefabs;

    [SerializeField] private float enenmySpawnTime = 15f;

    public enum EnemyType
    {
        EnemyNPC,
        NavMeshEnemyNPC,
        HostileEnemyNPC
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            //Debug.LogWarning("Disableing Enemy Spawner on Client");
            return;
        }

        TerrainManager.onTerrainGenerated.AddListener(() => 
        {
            StartCoroutine(SpawnEnemyRoutine());
        });
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(enenmySpawnTime);

            SpawnEnemy(EnemyType.HostileEnemyNPC);
        }
    }



    private void SpawnEnemy(EnemyType enemyType)
    {
        Vector3 position = SprinkleGenerator.Instance.GetPointInSprinkleOnNavmesh();

        EnemyNPC spawnedEnemy = Instantiate(enemyPrefabs[(int)enemyType], position, Quaternion.identity);

        NetworkObject spawnedEnemyNetworkObject = spawnedEnemy.GetComponent<NetworkObject>();
        spawnedEnemyNetworkObject.Spawn(true);

        //SpawnEnemyServerRpc(enemyType, position);
        Debug.Log("Spawned Enemy at: " + position);
    }

    #region Stuff for Clients, needed for QC & debugging

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc(EnemyType enemyType, Vector3 position)
    {
        EnemyNPC spawnedEnemy = Instantiate(enemyPrefabs[(int) enemyType], position, Quaternion.identity);
        
        NetworkObject spawnedEnemyNetworkObject = spawnedEnemy.GetComponent<NetworkObject>();
        spawnedEnemyNetworkObject.Spawn(true);
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
