using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static Inventory;

// Everything you need to know about a item
// Stored in a dictionary in Invenotry the with itemID as the key
// Once an item is added to the game world it is given a uniqueItemID to track it and any changes made to it
// But this ItemSO class contains info about how to create a new item of this type, and how it can be modified
[Serializable] [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Item Identification")]
    [SerializeField] protected ItemID itemID;

    [Header("UniqueItem Defaults")]
    [SerializeField] protected int maxModifications;
    [SerializeField] protected int maxCounterCount;
    [SerializeField] protected ItemID[] defaultModifications;
    [SerializeField] protected ItemID defaultCounterItem;
    [SerializeField] protected int defaultCounterCount; // Could be max counter count a lot of the time

    [Header("Valid Modifications")]
    [SerializeField] protected ItemID[][] validModificationTypes;
    [SerializeField] protected ItemID[] validCounterTypes;

    [Header("Prefabs")]
    [SerializeField] protected ItemPickup itemPickupPrefab;
    [SerializeField] protected UseableItem useableItemPrefab;
    [SerializeField] protected UniqueItemModel modelPrefab;
    // TODO: Could add [SerializeField] protected GameObject itemModelPrefab; and a default itempickup prefab that just has a collider and a rigidbody and a UniqueItemID

    [Header("Inventory Display")]
    [SerializeField] protected Sprite uiSprite;
    [SerializeField] protected Vector2Int inventoryDimensions = new Vector2Int(1, 1);

    #region Properties
    
    // Item Identification
    public ItemID ItemID 
    { get { return itemID; } }
    
    public int MaxModifications
    { get { return maxModifications; } }

    public int MaxCounterCount
    { get { return maxCounterCount; } }

    public ItemID[] DefaultModifications
    { get { return defaultModifications; } }

    // Prefabs
    public ItemPickup ItemPickupPrefab 
    { get { return itemPickupPrefab; } }
    
    public UseableItem UsableItemPrefab 
    { get { return useableItemPrefab; } }

    public UniqueItemModel ModelPrefab
    { get { return modelPrefab; } }

    // Inventory Display
    public Sprite UISprite 
    { get { return uiSprite; } }
    
    public Vector2Int InInventoryDimensions 
    { get { return inventoryDimensions; } }
    
    #endregion
}
