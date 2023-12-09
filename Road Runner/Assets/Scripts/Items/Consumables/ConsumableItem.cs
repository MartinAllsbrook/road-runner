 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableItem : UseableItem
{
    private bool used = false;

    public override void UseItem()
    {
        base.UseItem();
        if (!used)
        {
            used = true;
            BaseInventory.Instance.RemoveUsing();
        }
    }
}