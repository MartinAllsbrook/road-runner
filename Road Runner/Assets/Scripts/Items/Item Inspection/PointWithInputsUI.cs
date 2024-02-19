using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointWithInputsUI : InspectPointUIElement
{
    [SerializeField] protected ItemOptionUI itemOptionPrefab;
    [SerializeField] private Transform itemOptionParent;

    public void SpawnItemOptionUI(StoredItemID item, ItemOptionUI.GenericDelegate<StoredItemID> callBack)
    {
        ItemOptionUI itemOption = Instantiate(itemOptionPrefab, itemOptionParent);
        itemOption.SetItemOption(item, callBack);
    }
}
