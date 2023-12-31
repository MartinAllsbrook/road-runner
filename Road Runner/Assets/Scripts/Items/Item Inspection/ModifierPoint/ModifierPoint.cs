using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;
using static ConnectedInventory;
using System.Runtime.CompilerServices;

[System.Serializable]
public class ItemSelectEvent : UnityEngine.Events.UnityEvent<ContainedItem> 
{ 
}

public class ModifierPoint : InspectPoint
{
    [SerializeField] private InventoryItem itemTypeThatFits;
    [SerializeField] private ItemSelectEvent itemSelectEvent = new ItemSelectEvent();

    private ContainedItem[] _itemsThatFit;
    public ContainedItem[] ItemsThatFit
    {
        get { return _itemsThatFit; }
    }

    public override InspectPointUIElement CreateInspectHUDElement(Transform hudTransform)
    {
        _itemsThatFit = Inventory.Instance.FindInvetoryObjectsOfTypes(itemTypeThatFits);

        return base.CreateInspectHUDElement(hudTransform);
    }

    public void SelectOption(ContainedItem item)
    {
        itemSelectEvent.Invoke(item);
    }
}