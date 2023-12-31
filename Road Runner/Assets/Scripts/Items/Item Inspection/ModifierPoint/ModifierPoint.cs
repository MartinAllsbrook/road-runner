using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;
using static ConnectedInventory;
using System.Runtime.CompilerServices;

[System.Serializable]
public class ItemSelectEvent : UnityEngine.Events.UnityEvent<StoredItemID> 
{ 
}

public class ModifierPoint : InspectPoint
{
    [SerializeField] private ItemID itemTypeThatFits;
    [SerializeField] private ItemSelectEvent itemSelectEvent = new ItemSelectEvent();

    private StoredItemID[] _itemsThatFit;
    public StoredItemID[] ItemsThatFit
    {
        get { return _itemsThatFit; }
    }

    public override InspectPointUIElement CreateInspectHUDElement(Transform hudTransform)
    {
        _itemsThatFit = Inventory.Instance.FindInvetoryObjectsOfTypes(itemTypeThatFits);

        return base.CreateInspectHUDElement(hudTransform);
    }

    public void SelectOption(StoredItemID item)
    {
        itemSelectEvent.Invoke(item);
    }
}