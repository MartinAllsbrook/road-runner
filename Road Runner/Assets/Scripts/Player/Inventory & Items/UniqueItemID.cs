using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Inventory;

[System.Serializable]
public class UniqueItemID : INetworkSerializable
{
    // TODO: Add underscores before private fields

    [SerializeField] private ItemID _baseItemID;

    private UniqueItemID[] modifications; // Ordered list of modifications to the base item, each representing an item attached to the base item
    
    // TODO: These fields do not need to be serialized because their default values can come from the ItemSO
    [SerializeField] private ItemID counterItem; // Item type that is stored in this 
    [SerializeField] private int counterCount; // Number of items stored in this item

    #region Properties
    public ItemID BaseItemID
    {
        get { return _baseItemID; }
    }
    
    public UniqueItemID[] Modifications
    {
        get { return modifications; }
    }
    
    public ItemID CounterItem
    {
        get { return counterItem; }
    }

    public int CounterCount
    {
        get { return counterCount; }
    }

    public Vector2Int Dimensions
    {
        get { return ItemSODictionary[_baseItemID].InInventoryDimensions; }
    }

    #endregion

    #region Constructors
    // All constructors are orgainized into:
    // 1. Set baseItemID
    // 2. Set modifications
    // 3. Set counterItem and counterCount

    // Empty constructor creates an empty item
    public UniqueItemID()
    {
        _baseItemID = ItemID.Empty;

        modifications = new UniqueItemID[0]; // Empty array of modifications for empty item

        counterItem = ItemID.Empty;
        counterCount = 0;

    }

    // Constructor for an item with no modifications or counter
    public UniqueItemID(ItemID baseItemID)
    {
        _baseItemID = baseItemID;

        modifications = CreateDefaultModifications(baseItemID);

        counterItem = ItemID.Empty;
        counterCount = 0;

    }

    // Constructor for an item with no counter
    public UniqueItemID(ItemID baseItemID, UniqueItemID[] modifications)
    {
        _baseItemID = baseItemID;
        
        if(VerifyModificationsArray(modifications))
            this.modifications = modifications;

        counterItem = ItemID.Empty;
        counterCount = 0;

    }

    // Constructor for an item with no modifications
    public UniqueItemID(ItemID baseItemID, ItemID counterItem, int counterCount)
    {
        _baseItemID = baseItemID;

        modifications = CreateDefaultModifications(baseItemID);

        this.counterItem = counterItem;
        this.counterCount = counterCount;

    }

    // Constructor for an item with modifications and a counter
    public UniqueItemID(ItemID baseItemID, UniqueItemID[] modifications, ItemID counterItem, int counterCount)
    {
        _baseItemID = baseItemID;

        if (VerifyModificationsArray(modifications))
            this.modifications = modifications;

        this.counterItem = counterItem;
        this.counterCount = counterCount;

    }

    #endregion

    #region Public Methods

    public bool TryModifyItem(UniqueItemID modificationID, int modificationSlot, out UniqueItemID oldModID)
    {
        if (!CanModifySlot(modificationSlot)) // ERROR CHECK
        {
            oldModID = new UniqueItemID();
            return false;
        }

        Debug.Log(modifications.Length);

        if (modifications[modificationSlot]._baseItemID == ItemID.Empty)
        {
            modifications[modificationSlot] = modificationID;
            oldModID = new UniqueItemID();
            return true;
        }

        oldModID = modifications[modificationSlot];
        modifications[modificationSlot] = modificationID;
        return true;
    }

    public bool TryRemoveModification(int modificationSlot, out UniqueItemID oldModID)
    {
        if (!CanModifySlot(modificationSlot)) // ERROR CHECK
        {
            oldModID = new UniqueItemID();
            return false;
        }

        if (modifications[modificationSlot]._baseItemID == ItemID.Empty)
        {
            oldModID = new UniqueItemID();
            return false;
        }

        oldModID = modifications[modificationSlot];
        modifications[modificationSlot] = new UniqueItemID();
        return true;
    }

    public bool TryAddItemToCounter(ItemID itemID, int count)
    {
        if (counterItem == ItemID.Empty)
        {
            Debug.Log("Setting counter item to " + itemID + " with count " + count);
            counterItem = itemID;
            counterCount = count;
            return true;
        }

        if (counterItem != itemID)
        {
            Debug.LogWarning("Cannot add " + itemID + " to " + _baseItemID + " because it is not the same type as the counter item " + counterItem);
            return false;
        }

        if (counterCount + count > MaxCounterCount())
        {
            Debug.LogWarning("Cannot add " + count + " " + itemID + " to " + _baseItemID + " because it would exceed the max counter count of " + MaxCounterCount());
            return false;
        }

        Debug.Log("Adding " + count + " " + itemID + " to " + _baseItemID + " with counter count " + counterCount);
        counterCount += count;
        return true;
    }

    public bool TryRemoveItemFromCounter(int count, out ItemID counterItemOut)
    {
        if (counterItem == ItemID.Empty)
        {
            Debug.LogWarning("Cannot remove " + count + " from " + _baseItemID + " because it has no counter item");
            counterItemOut = ItemID.Empty;
            return false;
        }

        if (counterCount - count < 0)
        {
            Debug.LogWarning("Cannot remove " + count + " from " + _baseItemID + " because it would result in a negative counter count");
            counterItemOut = ItemID.Empty;
            return false;
        }

        counterCount -= count;
        counterItemOut = counterItem;
        return true;
    }

    #endregion

    #region Private Helper Methods

    private bool CanModifySlot(int modificationSlot)
    {
        if (modificationSlot < 0)
        {
            Debug.LogError("Modification slot must be greater than or equal to 0");
            return false;
        }

        if (modificationSlot >= ModificationCount()) 
        {
            Debug.LogError(_baseItemID + " does not have a modification slot " + modificationSlot + ". MaxMods = " + ModificationCount());
            return false;
        }

        return true;
    }

    private bool VerifyModificationsArray(UniqueItemID[] modifications)
    {
        if (modifications.Length != ModificationCount())
        {
            Debug.LogError("Length of modifications array must be equal to " + _baseItemID + "'s ModificationCount of " + ModificationCount());
            return false;
        }

        // TODO: Check that all modifications are valid for this item

        return true;
    }

    private UniqueItemID[] CreateDefaultModifications(ItemID baseItemID)
    {
        Debug.Log("Creating default modifications for " + baseItemID);
        Debug.Log("Default mods: " + DefaultModifications().Length);
        Debug.Log("Max mods: " + ModificationCount());

        ItemID[] defaultModItemIDs = DefaultModifications();
        UniqueItemID[] defaultModifications = new UniqueItemID[ModificationCount()];

        for (int i = 0; i < defaultModifications.Length; i++)
        {
            defaultModifications[i] = new UniqueItemID(defaultModItemIDs[i]);
        }

        return defaultModifications;
    }

    private int ModificationCount()
    {
        return ItemSODictionary[_baseItemID].MaxModifications;
    }
   
    public int MaxCounterCount()
    {
        return ItemSODictionary[_baseItemID].MaxCounterCount;
    }

    private ItemID[] DefaultModifications()
    {
        return ItemSODictionary[_baseItemID].DefaultModifications;
    }

    #endregion

    #region Network Serialization
    // Network serialization interface
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _baseItemID);

        serializer.SerializeValue(ref modifications); // I can't believe this works
        
        serializer.SerializeValue(ref counterItem);
        serializer.SerializeValue(ref counterCount);
    }

    #endregion
}
