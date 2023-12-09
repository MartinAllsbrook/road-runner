using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletNetworkManager : NetworkBehaviour
{
    public static BulletNetworkManager Instance;

    [SerializeField] private Transform bulletHole;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }
    
    // =====================================================
    // Bullet Hit Player Bullet Hit Player Bullet Hit Player
    // =====================================================
    public void BulletHitPlayer(NetworkObject playerStatsNetworkObject, float damage)
    {
        BulletHitPlayerServerRpc(playerStatsNetworkObject, damage);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void BulletHitPlayerServerRpc(NetworkObjectReference playerStatsNetworkObjectReference, float damage)
    {
        BulletHitPlayerClientRpc(playerStatsNetworkObjectReference, damage);
    }
    
    [ClientRpc]
    private void BulletHitPlayerClientRpc(NetworkObjectReference playerStatsNetworkObjectReference, float damage)
    {
        // Getting player that got hit
        playerStatsNetworkObjectReference.TryGet(out NetworkObject playerStatsNetworkObject);
        PlayerStats playerStats = playerStatsNetworkObject.GetComponent<PlayerStats>();
        playerStats.ChangeHealth(-damage);
    }

    // ====================================================================
    // Bullet Hit Environment Bullet Hit Environment Bullet Hit Environment
    // ====================================================================
    public void BulletHitEnvironment(Vector3 point, Vector3 normal)
    {
        SpawnBulletHoleServerRpc(point, normal);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SpawnBulletHoleServerRpc(Vector3 point, Vector3 normal)
    {
        SpawnBulletHoleClientRpc(point, normal);
    }
    
    [ClientRpc]
    public void SpawnBulletHoleClientRpc(Vector3 point, Vector3 normal)
    {
        Quaternion holeDirection = Quaternion.LookRotation(normal);

        BulletHolePool.Instance.PlaceBulletHole(point, holeDirection);
        // Instantiate(bulletHole, point, holeDirection);
    }
}
