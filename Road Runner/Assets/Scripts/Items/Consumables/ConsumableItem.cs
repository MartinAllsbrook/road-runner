 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableItem : UseableItem
{
    protected bool used = false;

    public override void OnUseItemInput()
    {
        base.OnUseItemInput();
        if (!used)
        {
            used = true;
            Inventory.Instance.RemoveUsing();
            Inventory.Instance.ConsumeItem(itemKey);
        }
    }
}