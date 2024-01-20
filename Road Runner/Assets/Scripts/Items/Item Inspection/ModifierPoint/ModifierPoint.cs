using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalItemDictionary;
using static ConnectedInventory;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

[System.Serializable]
public class ItemSelectEvent : UnityEngine.Events.UnityEvent<StoredItemID, int> 
{ 
}

public class ModifierPoint : InspectPoint
{
    [SerializeField] private AllowedModificationsSO allowedModificationsSO;
    [SerializeField] private ItemSelectEvent itemSelectEvent = new ItemSelectEvent();

    [SerializeField] private int modificationSlotIndex;

    private StoredItemID[] _itemsThatFit;
    public StoredItemID[] ItemsThatFit
    {
        get { return _itemsThatFit; }
    }

    public override InspectPointUIElement CreateInspectHUDElement(Transform hudTransform)
    {
        ItemID[] allowedModifications = allowedModificationsSO.AllowedModifications;
        List<StoredItemID> itemSIIDs = new List<StoredItemID>();

        for (int i = 0; i < allowedModifications.Length; i++)
        {
            ItemID itemID = allowedModifications[i];
            StoredItemID[] storedItemIDs = Inventory.Instance.GetAllItemsOfType(itemID);

            foreach (StoredItemID storedItemID in storedItemIDs)
                itemSIIDs.Add(storedItemID);
        }
        _itemsThatFit = itemSIIDs.ToArray();

        return base.CreateInspectHUDElement(hudTransform);
    }

    public void SelectOption(StoredItemID item)
    {
        itemSelectEvent.Invoke(item, modificationSlotIndex);
    }
}