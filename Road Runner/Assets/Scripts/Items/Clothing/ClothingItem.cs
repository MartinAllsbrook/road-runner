using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingItem : ConsumableItem
{
    [SerializeField] private ClothingItemSO clothingItemSO;

    public override void OnUseItemInput()
    {
        base.OnUseItemInput();
        Inventory.Instance.UpdateClothingInventory(clothingItemSO);
    }
}
