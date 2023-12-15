using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using QFSW.QC;

/// <summary>
/// Master class for all interactions with the player
/// </summary>
public class Player : NetworkBehaviour
{
    public static Player LocalPlayerInstance; // Singleton instance of the local player

    private PlayerStats _playerStats;
    private PlayerSpawner _playerSpawner;
    private PlayerFXController _playerFXController;
    // private PlayerMovement playerMovement;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            Destroy(GetComponent<PlayerInput>());
            return;
        }

        if (LocalPlayerInstance == null)
            LocalPlayerInstance = this;

        _playerStats = GetComponent<PlayerStats>();
        _playerSpawner = GetComponent<PlayerSpawner>();
        _playerFXController = GetComponent<PlayerFXController>();
    }

    [Command]
    public void HitWithBullet(float bulletDamage)
    {
        if (IsOwner)
        {
            _playerStats.ChangeHealth(-bulletDamage);
            _playerFXController.PlayHitWithBulletFX();
        }
    }

    [Command]
    public void TakeDamage(float damage)
    {
        if (IsOwner)
        {
            _playerStats.ChangeHealth(-damage);
            _playerFXController.PlayHitWithBulletFX();
        }
    }
}
