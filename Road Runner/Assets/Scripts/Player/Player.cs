using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using QFSW.QC;
using Unity.VisualScripting.Antlr3.Runtime;

/// <summary>
/// Master class for all interactions with the player
/// </summary>
public class Player : NetworkBehaviour
{
    public static Player LocalInstance; // Singleton instance of the local player

    [SerializeField] private GameObject[] playerModel; // Parts of the player model to be hidden for the local player TODO: better name and stuff maybe dedicated class

    [SerializeField] private LayerMask groundLayerMask;

    private Vector3 limboPosition = new Vector3(-1024, -1024, -1024);

    // Pausing
    private int numPauses;
    private bool paused;
    public bool Paused
    {
        get { return paused; }
        private set { }
    }
    /*    private LocalPlayerStats _playerStats;
        private PlayerSpawner _playerSpawner;
        private PlayerFXController _playerFXController;*/

    // Component references
    private PlayerInput playerInput;
    private Rigidbody rigidbodyRef;
    private CameraController cameraController;
    private PlayerClothingVisuals playerClothingVisuals;

    public PlayerClothingVisuals PlayerClothingVisuals
    {
        get { return playerClothingVisuals; }
    }

    // String to start debugs with
    private string debugTag = "<color=#0000ffff>[Player] </color>"; // TODO: Make a debug helper class

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidbodyRef = GetComponent<Rigidbody>();
        cameraController = GetComponent<CameraController>();
        playerClothingVisuals = GetComponent<PlayerClothingVisuals>();

        rigidbodyRef.useGravity = false;
        cameraController.SetLimbo(true);
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(debugTag + "Player Network Spawn");

        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            DestroyLocalOnlyBehaviours();
            return;
        }

        if (LocalInstance == null)
            LocalInstance = this;
        else
            Debug.LogError(debugTag + "More than one local player instance seems to exist!");

        InitializeLocalOnlyBehaviors();
        HideLocalPlayerModel();

        /*// Get component references
        _playerSpawner = GetComponent<PlayerSpawner>();
        _playerFXController = GetComponent<PlayerFXController>();

        // Store singleton references
        _playerStats = LocalPlayerStats.Instance; // TODO: This class might not need access to the player stats singleton*/
    }

    private void HideLocalPlayerModel()
    {
        // TODO: Make hidden layer mask
        gameObject.layer = 6;

        foreach (var modelObject in playerModel)
            modelObject.layer = 6;
    }

    public void SpawnPlayer()
    {
        Vector3 position;

        if (SprinkleGenerator.Instance == null)
        {
            Debug.LogWarning("Sprinkle generator is null");
            position = new Vector3(16, 16, 16);
            TeleportPlayerServerRpc(position);
            return;
        }

        position = SprinkleGenerator.Instance.GetSpawnPoint();
        Ray ray = new Ray(position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 120, groundLayerMask))
        {
            position.y = hitInfo.point.y;
            TeleportPlayerServerRpc(position);
            return;
        }
    }

    #region Limbo Control
    [Command]
    public void EnterLimbo()
    {
        if (!IsOwner)
            return;

        cameraController.SetLimbo(true);
        GetComponent<Rigidbody>().useGravity = false;
        UIManager.Instance.EnterLimbo();

        Pause();

        TeleportPlayerServerRpc(limboPosition);
    }

    [Command]
    public void ExitLimbo() // The only way to exit limbo is the limbo UI and doing so spawns the player
    {
        cameraController.SetLimbo(false);
        GetComponent<Rigidbody>().useGravity = true;
        UIManager.Instance.ExitLimbo();

        LocalPlayerStats.Instance.Spawn(); // TODO: Rename

        SpawnPlayer();

        Unpause();
    }
    #endregion

    #region Pause / Unpause
    public void Unpause()
    {
        numPauses--;

        if (numPauses > 0)
            return;

        paused = false;

        playerInput.currentActionMap = playerInput.actions.FindActionMap("Player");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        numPauses++;

        paused = true;

        playerInput.currentActionMap = playerInput.actions.FindActionMap("Paused");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #region Network Commands
    [ServerRpc(RequireOwnership = false)]
    private void TeleportPlayerServerRpc(Vector3 position)
    {
        TeleportPlayerClientRpc(position);
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(Vector3 position)
    {
        rigidbodyRef.position = position + Vector3.up;
    }
    #endregion

    #region QC Commands
    [Command("Pause", MonoTargetType.All)]
    private void PauseCommand()
    {
        if (IsOwner)
            Pause();
    }

    [Command("Unpause", MonoTargetType.All)]
    private void UnpauseCommand()
    {
        if (IsOwner)
            Unpause();
    }

    [Command("Respawn", MonoTargetType.All)]
    private void RespawnCommand()
    {
        if (IsOwner)
            SpawnPlayer();
    }
    #endregion

    #region Local Only Behaviour Control
    private void DestroyLocalOnlyBehaviours()
    {
        Debug.Log(debugTag + "Destroying local only behaviours");
        Destroy(GetComponent<PlayerInput>());
        Destroy(GetComponent<WorldInteractor>());
        Destroy(GetComponent<CameraController>());
    }

    private void InitializeLocalOnlyBehaviors()
    {
        GetComponent<WorldInteractor>().Initialize();
        GetComponent<CameraController>().Initialize();

    }
    #endregion
}
