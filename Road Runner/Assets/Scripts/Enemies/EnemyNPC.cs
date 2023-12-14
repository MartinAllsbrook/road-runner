using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class EnemyNPC : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;   

    private float _currentHealth;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _currentHealth = maxHealth;
    }

    public void DealDamage(float damage)
    {
        DealDamageServerRpc(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DealDamageServerRpc(float damage)
    {
        DealDamageClientRpc(damage);
    }

    [ClientRpc]
    private void DealDamageClientRpc(float damage)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            RemoveFromWorldServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveFromWorldServerRpc()
    {
        NetworkObject.Despawn(false);
    }

    public override void OnNetworkDespawn()
    {
        gameObject.SetActive(false);
        base.OnNetworkDespawn();
    }
}
