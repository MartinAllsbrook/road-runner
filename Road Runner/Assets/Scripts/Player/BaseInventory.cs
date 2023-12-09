using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseInventory : Inventory
{
    public static BaseInventory Instance;

    [Header("World Interaction")]
    [SerializeField] private KeyCode pickupItemKey = KeyCode.F;
    [SerializeField] private float maxItemPickupDistance;
    [SerializeField] private LayerMask isItemPickup;
    [SerializeField] private LayerMask isVehicle;
    [SerializeField] private Inventory droppedItemBag;
    
    private InventoryItem usingItem; // The item being used by the player
    private UseableItemController useableItemController;

    private InventoryItem inventoryHand; // The item being moved around the inventory

    private VehicleInteractionController vehicle;

    private Transform mainCamera;

    private Dictionary<int, Inventory> connectedInventories;


    protected override void Start()
    {
        useableItemController = GetComponent<UseableItemController>();
        
        base.Start();

        if (!IsOwner)
            return;

        if (Instance == null)
            Instance = this;

        connectedInventories = new Dictionary<int, Inventory>();

        inventoryHand = InventoryItem.Empty;

        mainCamera = Camera.main.transform;

        itemSoDictionary = ItemSpawner.ItemDictionary;

        connectedInventories.Add(0, this);
        InventoryDisplay.Instance.CreateBaseInventoryDisplay(width, height, this);
    }

    void Update()
    {
        if (!IsOwner)
            return;

        GetPickupItemInput();
    }

    private void AddConnectedInventory(Inventory inventoryToConnect)
    {
        int inventoryKey = GetAvailableIndex();
        connectedInventories.Add(inventoryKey, inventoryToConnect);
        inventoryToConnect.SetLocalKey(inventoryKey);

        Vector2Int invetoryDimensions = inventoryToConnect.GetDimensions();
        InventoryDisplay.Instance.CreateInventoryDisplay(inventoryKey, invetoryDimensions.x, invetoryDimensions.y);
        
        for (int x = 0; x < invetoryDimensions.x; ++x)
        {
            for (int y = 0; y < invetoryDimensions.y; ++y)
            {
                InventoryDisplay.Instance.UpdateItemSlot(inventoryKey, x, y, inventoryToConnect.GetItemAt(x, y));   
            }
        }
    }

    private int GetAvailableIndex()
    {
        for (int i = 0; i < 100; i++)
        {
            if (!connectedInventories.ContainsKey(i))
                return i;
        }

        Debug.LogError("Dude wtf there are more than 100 inventories stop it rn");
        return -1;
    }

    private void RemoveConnectedInventory(int key)
    {
        InventoryDisplay.Instance.RemoveInvetoryDisplay(key);
        connectedInventories.Remove(key);
    }

    private void GetPickupItemInput()
    {
        if (PlayerSpawner.localPlayerSpawner.Paused)
            return;

        if (Input.GetKeyDown(pickupItemKey))
        {
            if (vehicle != null)
            {
                RemoveConnectedInventory(vehicle.GetInvetory().GetLocalKey());
                vehicle.ExitVehicle(GetNetworkObject(NetworkObjectId));
                vehicle = null;
            }
            else
            {
                RaycastForPickups();
            }
        }
    }

    private void RaycastForPickups()
    {
        Ray ray = new Ray(mainCamera.position, mainCamera.forward);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, maxItemPickupDistance, isItemPickup))
        {
            if(raycastHit.transform.CompareTag("Test Add Inventory"))
            {
                Inventory invetoryToAdd = raycastHit.transform.GetComponent<Inventory>();
                AddConnectedInventory(invetoryToAdd);
                return;
            }

            ItemPickup itemPickup = raycastHit.transform.GetComponent<ItemPickup>();
            TryPickUpItem(itemPickup);
        }

        if (Physics.Raycast(ray, out raycastHit, maxItemPickupDistance, isVehicle))
        {
            VehicleInteractionController vehicleInteractionController = raycastHit.transform.GetComponent<VehicleInteractionController>();

            EnterVehicle(vehicleInteractionController);
        }
    }

    private void EnterVehicle(VehicleInteractionController vehicleInteractionController)
    {
        vehicle = vehicleInteractionController;
        vehicleInteractionController.EnterVehicle(GetNetworkObject(NetworkObjectId));

        AddConnectedInventory(vehicleInteractionController.GetInvetory());
    }

    private void TryPickUpItem(ItemPickup itemPickup)
    {
        if (TryAddItem(itemPickup.GetScriptableObject().GetInventoryItem()))
        {
            itemPickup.RemoveFromWorld();
        }
    }

    private bool TryAddItem(InventoryItem inventoryItem)
    {
        var keys = connectedInventories.Keys;

        foreach (var key in keys)
        {
            if (connectedInventories[key].AddItem(key, inventoryItem))
                return true;
        }

        return false;
    }

    public InventoryItem ClickOnSlot(int inventoryIndex, int x, int y, out InventoryItem handItem)
    {
        Inventory inventory = connectedInventories[inventoryIndex];

        if (inventoryHand == InventoryItem.Empty)
        {
            if (inventory.IsSlotFree(x, y))
            {
                handItem = InventoryItem.Empty;
                return InventoryItem.Empty;
            }

            inventoryHand = inventory.RemoveItem(x, y);

            handItem = inventoryHand;
            return InventoryItem.Empty;
        }

        InventoryItem tempItemHolder = inventoryHand;

        if (!inventory.IsSlotFree(x, y))
        {
            inventoryHand = inventory.RemoveItem(x, y);
            inventory.AddItem(x, y, tempItemHolder);

            handItem = inventoryHand;
            return tempItemHolder;
        }

        inventoryHand = InventoryItem.Empty;
        inventory.AddItem(x, y, tempItemHolder);

        handItem = inventoryHand;
        return tempItemHolder;

    }

    public InventoryItem SetUsing(out InventoryItem handItem)
    {
        InventoryItem tempItemHolder = usingItem;
        usingItem = inventoryHand;
        inventoryHand = tempItemHolder;

        HoldItemServerRpc(usingItem);

        handItem = inventoryHand;
        return usingItem;
    }

    public void RemoveUsing()
    {
        usingItem = InventoryItem.Empty;

        InventoryDisplay.Instance.ResetUsingSlot();

        HoldItemServerRpc(usingItem);
    }

    public void DropItem()
    {
        DropItemServerRpc(inventoryHand);
        inventoryHand = InventoryItem.Empty;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(InventoryItem item)
    {
        GameObject itemGameObject = Instantiate(itemSoDictionary[item].GetItemPickupPrefab(), transform.position + Vector3.up * 2.5f, new Quaternion(0, 0, 0, 0));

        NetworkObject itemNetworkObject = itemGameObject.GetComponent<NetworkObject>();
        itemNetworkObject.Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HoldItemServerRpc(InventoryItem item)
    {
        HoldItemClientRpc(item);
    }

    [ClientRpc]
    private void HoldItemClientRpc(InventoryItem item)
    {
        useableItemController.SetItem(itemSoDictionary[item]);
    }

    public void DropAllItems()
    {
        if (inventoryHand != InventoryItem.Empty)
        {
            DropItemServerRpc(inventoryHand);
            inventoryHand = InventoryItem.Empty;
        }

        if (usingItem != InventoryItem.Empty)
        {
            DropItemServerRpc(usingItem);
            RemoveUsing();
        }

        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(inventory[i, j] != InventoryItem.Empty)
                {
                    DropItemServerRpc(inventory[i, j]);
                    RemoveItem(i, j);
                }
                
                InventoryDisplay.Instance.UpdateItemSlot(0, i, j, InventoryItem.Empty);
            }
        }
    }
}
