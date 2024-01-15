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

    // String to start debugs with
    private string debugTag = "<color=#0000ffff>[Player] </color>"; // TODO: Make a debug helper class

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
        else
            Debug.LogError(debugTag + "More than one local player instance seems to exist!");

        // Get component references
        _playerStats = GetComponent<PlayerStats>();
        _playerSpawner = GetComponent<PlayerSpawner>();
        _playerFXController = GetComponent<PlayerFXController>();

        // Load player data
        CharacterPersistanceManager.Instance.FindAllAndLoad();
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
