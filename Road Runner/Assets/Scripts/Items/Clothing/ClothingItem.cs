using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClothingSlotUI;
using static GlobalItemDictionary;

[Serializable]
public class ClothingData
{
    [SerializeField] private ItemID baseItemID;
    [SerializeField] private ClothingSlot clothingSlot;
    [SerializeField] private Vector2Int clothingInventoryDimensions;
    [SerializeField] private GameObject clothingPrefab;

    [Header("Resistance")]
    [SerializeField] private int headResitance;
    [SerializeField] private int torsoResistance;
    [SerializeField] private int armsResistance;
    [SerializeField] private int legsResistance;

    public ItemID BaseItemID { get { return baseItemID; } }
    public ClothingSlot ClothingSlot { get { return clothingSlot; } }
    public Vector2Int ClothingInventoryDimensions { get { return clothingInventoryDimensions; } }
    public GameObject ClothingPrefab { get { return clothingPrefab; } }
    public int[] Resistances 
    { 
        get
        {
            int[] resistances = new int[] {0, headResitance, torsoResistance, armsResistance, legsResistance};
            return resistances;
        }
    }
}

public class ClothingItem : ConsumableItem
{
    // DO we need an itemSO reference here?

    [SerializeField] private ClothingData clothingData;

    public override void OnUseItemInput()
    {
        if (!used)
        {
            Inventory.Instance.UpdateClothingInventory(clothingData);
        }
        base.OnUseItemInput();
    }
}
