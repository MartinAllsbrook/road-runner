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
public class Inventory : MonoBehaviour, IPersistantData
{
    public static Inventory Instance;

    #region Variables
    [Header("Hotbar")]
    [SerializeField] private int hotbarSize = 9;
    [SerializeField] private int slotWidth = 2;
    [SerializeField] private int slotHeight = 2;

    [Header("Clothing")]
    [SerializeField] private int numClothingSlots = 8;

    private UniqueItemID inventoryHand; // The item being moved around the inventory
    
    private StoredItemID usingItem; // The item being used by the player

    // Inventories
    private Hotbar hotbar;
    private Dictionary<int, ConnectedInventory> connectedInventories;

    private InventoryUI inventoryUI;

    private StoredItemID heldItem;

    private bool initialized = false;

    private string debugTag = LogColors.GetColoredTag("[Inventory]", LogColors.InventoryColor);
    #endregion

    // Adds each itemSO from each itemSOList to the itemSODictionary


    protected void Start()
    {
        //inventoryUI = InventoryUI.Instance; // Also moved


        if (Instance == null)
            Instance = this;

        Debug.Log(Instance);

        // Code below moved to LoadData
/*       connectedInventories = new Dictionary<int, ConnectedInventory>();
        SetInventoryHand(new UniqueItemID());
        inventoryUI.InitializeInventoryDisplay(this);
        CreateHotbar();*/
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
    public void TryPickUpItem(ItemPickup itemPickup)
    {
        if (TryFitAnywehere(itemPickup.UniqueItemID))
        {
            itemPickup.RemoveFromWorld();
        }
    }

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
    private void RemoveConnectedInventory(int key)
    {
        inventoryUI.RemoveIventoryDisplay(key);
        connectedInventories.Remove(key);
    }

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

        DropItem(inventoryHand);
        SetInventoryHand(new UniqueItemID());
    }

    private void DropItem(UniqueItemID uniqueItemID)
    {
        ObjectSpawner.Instance.ItemSpawnRequest(uniqueItemID, Player.LocalPlayerInstance.transform.position + transform.up * 2);
    }

    /// <summary>
    /// This method is used to drop all items.
    /// </summary>
    public void DropAllItems()
    {
        if (inventoryHand.BaseItemID != ItemID.Empty)
        {
            DropItem(inventoryHand);
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
            DropItem(keyValuePair.Value.UniqueItemID);
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

    public StoredItemID GetNextItemAtHotbarSlot(int slotIndex)
    {
        return hotbar.GetItemAtSlot(slotIndex); // TODO: Add "Next" to the name of this method and make it cycle through items
    }

    public void RemoveUsing()
    {
        HoldItem(new StoredItemID());
    }

    public void HoldItem(StoredItemID storedItemID) // TODO: Make this private again
    {
        heldItem = storedItemID;
        UseableItemController.Instance.HoldItem(storedItemID); // Instace should be the local instance
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
        if (!initialized)
        {
            inventoryUI = InventoryUI.Instance;

            connectedInventories = new Dictionary<int, ConnectedInventory>();
            inventoryUI.InitializeInventoryDisplay(this);
            SetInventoryHand(new UniqueItemID());
            CreateHotbar();

            initialized = true;
        }

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
}
