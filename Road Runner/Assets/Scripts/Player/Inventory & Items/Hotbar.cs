using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Inventory;

public class Hotbar : ConnectedInventory
{
    int _numSlots;
    int _slotWidth;
    int _slotHeight;

    int _lastCheckedSlot = -1;
    int _lastCheckedSlotX = -1;
    int _lastCheckedSlotY = -1;

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

        Vector2Int start = new Vector2Int(0, 0);
        if (_lastCheckedSlot == slotIndex)
        {
            if (_lastCheckedSlotX < _slotHeight - 1)
                start = new Vector2Int(_lastCheckedSlotX + 1, _lastCheckedSlotY);
            else
                start = new Vector2Int(0, _lastCheckedSlotY + 1);
        }

        for (int xI = start.x; xI < _slotWidth; xI++)
        {
            for (int yI = start.y; yI < _slotHeight; yI++)
            {
                int x = xI + slotIndex * _slotWidth;
                int y = yI;

                Vector2Int cellPosition = new Vector2Int(x, y);

                StoredItemID itemAtCell = GetItemOverlappingCell(cellPosition);

                if ((int) itemAtCell.UniqueItemID.BaseItemID != -1)
                {
                    _lastCheckedSlot = slotIndex;
                    _lastCheckedSlotX = xI;
                    _lastCheckedSlotY = yI;
                    return itemAtCell;
                }
            }
        }

        return new StoredItemID();
    }

    private StoredItemID GetItemOverlappingCell(Vector2Int cellCoordinates)
    {
        foreach (KeyValuePair<int, StoredItemID> storedItem in containedItems)
        {
            Vector2Int topLeft = storedItem.Value.TopLeft;
            Vector2Int dimensions = storedItem.Value.UniqueItemID.Dimensions;

            for (int xI = 0; xI < dimensions.x; xI++)
            {
                for (int yI = 0; yI < dimensions.y; yI++)
                {
                    int x = xI + topLeft.x;
                    int y = yI + topLeft.y;

                    if (x == cellCoordinates.x && y == cellCoordinates.y)
                    {
                        return storedItem.Value;
                    }
                }
            }
        }

        return new StoredItemID();
    }
}
