using QFSW.QC;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static GlobalItemDictionary;

/// <summary>
/// This class represents the inventory system of the game. It communicates with the InventoryUI class to update the UI based on the inventory state.
/// </summary>
public class Inventory : NetworkBehaviour, IPersistantData
{
    public static Inventory Instance;

    private string debugTag = LogColors.GetColoredTag("[Inventory]", LogColors.InventoryColor);

    #region Variables

    [Header("World Interaction")]
    [SerializeField] private float maxItemPickupDistance;
    [SerializeField] private LayerMask isItemPickup;
    [SerializeField] private LayerMask isVehicle;
    [SerializeField] private Inventory droppedItemBag;

    [Header("Hotbar")]
    [SerializeField] private int hotbarSize = 9;
    [SerializeField] private int slotWidth = 2;
    [SerializeField] private int slotHeight = 2;

    [Header("Clothing")]
    [SerializeField] private int numClothingSlots;

    private UniqueItemID inventoryHand; // The item being moved around the inventory
    
    private StoredItemID usingItem; // The item being used by the player
    private UseableItemController useableItemController;

    private VehicleInteractionController vehicle;

    private Transform mainCamera;

    // Inventories
    private Hotbar hotbar;
    private Dictionary<int, ConnectedInventory> connectedInventories;

    private InventoryUI inventoryUI;

    private StoredItemID heldItem;

    #endregion

    // Adds each itemSO from each itemSOList to the itemSODictionary


    protected void Start()
    {
        useableItemController = GetComponent<UseableItemController>();
/*        inventoryUI = InventoryUI.Instance; // Also moved
*/
        if (!IsOwner)
            return;

        if (Instance == null)
            Instance = this;

        Debug.Log(Instance);
        mainCamera = Camera.main.transform;

        HoldItem(new StoredItemID()); // Hold empty item


        // Code below moved to LoadData
        /*        connectedInventories = new Dictionary<int, ConnectedInventory>();
                SetInventoryHand(new UniqueItemID());
                inventoryUI.InitializeInventoryDisplay(this);
                CreateHotbar();*/
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

    #region Inventory Interaction Methods for other classes

    public StoredItemID[] GetAllItemsOfType(ItemID itemTypes)
    {
        List<StoredItemID> containedItems = new List<StoredItemID>();

        foreach (var key in connectedInventories.Keys)
        {
            StoredItemID[] containedItemsOfType = connectedInventories[key].GetItemsOfType(itemTypes);
            containedItems.AddRange(containedItemsOfType);
        }

        return containedItems.ToArray();
    }

    #endregion


    public void AddItem(UniqueItemID uniqueItemID)
    {
        if (uniqueItemID.BaseItemID != ItemID.Empty)
            TryFitAnywehere(uniqueItemID);
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
        if (TryFitAnywehere(itemPickup.UniqueItemID))
        {
            itemPickup.RemoveFromWorld();
        }
    }

    /// <summary>
    /// This method is used to try and fit an item anywhere in the inventory.
    /// </summary>
    /// <param name="inventoryItem">The item to fit.</param>
    /// <returns>True if the item can fit, false otherwise.</returns>
    private bool TryFitAnywehere(UniqueItemID uniqueItemID)
    {
        foreach (KeyValuePair<int, ConnectedInventory> keyValuePair in connectedInventories)
        {
            if (keyValuePair.Value.TryFitItem(uniqueItemID, out int containedItemKey, out Vector2Int topLeft))
            {
                inventoryUI.AddItemDisplay(keyValuePair.Key, containedItemKey, ItemSODictionary[uniqueItemID.BaseItemID], topLeft);
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
        if (inventoryHand.BaseItemID == ItemID.Empty)
        {
            return false; // or true it doesn't matter
        }

        Debug.Log(debugTag + "Trying to place item in slot: " + slot + " of Inventory: " + inventoryIndex);

        ConnectedInventory inventory = connectedInventories[inventoryIndex];

        Vector2Int dimensions = inventoryHand.Dimensions;

        if (inventory.IsAreaAvailable(slot, dimensions))
        {
            Debug.Log(debugTag + "Placing item in slot: " + slot);

            int containedItemKey = inventory.AddItem(inventoryHand, slot);

            inventoryUI.AddItemDisplay(inventoryIndex, containedItemKey, ItemSODictionary[inventoryHand.BaseItemID], slot);
            
            SetInventoryHand(new UniqueItemID());
            
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
        if (inventoryHand == null)
        {
            Debug.LogWarning(debugTag + "Inventory hand is null lol");
            inventoryHand = new UniqueItemID();
        }

        if (inventoryHand.BaseItemID != ItemID.Empty)
        {
            // TODO: Maybe swap items in the furture
            return false;
        }

        //Debug.Log(debugTag + "Retrieving item from inventory: " + inventoryKey + ", item: " + itemKey);
        //Debug.Log(debugTag + "Held item: " + heldItem + " Held Item Key: " + heldItem.ItemKey);
        if (inventoryKey == 0 && itemKey == heldItem.ItemKey)
        {
            // We are trying to move / pick up the item we are holding
            HoldItem(new StoredItemID()); // Hold empty item
        }

        StoredItemID retrievedItem = RemoveItem(inventoryKey, itemKey);
        SetInventoryHand(retrievedItem.UniqueItemID);
       
        return true;
    }

    public bool ConsumeItem(int itemKey)
    {
        RemoveItem(0, itemKey);

        return true;
    }

    public StoredItemID RemoveItem(int inventoryKey, int itemKey)
    {
        Debug.Log(debugTag + "Removing item from inventory: " + inventoryKey + ", item: " + itemKey);
        StoredItemID containedItem = connectedInventories[inventoryKey].GetStoredItemID(itemKey);

        inventoryUI.ShowButtonArea(inventoryKey, containedItem.TopLeft, containedItem.UniqueItemID.Dimensions);
        inventoryUI.DestroyItemDisplay(inventoryKey, itemKey);

        StoredItemID retrievedItem = connectedInventories[inventoryKey].RemoveItem(itemKey);

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

        Debug.LogError(debugTag + "Dude wtf there are more than 100 inventories stop it rn. Also you just broke my inventory system");
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
        if(inventoryHand.BaseItemID == ItemID.Empty)
        {
            int inventoryKey = (int)clothingItemSO.ClothingSlot;

            SetInventoryHand(new UniqueItemID(clothingItemSO.ItemID));

            DropAllItems(inventoryKey);
            RemoveConnectedInventory(inventoryKey);

            inventoryUI.RemoveClothingSlot(inventoryKey);
        }
    }

    #endregion

    #region Little helper methods

    private void SetInventoryHand(UniqueItemID item)
    {
        inventoryHand = item;
        inventoryUI.SetInventoryHand(item.BaseItemID);
    }

    #endregion

    #region Drop Item Methods
    /// <summary>
    /// This method is used to drop an item.
    /// </summary>
    public void DropItem()
    {
        if (inventoryHand.BaseItemID == ItemID.Empty)
            return;

        DropItemServerRpc(inventoryHand);
        SetInventoryHand(new UniqueItemID());
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(UniqueItemID uniqueItemID)
    {
        ItemPickup instantiatedItemPickup = Instantiate(ItemSODictionary[uniqueItemID.BaseItemID].ItemPickupPrefab, transform.position + Vector3.up * 2.5f, Quaternion.identity);

        // Spawn network object
        NetworkObject itemNetworkObject = instantiatedItemPickup.GetComponent<NetworkObject>();
        itemNetworkObject.Spawn(true);

        instantiatedItemPickup.UniqueItemID = uniqueItemID;
    }

    /// <summary>
    /// This method is used to drop all items.
    /// </summary>
    public void DropAllItems()
    {
        if (inventoryHand.BaseItemID != ItemID.Empty)
        {
            DropItemServerRpc(inventoryHand);
            SetInventoryHand(new UniqueItemID());
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
        Dictionary<int, StoredItemID> containedItems = connectedInventory.GetAndClearItems();
        foreach (KeyValuePair<int, StoredItemID> keyValuePair in containedItems)
        {
            inventoryUI.DestroyItemDisplay(inventoryKey, keyValuePair.Key);
            DropItemServerRpc(keyValuePair.Value.UniqueItemID);
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

            StoredItemID storedItemID = hotbar.GetItemAtSlot(hotbarSlotIndex);

            Debug.Log(debugTag + "Holding item with HotbarSlot index: " + hotbarSlotIndex + ", Item: " + storedItemID);

            HoldItem(storedItemID);
        }
    }

    /// <summary>
    /// This method is used to remove the item that is currently being used.
    /// </summary>
    public void RemoveUsing()
    {
        HoldItemServerRpc(new StoredItemID());
    }

    private void HoldItem(StoredItemID storedItemID)
    {
        heldItem = storedItemID;
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
        useableItemController.SetItem(storedItemID);
    }

    #endregion

    #region Updating UniqueItemIDs

    public void UpdateUniqueItem(int inventoryKey, int itemKey, UniqueItemID modifiedUniqueItem)
    {
        connectedInventories[inventoryKey].UpdateUniqueItem(itemKey, modifiedUniqueItem);
    }

    #endregion

    #region IPersistantData Methods

    public void LoadData(CharacterData characterData)
    {
        // Inventory initialization code moved from Start
        inventoryUI = InventoryUI.Instance;

        connectedInventories = new Dictionary<int, ConnectedInventory>();
        inventoryUI.InitializeInventoryDisplay(this);
        SetInventoryHand(new UniqueItemID());
        CreateHotbar();

        foreach (StoredItemID storedItemID in characterData.StoredItems)
        {
            TryFitAnywehere(storedItemID.UniqueItemID);
        }
    }

    public void SaveData(ref CharacterData characterData)
    {
        List<StoredItemID> allStoredItems = new List<StoredItemID>();

        foreach (ConnectedInventory connectedInventory in connectedInventories.Values)
        {
            foreach (StoredItemID storedItemID in connectedInventory.GetAllItems())
            {
                allStoredItems.Add(storedItemID);
            }
        }

        characterData.StoredItems = allStoredItems.ToArray();
    }

    #endregion

    #region Debug Commands
    [Command]
    public void SpawnItemDebug(int x, int y, int z, ItemID itemEnum)
    {
        Vector3 position = new Vector3(x, y, z);

        if (!IsServer)
        {
            SpawnItemServerRpc(itemEnum, position);
            return;
        }

        SpawnedObject itemSpawnedObject = ItemSODictionary[itemEnum].ItemPickupPrefab.GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }

    [Command]
    public void SpawnItemDebug(ItemID itemEnum)
    {
        Transform playerTransform = PlayerSpawner.localPlayerSpawner.transform;
        Vector3 position = playerTransform.position + playerTransform.forward * 2;

        if (!IsServer)
        {
            SpawnItemServerRpc(itemEnum, position);
            return;
        }

        SpawnedObject itemSpawnedObject = ItemSODictionary[itemEnum].ItemPickupPrefab.GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemServerRpc(ItemID itemEnum, Vector3 position)
    {
        SpawnedObject itemSpawnedObject = ItemSODictionary[itemEnum].ItemPickupPrefab.GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }
    #endregion
}
