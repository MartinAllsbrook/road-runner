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
        Debug.Log(debugTag + "Player Network Spawn");

        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            DestroyLocalOnlyBehaviours();
            return;
        }

        if (LocalPlayerInstance == null)
            LocalPlayerInstance = this;
        else
            Debug.LogError(debugTag + "More than one local player instance seems to exist!");

        InitializeLocalOnlyBehaviors();

        // Get component references
        _playerStats = GetComponent<PlayerStats>();
        _playerSpawner = GetComponent<PlayerSpawner>();
        _playerFXController = GetComponent<PlayerFXController>();

        // Load player data
        CharacterPersistanceManager.Instance.FindAllAndLoad();
    }

    private void DestroyLocalOnlyBehaviours()
    {
        Destroy(GetComponent<PlayerInput>());
        Destroy(GetComponent<WorldInteractor>());
    }

    private void InitializeLocalOnlyBehaviors()
    {
        GetComponent<WorldInteractor>().Initialize();
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
