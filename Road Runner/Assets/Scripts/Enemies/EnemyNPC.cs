using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class EnemyNPC : NetworkBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Enemy References")]
    [SerializeField] private CustomEffect deathEffects;

    private float _currentHealth;
    private bool _alive = true;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _currentHealth = maxHealth;
    }

    public void DealDamage(float damage)
    {
        if (!_alive)
            return;

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
            Die();
        }
    }

    private void Die()
    {
        _alive = false;

        CustomEffect deathEffect = Instantiate(deathEffects, transform.position, Quaternion.identity);
        deathEffect.PlayEffects();

        if (IsOwner)
            RemoveFromWorldServerRpc();
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
        Destroy(gameObject);
    }
}
