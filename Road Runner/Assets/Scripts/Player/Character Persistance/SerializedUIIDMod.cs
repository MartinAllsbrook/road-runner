using System;
using UnityEngine;
using static GlobalItemDictionary;

[Serializable]
public class SerializedUIIDMod
{
    [SerializeField] public ItemID BaseItemID; // Base item of the modification
    [SerializeField] public int[] ModPath; // Path to the modification in the base item's modification tree

    [SerializeField] public int NumModSlots; // Number of modification slots this item has

    [SerializeField] public ItemID CounterItem; 
    [SerializeField] public int CounterCount; 

    public SerializedUIIDMod(ItemID baseItemID, int[] modPath, int numModSlots, ItemID counterItem, int counterCount)
    {
        BaseItemID = baseItemID;
        ModPath = modPath;
        CounterItem = counterItem;
        CounterCount = counterCount;
        NumModSlots = numModSlots;
    }
}
