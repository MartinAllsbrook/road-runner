using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostileEnemyNPC : NavMeshEnemyNPC
{
    [Header("Hostile Enemy Stats")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 16f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Hostile Enemy References")]
    [SerializeField] private CustomEffect attackEffects;

    protected float _nextAttackTime = 0f;

    protected override void Update()
    {
        base.Update();

        if (CanAttackLocalPlayer())
        {
            TryAttackLocalPlayer();
        }
    }

    protected bool CanAttackLocalPlayer()
    {
        if (_canSeeLocalPlayer)
        {
            float distanceToPlayer = GetVectorToLocalPlayer().magnitude;

            if (distanceToPlayer < attackRange)
            {
                return true;
            }
        }

        return false;
    }

    protected void TryAttackLocalPlayer()
    {
        if (Time.time > _nextAttackTime)
        {
            AttackLocalPlayer();
        }
    }

    protected void AttackLocalPlayer()
    {
        _nextAttackTime = Time.time + attackCooldown;
        LocalPlayerStats.Instance.DealDamage(LocalPlayerStats.BodyArea.Global, damage, true); // TODO: BodyArea.Global is a placeholder

        PlayAttackEffectsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayAttackEffectsServerRpc()
    {
        PlayAttackEffectsClientRpc();
    }

    [ClientRpc]
    private void PlayAttackEffectsClientRpc()
    {
        attackEffects.PlayEffects();
    }
}
