using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static Inventory;

public class ConnectedInventory
{
    public class ContainedItem
    {
        public InventoryItem inventoryItem;
        public Vector2Int topLeft;
        public Vector2Int inventoryDimensions;
    }

    protected int _width;
    protected int _height;

    protected int localKey;

    protected bool[,] inventorySlots;
    protected Dictionary<int, ContainedItem> containedItems;

    public ConnectedInventory(Vector2Int dimensions)
    {
        _width = dimensions.x;
        _height = dimensions.y;

        InitializeInventory();
    }

    protected void InitializeInventory()
    {
        inventorySlots = new bool[_width, _height];
        containedItems = new Dictionary<int, ContainedItem>();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                inventorySlots[x, y] = false; // Don't need to call set item because start is run on every inventory accross the server
            }
        }
    }

    public bool IsAreaAvailable(Vector2Int topLeft, Vector2Int invetoryDimensions)
    {
        if (invetoryDimensions.x < 0 || invetoryDimensions.y < 0)
        {
            Debug.LogError("Inventory dimensions cannot be negative");
            return false;
        }

        if (topLeft.x < 0 || topLeft.x + invetoryDimensions.x > _width || topLeft.y < 0 || topLeft.y + invetoryDimensions.y > _height)
        {
            return false;
        }

        for (int x = 0; x < invetoryDimensions.x; x++)
        {
            for (int y = 0; y < invetoryDimensions.y; y++)
            {
                if (inventorySlots[topLeft.x + x, topLeft.y + y])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool TryFitItem(InventoryItem inventoryItem, out int containedItemKey, out Vector2Int topLeft)
    {
        ItemSO itemSO = Inventory.ItemSODictionary[inventoryItem];
        Vector2Int dimensions = itemSO.InInventoryDimensions;

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (IsAreaAvailable(new Vector2Int(x, y), dimensions))
                {
                    topLeft = new Vector2Int(x, y);
                    containedItemKey = AddItem(inventoryItem, new Vector2Int(x, y), dimensions);         
                    return true;
                }
            }
        }

        topLeft = new Vector2Int(-1, -1);
        containedItemKey = -1;
        return false;
    }

    public int AddItem(InventoryItem inventoryItem, Vector2Int topLeft, Vector2Int dimensions)
    {
        if (!IsAreaAvailable(topLeft, dimensions))
        {
            Debug.LogError("Cannot add item to inventory, area is not available");
            return -1;
        }

        for (int xi = 0; xi < dimensions.x; xi++)
        {
            for (int yi = 0; yi < dimensions.y; yi++)
            {
                int x = topLeft.x + xi;
                int y = topLeft.y + yi;

                inventorySlots[x, y] = true;

            }
        }

        int newItemKey = AddItemToList(inventoryItem, topLeft, dimensions);

        return newItemKey;
    }

    public InventoryItem RemoveItem(int containedItemKey)
    {
        if (containedItems.ContainsKey(containedItemKey))
        {
            ContainedItem item = containedItems[containedItemKey];
            for (int x = 0; x < item.inventoryDimensions.x; x++)
            {
                for (int y = 0; y < item.inventoryDimensions.y; y++)
                {
                    inventorySlots[item.topLeft.x + x, item.topLeft.y + y] = false;
                }
            }

            containedItems.Remove(containedItemKey);
            return item.inventoryItem;
        }

        return InventoryItem.Empty;
    }
    protected int AddItemToList(InventoryItem inventoryItem, Vector2Int topLeft, Vector2Int dimensions)
    {
        ContainedItem newContainedItem = new ContainedItem { inventoryItem = inventoryItem, topLeft = topLeft, inventoryDimensions = dimensions };
        int uniqueKey = GetAvailableItemKey();
        containedItems.Add(uniqueKey, newContainedItem);

        return uniqueKey;
    }

    public Vector2Int GetDimensions()
    {
        return new Vector2Int(_width, _height);
    }

    public void SetLocalKey(int key)
    {
        localKey = key;
    }

    public int GetLocalKey()
    {
        return localKey;
    }

    public ContainedItem GetContainedItem(int containedItemKey)
    {
        if (containedItems.ContainsKey(containedItemKey))
        {
            return containedItems[containedItemKey];
        }

        return null;
    }

    public Dictionary<int, ContainedItem> GetAndClearItems()
    {
        Dictionary<int, ContainedItem> items = new Dictionary<int, ContainedItem>(containedItems);

        containedItems.Clear();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                inventorySlots[x, y] = false; 
            }
        }
        
        return items;
    }

    private int GetAvailableItemKey()
    {
        for (int i = 0; i < 200; i++)
        {
            if (!containedItems.ContainsKey(i))
                return i;
        }

        Debug.LogError("Dude wtf there are more than 200 contained items stop it rn. Also you just broke my inventory system");
        return -1;
    }

    // Set of methods for setting items accross the server if required
    // Going to save these for later, probably will only be needed in a extension of this class for external inventories
    // Yeah I can literally just make a public inventory class that extends this one and then just add the rpcs to that
    // These used to be a bool in this class to make it public FYI     

    /*    [ServerRpc(RequireOwnership = false)]
        private void SetItemServerRpc(int x, int y, InventoryItem inventoryItem)
        {
            SetItemClientRpc(x, y, inventoryItem);
        }

        [ClientRpc]
        private void SetItemClientRpc(int x, int y, InventoryItem inventoryItem)
        {
            inventorySlots[x, y] = inventoryItem;
        }*/
}
