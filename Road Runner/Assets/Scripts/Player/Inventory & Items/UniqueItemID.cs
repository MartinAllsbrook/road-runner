using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Inventory;

[System.Serializable]
public class UniqueItemID : INetworkSerializable
{
    [SerializeField] ItemID baseItemID;
    public ItemID BaseItemID
    {
        get { return baseItemID; }
    }

    // Ordered list of modifications to the base item, each representing an item attached to the base item
    [SerializeField] ItemID[] modifications;
    public ItemID[] Modifications
    {
        get { return modifications; }
    }

    // Item type that is stored in this item
    [SerializeField] ItemID counterItem;
    public ItemID CounterItem
    {
        get { return counterItem; }
    }

    // Number of items stored in this item
    [SerializeField] int counterCount;
    public int CounterCount
    {
        get { return counterCount; }
    }

    public Vector2Int Dimensions
    {
        get { return ItemSODictionary[baseItemID].InInventoryDimensions; }
    }

    #region Constructors
    // Constructor for an item with no modifications or counter
    public UniqueItemID()
    {
        baseItemID = ItemID.Empty;

        modifications = new ItemID[0];

        counterItem = ItemID.Empty;
        counterCount = 0;
    }

    public UniqueItemID(ItemID baseItemID)
    {
        this.baseItemID = baseItemID;

        modifications = new ItemID[0];

        counterItem = ItemID.Empty;
        counterCount = 0;
    }

    // Constructor for an item with no counter
    public UniqueItemID(ItemID baseItemID, ItemID[] modifications)
    {
        this.baseItemID = baseItemID;
        this.modifications = modifications;

        counterItem = ItemID.Empty;
        counterCount = 0;
    }

    // Constructor for an item with no modifications
    public UniqueItemID(ItemID baseItemID, ItemID counterItem, int counterCount)
    {
        this.baseItemID = baseItemID;
        this.counterItem = counterItem;
        this.counterCount = counterCount;

        modifications = new ItemID[0];
    }

    // Constructor for an item with modifications and a counter
    public UniqueItemID(ItemID baseItemID, ItemID[] modifications, ItemID counterItem, int counterCount)
    {
        this.baseItemID = baseItemID;
        this.modifications = modifications;

        this.counterItem = counterItem;
        this.counterCount = counterCount;
    }
    #endregion

    // Network serialization interface
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref baseItemID);
        serializer.SerializeValue(ref modifications);
        serializer.SerializeValue(ref counterItem);
        serializer.SerializeValue(ref counterCount);
    }
}
