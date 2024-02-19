using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointWithInputsUI : InspectPointUIElement
{
    [SerializeField] protected ItemOptionUI itemOptionPrefab;
    [SerializeField] private Transform itemOptionParent;

    private List<ItemOptionUI> itemOptions = new List<ItemOptionUI>();

    public void AddItemOptionUI(StoredItemID item, ItemOptionUI.GenericDelegate<StoredItemID> callBack)
    {
        ItemOptionUI itemOption = Instantiate(itemOptionPrefab, itemOptionParent);
        itemOption.SetItemOption(item, callBack);
        itemOptions.Add(itemOption);
    }

    public void RemoveItemOptionUI(StoredItemID item)
    {
        foreach (ItemOptionUI itemOption in itemOptions)
        {
            if (itemOption.AssociatedItem == item)
            {
                itemOptions.Remove(itemOption);
                Destroy(itemOption.gameObject);
                return;
            }
        }
    }
}
