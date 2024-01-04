using Unity.Netcode;
using UnityEngine;
using static Inventory;

public class StoredItemID : INetworkSerializable
{
    private UniqueItemID itemID;

    // Variables that describe where the item is stored
    private int inventoryKey;
    private int itemKey;
    private Vector2Int inventoryPosition;
    
    public StoredItemID()
    {
        itemID = new UniqueItemID();
        inventoryKey = -1;
        itemKey = -1;
        inventoryPosition = new Vector2Int(-1, -1);
    }

    public StoredItemID(UniqueItemID itemID, int inventoryKey, int itemKey, Vector2Int inventoryPosition)
    {
        this.itemID = itemID;
        this.inventoryKey = inventoryKey;
        this.itemKey = itemKey;
        this.inventoryPosition = inventoryPosition;
    }

    public UniqueItemID UniqueItemID
    {
        get { return itemID; }
        set { itemID = value; } // TODO: Remove this setter and make a better system for updating items
    }

    public int InventoryKey
    {
        get { return inventoryKey; }
    }

    public int ItemKey
    {
        get { return itemKey; }
    }

    public Vector2Int TopLeft
    {
        get { return inventoryPosition; }
    }

    // Network Serialization
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemID);

        serializer.SerializeValue(ref inventoryKey);
        serializer.SerializeValue(ref itemKey);

        serializer.SerializeValue(ref inventoryPosition);
    }

}
