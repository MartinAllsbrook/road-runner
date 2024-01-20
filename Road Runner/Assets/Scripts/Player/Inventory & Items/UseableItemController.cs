using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using QFSW.QC;
using static GlobalItemDictionary;
using System;

/// <summary>
/// Sets the current item and controls it's inputs and server actions
/// 
/// 1. Gets input from the player and passes it to the current item
/// 2. Allows UseableItems to perform actions accross the server without having to be network objects
/// </summary>
public class UseableItemController : NetworkBehaviour 
{
    public static UseableItemController Instance;

    [Header("References")]
    [SerializeField] private CameraController cameraController;

    [Header("Basics")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform handTransform;

    private readonly string debugTag = LogColors.GetColoredTag("[UseableItemController]", LogColors.PlayerColor);

    #region Hand Positions

    [Header("HandTransforms")]
    [Tooltip("Make sure this matches the order of the enums")] 
    [SerializeField] private Transform[] handPositionTransforms;

    private HandPosition _handPosition = HandPosition.Resting;

    public enum HandPosition
    {
        Resting,
        Inspecting,
        Aim,
        Reloading,
    }

    #endregion

    private bool _useInput = false;
    private bool _seccondaryInput = false;
    private bool _reloadInput = false;

    private bool _inspecting = false;

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
        SetItem();

        if (!IsOwner)
            return;

        if (Instance == null)
            Instance = this;

        Inventory.Instance.HoldItem(new StoredItemID());

        hudController = HUDController.Instance;
    }

    /// <summary>
    /// Passes input to the current item
    /// </summary>
    private void Update()
    {
        if (!IsOwner)
            return;

        if (_inspecting) // replace with a state machine sorta thing
            return;

        if (_useInput)
            currentUseableItem.OnUseItemInput();

        if (_reloadInput)
            currentUseableItem.OnReloadItemInput();
    }

    public void SetHandPosition(HandPosition handPosition)
    {
        Transform handPositionTransform = handPositionTransforms[(int)handPosition];

        _handPosition = handPosition; 

        handTransform.position = handPositionTransform.position;
        handTransform.rotation = handPositionTransform.rotation;
    }

    private void OnStartInspect()
    {
        cameraController.CameraLocked = true;

        SetHandPosition(HandPosition.Inspecting);

        hudController.StartInspectItem(currentUseableItem);
    }

    private void OnStopInspect()
    {
        cameraController.CameraLocked = false;
        
        SetHandPosition(HandPosition.Resting);

        hudController.StopInspectItem();
    }

    #region Hotbar => Holding Item
    public void OnHotbarKeyPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            string keyName = context.control.name;
            int hotbarSlotIndex = Int32.Parse(keyName) - 1;

            StoredItemID storedItemID = Inventory.Instance.GetNextItemAtHotbarSlot(hotbarSlotIndex);

            Debug.Log(debugTag + "Holding item with HotbarSlot index: " + hotbarSlotIndex + ", Item: " + storedItemID);

            Inventory.Instance.HoldItem(storedItemID); // This is going to go to the inventory and back here for now
        }
    }

    public void HoldItem(StoredItemID storedItemID)
    {
        HoldItemServerRpc(storedItemID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HoldItemServerRpc(StoredItemID storedItemID)
    {
        HoldItemClientRpc(storedItemID);
    }

    [ClientRpc]
    private void HoldItemClientRpc(StoredItemID storedItemID)
    {
        SetItem(storedItemID);
    }

    // Sets the current item, within the scope of this class
    public void SetItem(StoredItemID storedItemID)
    {
        ItemSO itemSO = ItemSODictionary[storedItemID.UniqueItemID.BaseItemID];

        Destroy(currentUseableItem.gameObject);
        currentUseableItem = Instantiate(itemSO.UsableItemPrefab, handTransform.position, handTransform.rotation, handTransform); 

        currentUseableItem.SetUniqueItemID(storedItemID);
        currentUseableItem.ParentItemController = this;
        currentUseableItem.IsOwner = IsOwner;
        currentUseableItem.BuildModel();
    }

    // Sets to empty at start idk if we need this but yuh
    public void SetItem()
    {
        ItemSO itemSO = ItemSODictionary[ItemID.Empty];

        currentUseableItem = Instantiate(itemSO.UsableItemPrefab, handTransform.position, handTransform.rotation, handTransform);

        currentUseableItem.SetUniqueItemID(new StoredItemID());
        currentUseableItem.ParentItemController = this;
        currentUseableItem.IsOwner = IsOwner;
        currentUseableItem.BuildModel();
    }

    #endregion

    // Methods called by UseableItems to perform actions accross the server without having to be network objects themselves
    #region Server Actions

    #region Primary Use

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

    #endregion

    #region Seccondary Use

    [ServerRpc(RequireOwnership = false)]
    public void SeccondaryUseServerRpc()
    {
        SeccondaryUseClientRpc();
    }

    [ClientRpc]
    private void SeccondaryUseClientRpc()
    {
        if (IsOwner) // TODO: figure out why this is needed and coommented out eslewhere
            return;

        currentUseableItem.SeccondaryUseServerAction();
    }

    #endregion

    #region Reload

    [ServerRpc(RequireOwnership = false)]
    public void ReloadServerRpc()
    {
        ReloadClientRpc();
    }

    [ClientRpc]
    private void ReloadClientRpc()
    {
/*        if(IsOwner) 
            return;*/

        currentUseableItem.ReloadServerAction();
    }

    #endregion

    #endregion

    // Methods called by InputSystem events
    #region Getting Inputs

    public void SetUseInput(InputAction.CallbackContext context)
    {
        _useInput = context.action.IsPressed();
    }

    public void SetSeccondaryUseInput(InputAction.CallbackContext context)
    {
        currentUseableItem.OnSeccondaryUseItemInput(context);
    }

    public void SetReloadInput(InputAction.CallbackContext context)
    {
        _reloadInput = context.action.IsPressed();
    }

    public void SetInspectInput(InputAction.CallbackContext context)
    {
        _inspecting = context.action.IsPressed();

        if (context.action.WasPerformedThisFrame())
            OnStartInspect();

        if (context.action.WasReleasedThisFrame())
            OnStopInspect();
    }
    #endregion
}
