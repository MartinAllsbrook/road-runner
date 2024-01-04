using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagItem : UseableItem
{
    public void TryAddRoundToMag()
    {
        uniqueItemID.TryAddItemToCounter(Inventory.ItemID.Bullet_556, 1); // Edit copy in hands

        if (isOwner)
            Inventory.Instance.UpdateUniqueItem(inventoryKey, itemKey, uniqueItemID); // Update copy in inventory
    }
}
