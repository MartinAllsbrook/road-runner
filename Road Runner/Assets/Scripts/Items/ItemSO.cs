using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable] [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
    [SerializeField] protected Inventory.InventoryItem inventoryItem;
    [SerializeField] protected GameObject itemPickupPrefab;
    [SerializeField] protected GameObject itemPrefab;
    [SerializeField] protected Sprite uiSprite;
    [SerializeField] protected bool isGun;

    public Sprite GetSprite()
    {
        return uiSprite;
    }
    
    public bool GetIsGun()
    {
        return isGun;
    }
    
    public Inventory.InventoryItem GetInventoryItem()
    {
        return inventoryItem;
    }

    public GameObject GetItemPrefab() 
    {
        return itemPrefab;
    }

    public GameObject GetItemPickupPrefab()
    {
        return itemPickupPrefab;
    }
    
    public NetworkObject GetItemPickupNetworkObject()
    {
        return itemPickupPrefab.GetComponent<NetworkObject>();
    }
}
