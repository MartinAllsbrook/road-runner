using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class Hotbar : ConnectedInventory
{
    int _numSlots;
    int _slotWidth;
    int _slotHeight;

    public Hotbar(int numSlots, int slotWidth, int slotHeight) : base(numSlots * slotWidth, slotHeight)
    {
        _numSlots = numSlots;
        _slotWidth = slotWidth;
        _slotHeight = slotHeight;
        InitializeInventory();
    }

    public InventoryItem GetItemAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > _numSlots)
            return InventoryItem.Empty;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            for (int x = 0; x < _slotWidth; x++)
            {
                for (int y = 0; y < _slotHeight; y++)
                {
                    if (inventoryItems[i].topLeft.x == slotIndex * _slotWidth && inventoryItems[i].topLeft.y == y)
                        return inventoryItems[i].inventoryItem;
                }
            }
        }

        return InventoryItem.Empty;
    }
}
