 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableItem : UseableItem
{
    private bool used = false;

    public override void OnUseItemInput()
    {
        if (!used)
        {
            used = true;
            BaseInventory.Instance.RemoveUsing();
        }
    }
}