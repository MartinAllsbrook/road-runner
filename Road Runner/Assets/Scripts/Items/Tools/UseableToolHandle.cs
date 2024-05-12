using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseableToolHandle : UseableItem
{
    private float[] damages = new float[4];
    public override void ModifyUniqueItemID(StoredItemID modificationSIID, int modificationSlot)
    {
        base.ModifyUniqueItemID(modificationSIID, modificationSlot);
    }
}
