using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalItemDictionary;
using static ConnectedInventory;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

[System.Serializable]
public class ItemSelectEvent : UnityEngine.Events.UnityEvent<StoredItemID, int> { }

public class ModifierPoint : PointWithInputs
{
    [SerializeField] private ItemSelectEvent itemSelectEvent = new ItemSelectEvent();

    [SerializeField] private int modificationSlotIndex;

    public override void SelectOption(StoredItemID item)
    {
        itemSelectEvent.Invoke(item, modificationSlotIndex);
    }
}