using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static GlobalItemDictionary;

[Serializable]
public class UniqueItemID : INetworkSerializable, ISerializationCallbackReceiver
{
    [SerializeField] private ItemID _baseItemID;
    
    [SerializeField] private ItemID counterItem; // Item type that is stored in this 
    [SerializeField] private int counterCount; // Number of items stored in this item

    [SerializeField] private int numModSlots;

    private UniqueItemID[] modifications; // Ordered list of modifications to the base item, each representing an item attached to the base item

    [SerializeField] private SerializedUIIDMod[] serializedModifications;

    #region Debug Method

    private string debugTag = "<color=#ff00ffff>[UniqueItemID] </color>";

    private void LogAllSerilizedFields(string start)
    {
        Debug.Log(debugTag + start + "BaseItemID: " + _baseItemID + "CounterItem: " + counterItem + "CounterCount: " + counterCount + "NumModSlots: " + numModSlots);
    }

    #endregion

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

    public int NumModSlots
    {
        get { return numModSlots; }
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
        numModSlots = 0;
        
        counterItem = ItemID.Empty;
        counterCount = 0;

        LogAllSerilizedFields("Creted new UIID");
    }

    // Constructor for an item with no modifications or counter
    public UniqueItemID(ItemID baseItemID)
    {
        _baseItemID = baseItemID;

        modifications = CreateDefaultModifications(baseItemID);
        numModSlots = modifications.Length;

        counterItem = ItemID.Empty;
        counterCount = 0;

        LogAllSerilizedFields("Creted new UIID");
    }

    // Constructor for an item with no counter
    public UniqueItemID(ItemID baseItemID, UniqueItemID[] modifications)
    {
        _baseItemID = baseItemID;
        
        if(VerifyModificationsArray(modifications))
            this.modifications = modifications;
        numModSlots = modifications.Length;

        counterItem = ItemID.Empty;
        counterCount = 0;

        LogAllSerilizedFields("Creted new UIID");
    }

    // Constructor for an item with no modifications
    public UniqueItemID(ItemID baseItemID, ItemID counterItem, int counterCount)
    {
        _baseItemID = baseItemID;

        modifications = CreateDefaultModifications(baseItemID);
        numModSlots = modifications.Length;

        this.counterItem = counterItem;
        this.counterCount = counterCount;
        
        LogAllSerilizedFields("Creted new UIID");
    }

    // Constructor for an item with unspecified modifications (no dictionary lookup)
    public UniqueItemID(ItemID baseItemID, int numModSlots, ItemID counterItem, int counterCount)
    {
        _baseItemID = baseItemID;

        this.numModSlots = numModSlots;
        this.modifications = new UniqueItemID[numModSlots];

        this.counterItem = counterItem;
        this.counterCount = counterCount;
        
        LogAllSerilizedFields("Creted new UIID");
    }

    // Constructor for an item with modifications and a counter
    public UniqueItemID(ItemID baseItemID, UniqueItemID[] modifications, ItemID counterItem, int counterCount)
    {
        _baseItemID = baseItemID;

        if (VerifyModificationsArray(modifications))
            this.modifications = modifications;
        numModSlots = modifications.Length;

        this.counterItem = counterItem;
        this.counterCount = counterCount;
        
        LogAllSerilizedFields("Creted new UIID");
    }

    #endregion

    #region Public Methods

    public bool TryModifyItem(UniqueItemID modificationID, int modificationSlot, out UniqueItemID oldModID)
    {
        LogAllSerilizedFields("Started Modifying UIID");

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
            
            LogAllSerilizedFields("Finished Modifying UIID");

            return true;
        }

        oldModID = modifications[modificationSlot];
        modifications[modificationSlot] = modificationID;

        LogAllSerilizedFields("Finished Modifying UIID");

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
            Debug.Log(debugTag + "Setting counter item to " + itemID + " with count " + count);
            counterItem = itemID;
            counterCount = count;
            return true;
        }

        if (counterItem != itemID)
        {
            Debug.LogWarning(debugTag + "Cannot add " + itemID + " to " + _baseItemID + " because it is not the same type as the counter item " + counterItem);
            return false;
        }

        if (counterCount + count > MaxCounterCount())
        {
            Debug.LogWarning(debugTag + "Cannot add " + count + " " + itemID + " to " + _baseItemID + " because it would exceed the max counter count of " + MaxCounterCount());
            return false;
        }

        Debug.Log(debugTag + "Adding " + count + " " + itemID + " to " + _baseItemID + " with counter count " + counterCount);
        counterCount += count;
        return true;
    }

    public bool TryRemoveItemFromCounter(int count, out ItemID counterItemOut)
    {
        if (counterItem == ItemID.Empty)
        {
            //Debug.LogWarning(debugTag + "Cannot remove " + count + " from " + _baseItemID + " because it has no counter item");
            counterItemOut = ItemID.Empty;
            return false;
        }

        if (counterCount - count < 0)
        {
            //Debug.LogWarning(debugTag + "Cannot remove " + count + " from " + _baseItemID + " because it would result in a negative counter count");
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
            Debug.LogError(debugTag + _baseItemID + " does not have a modification slot " + modificationSlot + ". MaxMods = " + ModificationCount());
            return false;
        }

        return true;
    }

    private bool VerifyModificationsArray(UniqueItemID[] modifications)
    {
        if (modifications.Length != ModificationCount())
        {
            Debug.LogError(debugTag + "Length of modifications array must be equal to " + _baseItemID + "'s ModificationCount of " + ModificationCount());
            return false;
        }

        // TODO: Check that all modifications are valid for this item

        return true;
    }

    private UniqueItemID[] CreateDefaultModifications(ItemID baseItemID)
    {
        Debug.Log(debugTag + "Creating default modifications for " + baseItemID);
        Debug.Log(debugTag + "Default mods: " + DefaultModifications().Length);
        Debug.Log(debugTag + "Max mods: " + ModificationCount());

        ItemID[] defaultModItemIDs = DefaultModifications();
        numModSlots = ModificationCount();
        UniqueItemID[] defaultModifications = new UniqueItemID[numModSlots];

        for (int i = 0; i < defaultModifications.Length; i++)
        {
            defaultModifications[i] = new UniqueItemID(defaultModItemIDs[i]);
        }

        return defaultModifications;
    }

    private int ModificationCount()
    {
        numModSlots = ItemSODictionary[_baseItemID].MaxModifications; // This method make me go BRUH
        return numModSlots;
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

    #region JSON Serialization

    public void OnBeforeSerialize()
    {
        serializedModifications = RecursivelySerializeMods(this, new int[0]);
    }

    // Recursive function to traverse mod tree and serialize all modifications
    private SerializedUIIDMod[] RecursivelySerializeMods(UniqueItemID modification, int[] pathToMod)
    {
        List<SerializedUIIDMod> serializedMods = new List<SerializedUIIDMod>();

        // Add this mod to the list if it is not the base item
        if (modification != this)
            serializedMods.Add(new SerializedUIIDMod(modification.BaseItemID, pathToMod, modification.numModSlots, modification.counterItem, modification.counterCount));

        // Check if we are at the end of the path
        for (int i = 0; i < modification.modifications.Length; i++)
        {
            //if (modification.modifications[i]._baseItemID != ItemID.Empty)
            //{
                // Create a new path to the next mod
                int[] newPath = new int[pathToMod.Length + 1];
                pathToMod.CopyTo(newPath, 0);
                newPath[newPath.Length - 1] = i;

                // Recursively serialize the next mod and add it to the list
                serializedMods.AddRange(RecursivelySerializeMods(modification.modifications[i], newPath));
            //}
        }

        return serializedMods.ToArray();
    }

    public void OnAfterDeserialize()
    {
        modifications = new UniqueItemID[numModSlots];
        DeserilaizeMods(serializedModifications);
    }

    private void DeserilaizeMods(SerializedUIIDMod[] serializedUIIDMods)
    {
        for (int i = 0; i < serializedUIIDMods.Length; i++)
        {
            int[] modPath = serializedUIIDMods[i].ModPath;

            if (modPath.Length == 0)
                continue; // Skip the base item

            SerializedUIIDMod serializedMod = serializedUIIDMods[i];
            UniqueItemID modification = new UniqueItemID(serializedMod.BaseItemID, serializedMod.NumModSlots, serializedMod.CounterItem, serializedMod.CounterCount);

            AddModToTree(this, modification, modPath);
        }
    }

    private void AddModToTree(UniqueItemID parent, UniqueItemID modification, int[] modPath)
    {
        if (modPath.Length == 1)
        {
            parent.modifications[modPath[0]] = modification;
            return;
        }

        int[] newModPath = modPath.SubArray(1, modPath.Length - 1); // SubArray is a method from QFSW.QC.Utilities.CollectionExtensions that uses Array.Copy to create a sub array
        AddModToTree(parent.modifications[modPath[0]], modification, newModPath);
    }

    // Debug function to print the mod tree
    private void PrintModTree(UniqueItemID modification, int depth)
    {
        string indent = "";
        for (int i = 0; i < depth; i++)
        {
            indent += "  ";
        }

        Debug.Log(debugTag + indent + modification.BaseItemID + " " + modification.counterItem + " " + modification.counterCount);

        for (int i = 0; i < modification.modifications.Length; i++)
        {
            if (modification.modifications[i]._baseItemID != ItemID.Empty)
            {
                PrintModTree(modification.modifications[i], depth + 1);
            }
        }
    }

    #endregion

    #region Network Serialization
    // Network serialization interface
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        Debug.Log(debugTag + "Serializing " + _baseItemID + " with " + modifications.Length + " mods");

        serializer.SerializeValue(ref _baseItemID);

        serializer.SerializeValue(ref modifications); 
        serializer.SerializeValue(ref numModSlots); 

        serializer.SerializeValue(ref counterItem);
        serializer.SerializeValue(ref counterCount);
    }

    #endregion
}
