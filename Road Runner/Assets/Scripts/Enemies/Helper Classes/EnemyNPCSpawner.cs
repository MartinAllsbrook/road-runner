using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using QFSW.QC;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class EnemyNPCSpawner : NetworkBehaviour
{
    [SerializeField] private EnemyNPC[] enemyPrefabs;

    public enum EnemyType
    {
        EnemyNPC,
        NavMeshEnemyNPC,
        HostileEnemyNPC
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            TerrainManager.onTerrainGenerated.AddListener(() => 
            {
                StartCoroutine(SpawnEnemyRoutine());
            });
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            SpawnEnemy(EnemyType.HostileEnemyNPC);
        }
    }

    [Command]
    private void SpawnEnemy(EnemyType enemyType, Vector3 positon)
    {
        SpawnEnemyServerRpc(enemyType, positon);
    }

    [Command]
    private void SpawnEnemy(EnemyType enemyType)
    {
        Vector3 position = SprinkleGenerator.Instance.GetPointInSprinkleOnNavmesh();
        
        SpawnEnemyServerRpc(enemyType, position);
        Debug.Log("Spawned Enemy at: " + position);
    }

    [Command("SpawnEnemyHere")]
    private void SpawnEnemyDebug(EnemyType enemyType)
    {
        SpawnEnemyServerRpc(enemyType, Player.LocalPlayerInstance.transform.position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc(EnemyType enemyType, Vector3 positon)
    {
        EnemyNPC spawnedEnemy = Instantiate(enemyPrefabs[(int) enemyType], positon, Quaternion.identity);
        
        NetworkObject spawnedEnemyNetworkObject = spawnedEnemy.GetComponent<NetworkObject>();
        spawnedEnemyNetworkObject.Spawn(true);
    }
}
