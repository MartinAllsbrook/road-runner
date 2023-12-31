using UnityEngine;
using static Inventory;

public class StoredItemID
{
    private UniqueItemID itemID;

    // Variables that describe where the item is stored
    private int inventoryKey;
    private int itemKey;
    private Vector2Int inventoryPosition;

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
}
