using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletNetworkManager : NetworkBehaviour
{
    public static BulletNetworkManager Instance;

    [SerializeField] private EffectPool bulletHolePool;
    [SerializeField] private EffectPool bulletCrackPool;
    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    #region Bullet Hit Player
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
    #endregion

    #region Bullet Hit Environment
    public void BulletHitEnvironment(Vector3 point, Vector3 normal)
    {
        SpawnBulletHoleServerRpc(point, normal);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletHoleServerRpc(Vector3 point, Vector3 normal)
    {
        SpawnBulletHoleClientRpc(point, normal);
    }
    
    [ClientRpc]
    private void SpawnBulletHoleClientRpc(Vector3 point, Vector3 normal)
    {
        Quaternion holeDirection = Quaternion.LookRotation(normal);

        bulletHolePool.PlaceEffect(point, holeDirection);
    }
    #endregion

    #region Bullet Causes Bullet Crack
    public void SpawnBulletCrack(Vector3 point)
    {
        SpawnBulletCrackServerRpc(point);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletCrackServerRpc(Vector3 point)
    {
        SpawnBulletCrackClientRpc(point);
    }
    
    [ClientRpc]
    private void SpawnBulletCrackClientRpc(Vector3 point)
    {
        bulletCrackPool.PlaceEffect(point, Quaternion.identity);
    }
    #endregion
}
