using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagItem : UseableItem
{
    public void TryAddRoundToMag()
    {
        AddToCounter(Inventory.ItemID.Bullet_556, 1);
    }
}
