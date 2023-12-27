using Mono.CSharp;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static ClothingItemSO;
using static ConnectedInventory;

/// <summary>
/// This class represents the inventory system of the game. It communicates with the InventoryUI class to update the UI based on the inventory state.
/// </summary>
public class Inventory : NetworkBehaviour
{
    public static Inventory Instance;

    [Header("World Interaction")]
    [SerializeField] private float maxItemPickupDistance;
    [SerializeField] private LayerMask isItemPickup;
    [SerializeField] private LayerMask isVehicle;
    [SerializeField] private Inventory droppedItemBag;

    [Header("Item Refs")]
    [SerializeField] private ItemSO[] itemSos;

    [Header("Hotbar")]
    [SerializeField] private int hotbarSize = 9;
    [SerializeField] private int slotWidth = 2;
    [SerializeField] private int slotHeight = 2;

    [Header("Clothing")]
    [SerializeField] private int numClothingSlots;

    private InventoryItem inventoryHand; // The item being moved around the inventory
    
    private InventoryItem usingItem; // The item being used by the player
    private UseableItemController useableItemController;


    private VehicleInteractionController vehicle;

    private Transform mainCamera;

    // Inventories
    private Hotbar hotbar;
    private Dictionary<int, ConnectedInventory> connectedInventories;

    private InventoryUI inventoryUI;

    protected static Dictionary<InventoryItem, ItemSO> itemSoDictionary;
    public static Dictionary<InventoryItem, ItemSO> ItemSODictionary { get { return itemSoDictionary; } }

    public enum InventoryItem
    {
        Empty,

        Gun_Ak74,
        Gun_BenneliM4,
        Gun_M4_8,
        Gun_M107,
        Gun_M1911,
        Gun_Rpg7,
        Gun_Uzi,
        Gun_M249,

        Ammo_9mm,

        Consumable_Apple,
        Consumable_WaterBottle,
        Consumable_Beans,
        Consumable_Medkit,
        Consumable_Pills,

        Clothing_Backpack
    }

    /// <summary>
    /// This method is called at the start of the game. It initializes the inventory system.
    /// </summary>
    protected void Start()
    {
        useableItemController = GetComponent<UseableItemController>();
        inventoryUI = InventoryUI.Instance;

        if (!IsOwner)
            return;

        if (Instance == null)
            Instance = this;

        Debug.Log(Instance);

        // Create hotbar

        if (itemSoDictionary == null)
        {
            itemSoDictionary = new Dictionary<InventoryItem, ItemSO>();
            for (int i = 0; i < itemSos.Length; i++)
                itemSoDictionary.Add(itemSos[i].InventoryItem, itemSos[i]);
        }

        connectedInventories = new Dictionary<int, ConnectedInventory>();

        SetInventoryHand(InventoryItem.Empty);

        mainCamera = Camera.main.transform;

        // Create Inventory Stuff
        inventoryUI.InitializeInventoryDisplay(this);
        CreateHotbar();
    }



    /// <summary>
    /// This method is called when an item is picked up.
    /// </summary>
    /// <param name="context">The context of the input action.</param>
    public void OnItemPickUpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (vehicle != null)
            {
                RemoveConnectedInventory(vehicle.GetInvetory().GetLocalKey());
                vehicle.ExitVehicle(GetNetworkObject(NetworkObjectId));
                vehicle = null;
            }
            else
            {
                RaycastForPickups();
            }
        }
    }

    #region Picking up items

    /// <summary>
    /// This method is used to raycast for pickups in the game world.
    /// </summary>
    private void RaycastForPickups()
    {
        Ray ray = new Ray(mainCamera.position, mainCamera.forward);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, maxItemPickupDistance, isItemPickup))
        {
            if (raycastHit.transform.CompareTag("Test Add Inventory"))
            {
                ConnectedInventory invetoryToAdd = raycastHit.transform.GetComponent<ConnectedInventory>();
                AddConnectedInventory(invetoryToAdd);
                return;
            }

            ItemPickup itemPickup = raycastHit.transform.GetComponent<ItemPickup>();
            TryPickUpItem(itemPickup);
        }

        if (Physics.Raycast(ray, out raycastHit, maxItemPickupDistance, isVehicle))
        {
            VehicleInteractionController vehicleInteractionController = raycastHit.transform.GetComponent<VehicleInteractionController>();

            EnterVehicle(vehicleInteractionController);
        }
    }

    /// <summary>
    /// This method is used to try and pick up an item.
    /// </summary>
    /// <param name="itemPickup">The item to pick up.</param>
    private void TryPickUpItem(ItemPickup itemPickup)
    {
        if (TryFitAnywehere(itemPickup.GetScriptableObject().InventoryItem))
        {
            itemPickup.RemoveFromWorld();
        }
    }

    /// <summary>
    /// This method is used to try and fit an item anywhere in the inventory.
    /// </summary>
    /// <param name="inventoryItem">The item to fit.</param>
    /// <returns>True if the item can fit, false otherwise.</returns>
    private bool TryFitAnywehere(InventoryItem inventoryItem)
    {
        var keys = connectedInventories.Keys;

        foreach (var key in keys)
        {
            if (connectedInventories[key].TryFitItem(inventoryItem, out int containedItemKey, out Vector2Int topLeft))
            {
                inventoryUI.AddItemDisplay(key, containedItemKey, itemSoDictionary[inventoryItem], topLeft);
                return true;
            }
        }

        return false;
    }
    #endregion

    #region All-inventory Methods
    /// <summary>
    /// This method is used to remove a connected inventory.
    /// </summary>
    /// <param name="key">The key of the inventory to remove.</param>
    private void RemoveConnectedInventory(int key)
    {
        inventoryUI.RemoveIventoryDisplay(key);
        connectedInventories.Remove(key);
    }

    /// <summary>
    /// This method is used to add a connected inventory.
    /// </summary>
    /// <param name="inventoryToConnect">The inventory to connect.</param>
    private void AddConnectedInventory(ConnectedInventory inventoryToConnect)
    {
        int inventoryKey = GetAvailableIndex();
        connectedInventories.Add(inventoryKey, inventoryToConnect);
        inventoryToConnect.SetLocalKey(inventoryKey);

        Vector2Int invetoryDimensions = inventoryToConnect.GetDimensions();
        //inventoryUI.CreateInventoryDisplay(inventoryKey, invetoryDimensions.x, invetoryDimensions.y);

        for (int x = 0; x < invetoryDimensions.x; ++x)
        {
            for (int y = 0; y < invetoryDimensions.y; ++y)
            {
                //inventoryUI.UpdateItemSlot(inventoryKey, x, y, inventoryToConnect.GetItemAt(x, y));
            }
        }
    }

    /// <summary>
    /// This method is used to try and place an item in a slot.
    /// </summary>
    /// <param name="inventoryIndex">The index of the inventory.</param>
    /// <param name="slot">The slot to place the item in.</param>
    /// <returns>True if the item can be placed, false otherwise.</returns>
    public bool TryPlaceInSlot(int inventoryIndex, Vector2Int slot)
    {
        if (inventoryHand == InventoryItem.Empty)
        {
            return false; // or true it doesn't matter
        }

        Debug.Log("Trying to place item in slot: " + slot + " of Inventory: " + inventoryIndex);

        ConnectedInventory inventory = connectedInventories[inventoryIndex];

        ItemSO itemSO = itemSoDictionary[inventoryHand];
        Vector2Int dimensions = itemSO.InInventoryDimensions;

        if (inventory.IsAreaAvailable(slot, dimensions))
        {
            Debug.Log("Placing item in slot: " + slot);

            int containedItemKey = inventory.AddItem(inventoryHand, slot, dimensions);

            inventoryUI.AddItemDisplay(inventoryIndex, containedItemKey, itemSO, slot);
            
            SetInventoryHand(InventoryItem.Empty);

            return true;
        }

        return false;
    }

    /// <summary>
    /// This method is used to retrieve an item from the inventory.
    /// </summary>
    /// <param name="inventoryIndex">The index of the inventory.</param>
    /// <param name="containedItem">The item to retrieve.</param>
    /// <returns>True if the item can be retrieved, false otherwise.</returns>
    public bool RetrieveItem(int inventoryKey, int itemKey)
    {
        if (inventoryHand != InventoryItem.Empty)
        {
            // Maybe swap items in the furture
            return false;
        }

        InventoryItem retrievedItem = RemoveItem(inventoryKey, itemKey);

        SetInventoryHand(retrievedItem);
       
        return true;
    }

    public bool ConsumeItem(int itemKey)
    {
        RemoveItem(0, itemKey);

        return true;
    }

    private InventoryItem RemoveItem(int inventoryKey, int itemKey)
    {
        Debug.Log("Removing item from inventory: " + inventoryKey + ", item: " + itemKey);
        ContainedItem containedItem = connectedInventories[inventoryKey].GetContainedItem(itemKey);

        inventoryUI.ShowButtonArea(inventoryKey, containedItem.topLeft, containedItem.inventoryDimensions);
        inventoryUI.DestroyItemDisplay(inventoryKey, itemKey);

        InventoryItem retrievedItem = connectedInventories[inventoryKey].RemoveItem(itemKey);

        return retrievedItem;
    }

    /// <summary>
    /// This method is used to get an available index for the inventory.
    /// </summary>
    /// <returns>The available index.</returns>
    private int GetAvailableIndex()
    {
        int numReservedSlots = numClothingSlots + 1;

        for (int i = numReservedSlots; i < 100; i++)
        {
            if (!connectedInventories.ContainsKey(i))
                return i;
        }

        Debug.LogError("Dude wtf there are more than 100 inventories stop it rn. Also you just broke my inventory system");
        return -1;
    }

    /// <summary>
    /// This method is used to enter a vehicle.
    /// </summary>
    /// <param name="vehicleInteractionController">The vehicle to enter.</param>
    private void EnterVehicle(VehicleInteractionController vehicleInteractionController)
    {
        vehicle = vehicleInteractionController;
        vehicleInteractionController.EnterVehicle(GetNetworkObject(NetworkObjectId));

        AddConnectedInventory(vehicleInteractionController.GetInvetory());
    }
    #endregion

    #region Clothing Methods

    public void UpdateClothingInventory(ClothingItemSO clothingSO)
    {
        int inventoryKey = (int) clothingSO.ClothingSlot;

        if (connectedInventories.ContainsKey(inventoryKey))
        {
            DropAllItems(inventoryKey);
            inventoryUI.RemoveIventoryDisplay(inventoryKey);
        }

        ConnectedInventory connectedInventory = new ConnectedInventory(clothingSO.ClothingInventoryDimensions);
        connectedInventories.Add(inventoryKey, connectedInventory);

        inventoryUI.CreateInventoryDisplay(inventoryKey, clothingSO.ClothingInventoryDimensions);
        inventoryUI.SetClothingSlot(clothingSO);
    }

    public void RemoveClothingInventory(ClothingItemSO clothingItemSO)
    {
        if(inventoryHand == InventoryItem.Empty)
        {
            int inventoryKey = (int)clothingItemSO.ClothingSlot;

            SetInventoryHand(clothingItemSO.InventoryItem);

            DropAllItems(inventoryKey);
            RemoveConnectedInventory(inventoryKey);

            inventoryUI.RemoveClothingSlot(inventoryKey);
        }
    }

    #endregion

    #region Little helper methods

    private void SetInventoryHand(InventoryItem item)
    {
        inventoryHand = item;
        inventoryUI.SetInventoryHand(item);
    }

    #endregion

    #region Drop Item Methods
    /// <summary>
    /// This method is used to drop an item.
    /// </summary>
    public void DropItem()
    {
        if (inventoryHand == InventoryItem.Empty)
            return;

        DropItemServerRpc(inventoryHand);
        SetInventoryHand(InventoryItem.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(InventoryItem item)
    {
        GameObject itemGameObject = Instantiate(itemSoDictionary[item].ItemPickupPrefab, transform.position + Vector3.up * 2.5f, new Quaternion(0, 0, 0, 0));

        NetworkObject itemNetworkObject = itemGameObject.GetComponent<NetworkObject>();
        itemNetworkObject.Spawn(true);
    }

    /// <summary>
    /// This method is used to drop all items.
    /// </summary>
    public void DropAllItems()
    {
        if (inventoryHand != InventoryItem.Empty)
        {
            DropItemServerRpc(inventoryHand);
            SetInventoryHand(InventoryItem.Empty);
        }

        inventoryUI.ResetInventoryDisplay();


        foreach (var key in connectedInventories.Keys)
        {
            // TODO: Might need to do a check in the future to make sure were not dropping from chests or vehicles
            DropAllItems(key);
        }

    }

    public void DropAllItems(int inventoryKey)
    {
        ConnectedInventory connectedInventory = connectedInventories[inventoryKey];
        Dictionary<int, ContainedItem> containedItems = connectedInventory.GetAndClearItems();
        foreach (KeyValuePair<int, ContainedItem> keyValuePair in containedItems)
        {
            inventoryUI.DestroyItemDisplay(inventoryKey, keyValuePair.Key);
            DropItemServerRpc(keyValuePair.Value.inventoryItem);
        }
    }
    #endregion

    #region Hotbar and UsingItem Methods
    /// <summary>
    /// This method creates the hotbar for the inventory system.
    /// </summary>
    private void CreateHotbar()
    {
        hotbar = new Hotbar(hotbarSize, slotWidth, slotHeight);
        connectedInventories.Add(0, hotbar);

        inventoryUI.CreateHotbarSlotUIs(hotbarSize, slotWidth, slotHeight);
    }

    /// <summary>
    /// This method is called when a hotbar key is pressed.
    /// </summary>
    /// <param name="context">The context of the input action.</param>
    public void OnHotbarKeyPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            string keyName = context.control.name;
            int hotbarSlotIndex = Int32.Parse(keyName) - 1;

            InventoryItem item = hotbar.GetItemAtSlot(hotbarSlotIndex, out int itemKey);

            Debug.Log("Slot index: " + hotbarSlotIndex + ", Item: " + item + ", Key: " + itemKey);

            HoldItemServerRpc(item, itemKey);
        }
    }

    /// <summary>
    /// This method is used to remove the item that is currently being used.
    /// </summary>
    public void RemoveUsing()
    {
        HoldItemServerRpc(InventoryItem.Empty, -1);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HoldItemServerRpc(InventoryItem item, int itemKey)
    {
        HoldItemClientRpc(item, itemKey);
    }

    [ClientRpc]
    private void HoldItemClientRpc(InventoryItem item, int itemKey)
    {
        useableItemController.SetItem(itemSoDictionary[item], itemKey);
    }
    #endregion

    #region Debug Commands
    [Command]
    public void SpawnItemDebug(int x, int y, int z, InventoryItem itemEnum)
    {
        Vector3 position = new Vector3(x, y, z);

        if (!IsServer)
        {
            SpawnItemServerRpc(itemEnum, position);
            return;
        }

        SpawnedObject itemSpawnedObject = itemSoDictionary[itemEnum].ItemPickupPrefab.GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }

    [Command]
    public void SpawnItemDebug(InventoryItem itemEnum)
    {
        Transform playerTransform = PlayerSpawner.localPlayerSpawner.transform;
        Vector3 position = playerTransform.position + playerTransform.forward * 2;

        if (!IsServer)
        {
            SpawnItemServerRpc(itemEnum, position);
            return;
        }

        SpawnedObject itemSpawnedObject = itemSoDictionary[itemEnum].ItemPickupPrefab.GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemServerRpc(InventoryItem itemEnum, Vector3 position)
    {
        SpawnedObject itemSpawnedObject = itemSoDictionary[itemEnum].ItemPickupPrefab.GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }

    #endregion
}
