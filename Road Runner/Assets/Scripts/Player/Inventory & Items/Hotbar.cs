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

    public StoredItemID GetItemAtSlot(int slotIndex)
    {        
        if (slotIndex < 0 || slotIndex > _numSlots)
            return new StoredItemID();

        foreach (KeyValuePair<int, StoredItemID> storedItem in containedItems)
        {
            for (int x = 0; x < _slotWidth; x++)
            {
                for (int y = 0; y < _slotHeight; y++)
                {
                    if (storedItem.Value.TopLeft.x == slotIndex * _slotWidth + x && storedItem.Value.TopLeft.y == y)
                    {
                        return storedItem.Value;
                    }
                }
            }
        }

        return new StoredItemID();
    }
}
