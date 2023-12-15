using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using QFSW.QC;

public class EnemyNPCSpawner : NetworkBehaviour
{
    [SerializeField] private EnemyNPC[] enemyPrefabs;

    public enum EnemyType
    {
        EnemyNPC,
        NavMeshEnemyNPC,
    }

    [Command]
    private void SpawnEnemy(EnemyType enemyType, Vector3 positon)
    {
        SpawnEnemyServerRpc(enemyType, positon);
    }

    [Command]
    private void SpawnEnemy(EnemyType enemyType)
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
