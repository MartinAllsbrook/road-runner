using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AllItems", menuName = "ScriptableObjects/Items/AllItemsList")]
public class AllItemsSO : ScriptableObject
{
    [Tooltip("Items that are not part of a list. Reccomended to organize items into lists by type instead")]
    [SerializeField] protected ItemSO[] individualItems;
    [Tooltip("Lists of items, reccomended to be organized by type")]
    [SerializeField] protected ItemListSO[] itemSOLists;

    public ItemSO[] GetAllItemSOs()
    {
        List<ItemSO> allItems = new List<ItemSO>();
        foreach (ItemSO item in individualItems)
        {
            allItems.Add(item);
        }
        foreach (ItemListSO list in itemSOLists)
        {
            foreach (ItemSO item in list.ItemSOs)
            {
                allItems.Add(item);
            }
        }
        return allItems.ToArray();
    }
}