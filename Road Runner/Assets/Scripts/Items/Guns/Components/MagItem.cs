using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalItemDictionary;
public class MagItem : UseableItem
{
    public void TryAddRoundToMag()
    {
        AddToCounter(ItemID.Bullet_556, 1);
    }
}
