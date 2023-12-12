using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerSpawner : NetworkBehaviour
{
    public static PlayerSpawner localPlayerSpawner;

    [SerializeField] private GameObject[] playerModel;

    [SerializeField] private TerrainManager terrainManager;

    [SerializeField] private LayerMask layerMask;

    private CameraController _cameraController;

    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    private Vector3 _limboPosition;

    private int numPauses;
    private bool paused;
    public bool Paused
    {
        get { return paused; } 
        private set { }
    }
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _cameraController = GetComponent<CameraController>();

        if (!IsOwner)
            return;

        if (localPlayerSpawner == null)
            localPlayerSpawner = this;

        gameObject.layer = 6;

        foreach (var modelObject in playerModel)
            modelObject.layer = 6;

        _rigidbody.useGravity = false;
        _cameraController.SetLimbo(true);
        Pause(); //???
    }

    [Command]
    public void EnterLimbo()
    {
        _cameraController.SetLimbo(true);
        _rigidbody.useGravity = false;
        UIManager.Instance.EnterLimbo();

        Pause();

        transform.position = _limboPosition;
    }

    [Command]
    public void ExitLimbo() // The only way to exit limbo is the limbo UI and doing so spawns the player
    {
        _cameraController.SetLimbo(false);
        _rigidbody.useGravity = true;
        UIManager.Instance.ExitLimbo();

        Unpause();

        SpawnPlayer();
    }

    public void Unpause()
    {
        numPauses--;

        if (numPauses > 0)
            return;

        paused = false;

        _playerInput.currentActionMap = _playerInput.actions.FindActionMap("Player");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        numPauses++;

        paused = true;

        _playerInput.currentActionMap = _playerInput.actions.FindActionMap("Paused");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SpawnPlayer()
    {
        Vector3 position;

        if (SprinkleGenerator.Instance == null)
        {
            Debug.LogWarning("Sprinkle generator is null");
             position = new Vector3(32, 100, 32);
            TeleportPlayerServerRpc(position);
            return;
        }
            
        position = SprinkleGenerator.Instance.GetSpawnPoint();
        Ray ray = new Ray(position, Vector3.down);

        if(Physics.Raycast(ray, out RaycastHit hitInfo, 120, layerMask))
        {
            position.y = hitInfo.point.y;
            transform.position = position; // Just in case the server doesn't do it (trial)
            TeleportPlayerServerRpc(position);
            return;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TeleportPlayerServerRpc(Vector3 position)
    {
        TeleportPlayerClientRpc(position);
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(Vector3 position)
    {
        transform.position = position;
    }
}
