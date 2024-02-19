using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalItemDictionary;

[Serializable]
[CreateAssetMenu(fileName = "NewCraftingRecipieList", menuName = "Items/CraftingRecipieList")]
public class CraftingRecipiesSO : ScriptableObject
{
    [SerializeField] private CraftingRecipie[] craftingRecipies;

    public CraftingRecipie[] CraftingRecipies
    {
        get { return craftingRecipies; }
    }
}

[Serializable]
public class CraftingRecipie
{
    [SerializeField] private ItemID[] requiredItems;
    [SerializeField] private ItemID resultItem;

    public ItemID[] RequiredItems
    {
        get { return requiredItems; }
    }

    public ItemID ResultItem
    {
        get { return resultItem; }
    }
}
