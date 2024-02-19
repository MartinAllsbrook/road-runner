using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GlobalItemDictionary;

public class CraftPointUIElement : PointWithInputsUI
{
    [SerializeField] private Button craftButton;

    [SerializeField] private RectTransform inputedItemParent;

    List<ItemOptionUI> inputedItems = new List<ItemOptionUI>();

    public delegate void GenericDelegate();

    public void InitializeCraftUI(GenericDelegate callBack)
    {
        craftButton.onClick.AddListener(() => 
        { 
            callBack.Invoke();
        });
    }

    public void AddItemToInputUI(StoredItemID item, ItemOptionUI.GenericDelegate<StoredItemID> callBack)
    {
        ItemOptionUI itemUI = Instantiate(itemOptionPrefab, inputedItemParent);
        itemUI.SetItemOption(item, callBack);
        inputedItems.Add(itemUI);
    }

    public void RemoveItemFromInputUI(StoredItemID item)
    {
        foreach (ItemOptionUI itemUI in inputedItems)
        {
            if (itemUI.AssociatedItem == item)
            {
                inputedItems.Remove(itemUI);
                Destroy(itemUI.gameObject);
                return;
            }
        }
    }
}
