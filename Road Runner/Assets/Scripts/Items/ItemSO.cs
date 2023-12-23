using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[Serializable] [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
    [SerializeField] protected Inventory.InventoryItem inventoryItem;
    [SerializeField] protected GameObject itemPickupPrefab;
    [SerializeField] protected GameObject itemPrefab;
    [SerializeField] protected Sprite uiSprite;

    [Header("Inventory Display Stuff")]
    [SerializeField] protected Vector2Int inventoryDimensions;

    public Inventory.InventoryItem GetInventoryItem()
    {
        return inventoryItem;
    }
    public GameObject GetItemPickupPrefab()
    {
        return itemPickupPrefab;
    }

    public GameObject GetItemPrefab() 
    {
        return itemPrefab;
    }

    public Sprite GetSprite()
    {
        return uiSprite;
    }

    public Vector2Int GetInventoryDimensions()
    {
        return inventoryDimensions;
    }
}
