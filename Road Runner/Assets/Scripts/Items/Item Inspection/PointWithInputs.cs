using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalItemDictionary;

public class PointWithInputs : InspectPoint
{
    [SerializeField] protected AllowedItemInputsSO allowedModificationsSO;

    private PointWithInputsUI pointWithInputsUI;

    private StoredItemID[] _itemsThatFit;

    public virtual void SelectOption(StoredItemID item)
    {
        pointWithInputsUI.RemoveItemOptionUI(item);
    }

    public override InspectPointUIElement CreateInspectHUDElement(Transform hudTransform)
    {
        // Find and display all items that fit the allowed modifications
        _itemsThatFit = FindItemsThatFit(allowedModificationsSO.AllowedModifications);

        pointWithInputsUI = (PointWithInputsUI)base.CreateInspectHUDElement(hudTransform);
        foreach (StoredItemID item in _itemsThatFit)
        {
            pointWithInputsUI.AddItemOptionUI(item, SelectOption);
        }

        return pointWithInputsUI;
    }

    private StoredItemID[] FindItemsThatFit(ItemID[] allowedModifications)
    {
        List<StoredItemID> itemsThatFit = new List<StoredItemID>();

        for (int i = 0; i < allowedModifications.Length; i++)
        {
            ItemID itemID = allowedModifications[i];
            StoredItemID[] storedItemIDs = Inventory.Instance.GetAllItemsOfType(itemID);

            foreach (StoredItemID storedItemID in storedItemIDs)
                itemsThatFit.Add(storedItemID);
        }

        return itemsThatFit.ToArray();
    }
}
