using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static ClothingSlotUI;
using static GlobalItemDictionary;

[Serializable]
public class CharacterData
{
    public string CName;

    // From player stats
    public float Health;
    public float Food;
    public float Water;

    // Inventory
    public StoredItemID[] StoredItems;
    public ClothingData[] ClothingItems;

    public CharacterData()
    {
        CName = "New Character";
        
        Health = 100;
        Food = 100;
        Water = 100;

        StoredItems = new StoredItemID[0];
        ClothingItems = new ClothingData[0];
    }
}


