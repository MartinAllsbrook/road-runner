using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingItem : ConsumableItem
{
    [SerializeField] private ClothingItemSO clothingItemSO;

    public override void OnUseItemInput()
    {
        if (!used)
        {
            Inventory.Instance.UpdateClothingInventory(clothingItemSO);
        }
        base.OnUseItemInput();
    }
}
