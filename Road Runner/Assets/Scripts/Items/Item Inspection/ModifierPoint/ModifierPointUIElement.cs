using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalItemDictionary;
using static ConnectedInventory;

public class ModifierPointUIElement : PointWithInputsUI
{
/*    public override void GenericSet<T>(T point)
    {
        if (!_set)
        {
            base.GenericSet(point);
        }

        ModifierPoint modifierPoint = point as ModifierPoint;

        StoredItemID[] itemOptions = modifierPoint.ItemsThatFit;
        foreach (StoredItemID item in itemOptions)
        {
            Sprite sprite = ItemSODictionary[item.UniqueItemID.BaseItemID].UISprite;
            SpawnItemOptionUI(item, modifierPoint);
        }
    }*/

}
