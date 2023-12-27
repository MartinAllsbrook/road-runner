using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class Hotbar : ConnectedInventory
{
    int _numSlots;
    int _slotWidth;
    int _slotHeight;

    public Hotbar(int numSlots, int slotWidth, int slotHeight) : base(new Vector2Int(numSlots * slotWidth, slotHeight))
    {
        _numSlots = numSlots;
        _slotWidth = slotWidth;
        _slotHeight = slotHeight;
        InitializeInventory();
    }

    public InventoryItem GetItemAtSlot(int slotIndex, out int itemKey)
    {
        itemKey = -1;
        
        if (slotIndex < 0 || slotIndex > _numSlots)
            return InventoryItem.Empty;

        foreach (KeyValuePair<int, ContainedItem> item in containedItems)
        {
            for (int x = 0; x < _slotWidth; x++)
            {
                for (int y = 0; y < _slotHeight; y++)
                {
                    if (item.Value.topLeft.x == slotIndex * _slotWidth + x && item.Value.topLeft.y == y)
                    {
                        itemKey = item.Key;
                        return item.Value.inventoryItem;
                    }
                }
            }
        }

        return InventoryItem.Empty;
    }
}
