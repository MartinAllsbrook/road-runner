using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class UseableItemController : NetworkBehaviour 
{
    public static UseableItemController Instance;

    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform handPosition;
    [SerializeField] private ItemSO fists;

    [Header("Inputs")]
    [SerializeField] private KeyCode useKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode seccondaryUseKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    private bool _useInput;
    private bool _seccondaryInput;
    private bool _reloadInput;

    private ItemSO currentItemSo;
    private GameObject currentItemPrefab;
    private UseableItem currentUseableItem;

    private HUDController hudController;
    public HUDController HudController
    {
        get { return hudController; }
        private set { }
    }

    public Transform CameraPosition
    {
        get { return cameraPosition; }
        private set { }
    }

    private void Start()
    {
        SetItem(fists);

        if (!IsOwner)
            return;

        if (Instance == null)
            Instance = this;

        hudController = GameObject.Find("HUD").GetComponent<HUDController>();

        // magazine = new Magazine(gunSo.magSize);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        //timeSinceLastShot += Time.deltaTime;

        if (_useInput)
            currentUseableItem.UseItem();

        if (_seccondaryInput)
            currentUseableItem.SeccondaryUseItem();

        if (_reloadInput)
            currentUseableItem.ReloadItem();
    }

    public void SetUseInput(InputAction.CallbackContext context)
    {
        _useInput = context.action.IsPressed();
    }
    public void SetSeccondaryUseInput(InputAction.CallbackContext context)
    {
        _seccondaryInput = context.action.IsPressed();
    }

    public void SetReloadInput(InputAction.CallbackContext context)
    {
        _reloadInput = context.action.IsPressed();
    }

    public void SetItem(ItemSO itemSO)
    {
        Destroy(currentItemPrefab);
        currentItemSo = itemSO;
        currentItemPrefab =  Instantiate(itemSO.GetItemPrefab(), handPosition.position, handPosition.rotation, handPosition);
        currentUseableItem = currentItemPrefab.GetComponent<UseableItem>();
        currentUseableItem.IsOwner = IsOwner;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UseServerRpc()
    {
        UseClientRpc();
    }

    [ClientRpc]
    public void UseClientRpc()
    {
/*        if (IsOwner)
            return;*/

        currentUseableItem.UseServerAction();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SeccondaryUseServerRpc()
    {
        SeccondaryUseClientRpc();
    }

    [ClientRpc]
    public void SeccondaryUseClientRpc()
    {
        if (IsOwner)
            return;

        currentUseableItem.SeccondaryUseServerAction();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReloadServerRpc()
    {
        ReloadClientRpc();
    }

    [ClientRpc]
    public void ReloadClientRpc()
    {
/*        if(IsOwner) 
            return;*/

        currentUseableItem.ReloadServerAction();
    }
}
