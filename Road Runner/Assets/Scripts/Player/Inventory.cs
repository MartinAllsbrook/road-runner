using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    [SerializeField] protected int width;
    [SerializeField] protected int height;

    [SerializeField] private bool publicInventory = false;

    private int localKey;

    protected static Dictionary<InventoryItem, ItemSO> itemSoDictionary;

    protected InventoryItem[,] inventory;
    public enum InventoryItem
    {
        Empty,

        GunAk74,
        GunAug,
        GunBoltAction,
        GunDeagle,
        GunFamas,
        GunGlock,
        GunHunter,
        GunM4A1,
        GunMac10,
        GunMp5,
        GunPumpAction,
        GunRevolver,
        GunTrippleBarrel,
        GunUspS,
        GunBenneliM4,
        GunM4_8,
        GunM107,
        GunM1911,
        GunRpg7,
        GunUzi,
        GunM249,

        Ammo9mm,

        ConsumableApple,
        ConsumableWaterBottle,
        ConsumableBeans,
        ConsumableMedkit,
        ConsumablePills,

        Flare,
        Batteries,
        Flashlight,
        GasCan,
        Knife,
        Rope,
        Lighter
    }

    protected virtual void Start()
    {
        inventory = new InventoryItem[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                inventory[x, y] = InventoryItem.Empty; // Don't need to call set item because start is run on every inventory accross the server
            }
        }
    }

    public bool IsSlotFree(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return false;
        
        return inventory[x, y] == InventoryItem.Empty;
    }

    public void AddItem(int x, int y, InventoryItem inventoryItem)
    {
        SetItem(x, y, inventoryItem);
    }

    public InventoryItem RemoveItem(int x, int y)
    {
        InventoryItem tempItemHolder = inventory[x, y];
        SetItem(x, y, InventoryItem.Empty);
        return tempItemHolder;
    }

    public bool AddItem(int invetoryKey, InventoryItem inventoryItem)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (inventory[x,y] == InventoryItem.Empty)
                {
                    SetItem(x, y, inventoryItem);
                    InventoryDisplay.Instance.UpdateItemSlot(invetoryKey, x, y, inventoryItem);

                    return true;
                }
            }
        }

        return false;
    }

    public InventoryItem GetItemAt(int x, int y)
    {
        return inventory[x, y];
    }

    public Vector2Int GetDimensions()
    {
        return new Vector2Int(width, height);
    }

    public void SetLocalKey(int key)
    {
        localKey = key;
    }

    public int GetLocalKey()
    {
        return localKey;
    }

    // Set of methods for setting items accross the server if required
    private void SetItem(int x, int y, InventoryItem inventoryItem)
    {
        if (!publicInventory)
            inventory[x, y] = inventoryItem;
        else
            SetItemServerRpc(x, y, inventoryItem);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetItemServerRpc(int x, int y, InventoryItem inventoryItem) 
    { 
        SetItemClientRpc(x, y, inventoryItem);
    }

    [ClientRpc]
    private void SetItemClientRpc(int x, int y, InventoryItem inventoryItem)
    {
        inventory[x, y] = inventoryItem;
    }
}
