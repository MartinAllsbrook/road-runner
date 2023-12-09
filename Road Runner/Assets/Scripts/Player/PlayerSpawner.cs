using QFSW.QC.Utilities;
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

    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

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

        if (!IsOwner)
            return;

        if (localPlayerSpawner == null)
            localPlayerSpawner = this;

        gameObject.layer = 6;

        foreach (var modelObject in playerModel)
            modelObject.layer = 6;

        FreezePlayer();
    }

/*    private void Update()
    {
        if (!IsOwner)
            return;

    }*/

    public void FreezePlayer()
    {
        Time.timeScale = 0;
        _rigidbody.useGravity = false;
    }

    public void UnfreezePlayer()
    {
        Time.timeScale = 1;
        _rigidbody.useGravity = true;
        Unpause();
        Invoke("SpawnPlayer", 3);
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
        Vector3 position = SprinkleGenerator.Instance.GetSpawnPoint();
        Ray ray = new Ray(position, Vector3.down);

        if(Physics.Raycast(ray, out RaycastHit hitInfo, 120, layerMask))
        {
            position.y = hitInfo.point.y;
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
