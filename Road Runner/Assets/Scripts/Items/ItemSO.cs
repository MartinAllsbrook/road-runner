using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static Inventory;

[Serializable] [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Item Stuff")]
    [SerializeField] protected InventoryItem inventoryItem; // TODO: Rename this to inventoryItemEnum?
    public InventoryItem InventoryItem { get { return inventoryItem; } }

    [SerializeField] protected GameObject itemPickupPrefab;
    public GameObject ItemPickupPrefab { get { return itemPickupPrefab; } }
    
    [SerializeField] protected GameObject itemPrefab; // TODO: Rename this to usableItemPrefab
    public GameObject UsableItemPrefab { get { return itemPrefab; } }
    
    [Header("Inventory Display Stuff")]
    [SerializeField] protected Sprite uiSprite;
    public Sprite UISprite { get { return uiSprite; } }

    [SerializeField] protected Vector2Int inventoryDimensions;
    public Vector2Int InInventoryDimensions { get { return inventoryDimensions; } }
}
