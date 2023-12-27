using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotButton : InventoryUIButton
{
    private int _inventoryKey;
    private Vector2Int _slotPosition;

    public void Set(int inventoryKey, Vector2Int slotPosition, int slotWidth)
    {
        _inventoryKey = inventoryKey;
        _slotPosition = slotPosition;

        slotPosition.y = -slotPosition.y;
        StyleRect(slotPosition * slotWidth, Vector2Int.one * slotWidth);

        AddListener();
    }

    protected override void OnClick()
    {
        base.OnClick();

        Inventory.Instance.TryPlaceInSlot(_inventoryKey, _slotPosition);
    }
}
