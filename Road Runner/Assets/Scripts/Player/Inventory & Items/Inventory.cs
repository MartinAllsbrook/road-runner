using Mono.CSharp;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : NetworkBehaviour
{
    public static Inventory Instance;

    [Header("World Interaction")]
    [SerializeField] private float maxItemPickupDistance;
    [SerializeField] private LayerMask isItemPickup;
    [SerializeField] private LayerMask isVehicle;
    [SerializeField] private Inventory droppedItemBag;

    [Header("Item Refs")]
    [SerializeField] private ItemSO[] itemSos;

    [Header("Hotbar")]
    [SerializeField] private int hotbarSize = 9;
    [SerializeField] private int slotWidth = 2;
    [SerializeField] private int slotHeight = 2;

    private InventoryItem usingItem; // The item being used by the player
    private UseableItemController useableItemController;

    private InventoryItem inventoryHand; // The item being moved around the inventory

    private VehicleInteractionController vehicle;

    private Transform mainCamera;

    private Hotbar hotbar;
    private Dictionary<int, ConnectedInventory> connectedInventories;

    private InventoryUI inventoryUI;

    protected static Dictionary<InventoryItem, ItemSO> itemSoDictionary;
    public static Dictionary<InventoryItem, ItemSO> ItemSODictionary { get { return itemSoDictionary; } }

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

    private void Awake()
    {
        if (!IsOwner)
            return;

        if (Instance == null)
            Instance = this;
    }

    protected void Start()
    {
        useableItemController = GetComponent<UseableItemController>();
        inventoryUI = InventoryUI.Instance;

        if (!IsOwner)
            return;

        // Create hotbar

        if (itemSoDictionary == null)
        {
            itemSoDictionary = new Dictionary<InventoryItem, ItemSO>();
            for (int i = 0; i < itemSos.Length; i++)
                itemSoDictionary.Add(itemSos[i].GetInventoryItem(), itemSos[i]);
        }

        connectedInventories = new Dictionary<int, ConnectedInventory>();

        inventoryHand = InventoryItem.Empty;

        mainCamera = Camera.main.transform;

        CreateHotbar();
    }

    private void CreateHotbar()
    {
        hotbar = new Hotbar(hotbarSize, slotWidth, slotHeight);
        connectedInventories.Add(0, hotbar);

        inventoryUI.CreateHotbarSlotUIs(hotbarSize, slotWidth, slotHeight);
    }

    public void OnItemPickUpInput(InputAction.CallbackContext context)
    {
        if (context.started)
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

    #region Picking up items

    private void RaycastForPickups()
    {
        Ray ray = new Ray(mainCamera.position, mainCamera.forward);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, maxItemPickupDistance, isItemPickup))
        {
            if (raycastHit.transform.CompareTag("Test Add Inventory"))
            {
                ConnectedInventory invetoryToAdd = raycastHit.transform.GetComponent<ConnectedInventory>();
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

    private void TryPickUpItem(ItemPickup itemPickup)
    {
        if (TryFitAnywehere(itemPickup.GetScriptableObject().GetInventoryItem()))
        {
            itemPickup.RemoveFromWorld();
        }
    }

    private bool TryFitAnywehere(InventoryItem inventoryItem)
    {
        var keys = connectedInventories.Keys;

        foreach (var key in keys)
        {
            if (connectedInventories[key].TryFitItem(inventoryItem, out ConnectedInventory.ContainedItem containedItem))
            {
                inventoryUI.AddItemDisplay(itemSoDictionary[inventoryItem], containedItem, key);
                return true;
            }
        }

        return false;
    }
    #endregion

    #region All-inventory Methods
    private void RemoveConnectedInventory(int key)
    {
        inventoryUI.RemoveInventoryDisplay(key);
        connectedInventories.Remove(key);
    }

    private void AddConnectedInventory(ConnectedInventory inventoryToConnect)
    {
        int inventoryKey = GetAvailableIndex();
        connectedInventories.Add(inventoryKey, inventoryToConnect);
        inventoryToConnect.SetLocalKey(inventoryKey);

        Vector2Int invetoryDimensions = inventoryToConnect.GetDimensions();
        //inventoryUI.CreateInventoryDisplay(inventoryKey, invetoryDimensions.x, invetoryDimensions.y);

        for (int x = 0; x < invetoryDimensions.x; ++x)
        {
            for (int y = 0; y < invetoryDimensions.y; ++y)
            {
                //inventoryUI.UpdateItemSlot(inventoryKey, x, y, inventoryToConnect.GetItemAt(x, y));
            }
        }
    }
    /// <summary>
    /// Method called to place an item in a slot
    /// </summary>
    /// <param name="inventoryIndex"></param>
    /// <param name="slot"></param>
    public bool TryPlaceInSlot(int inventoryIndex, Vector2Int slot)
    {
        if (inventoryHand == InventoryItem.Empty)
        {
            return false; // or true it doesn't matter
        }

        ConnectedInventory inventory = connectedInventories[inventoryIndex];

        ItemSO itemSO = itemSoDictionary[inventoryHand];
        Vector2Int dimensions = itemSO.GetInventoryDimensions();

        if (inventory.IsAreaAvailable(slot, dimensions))
        {
            ConnectedInventory.ContainedItem containedItem = inventory.AddItem(inventoryHand, slot, dimensions);

            inventoryUI.AddItemDisplay(itemSO, containedItem, inventoryIndex);

            inventoryHand = InventoryItem.Empty;
            return true;
        }

        return false;
    }

    public bool RetrieveItem(int inventoryIndex, ConnectedInventory.ContainedItem containedItem)
    {
        if (inventoryHand != InventoryItem.Empty)
        {
            // Maybe swap items in the furture
            return false;
        }

        ConnectedInventory inventory = connectedInventories[inventoryIndex];

        InventoryItem retrievedItem = inventory.RemoveItem(containedItem);

        inventoryHand = retrievedItem;
        return true;
    }

    private int GetAvailableIndex()
    {
        for (int i = 0; i < 100; i++)
        {
            if (!connectedInventories.ContainsKey(i))
                return i;
        }

        Debug.LogError("Dude wtf there are more than 100 inventories stop it rn. Also you just broke my inventory system");
        return -1;
    }

    private void EnterVehicle(VehicleInteractionController vehicleInteractionController)
    {
        vehicle = vehicleInteractionController;
        vehicleInteractionController.EnterVehicle(GetNetworkObject(NetworkObjectId));

        AddConnectedInventory(vehicleInteractionController.GetInvetory());
    }
    #endregion

    #region Using Item Methods
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

        // TODO: Reset Hotbar I guess?
        //inventoryUI.ResetUsingSlot();

        Debug.LogWarning("RemoveUsing() is not fully implemented");

        HoldItemServerRpc(usingItem);
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
    #endregion

    #region Drop Item Methods
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

        foreach (var key in connectedInventories.Keys)
        {
            ConnectedInventory connectedInventory = connectedInventories[key];
            Vector2Int dimensions = connectedInventory.GetDimensions();
            int width = dimensions.x;
            int height = dimensions.y;

            // TODO: Drop all items in the inventory using the new system
            Debug.LogWarning("DropAllItems() is not fully implemented");

/*            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (connectedInventory.IsSlotFree(i, j))
                        DropItemServerRpc(connectedInventory.RemoveItem(i,j));

                    InventoryDisplay.Instance.UpdateItemSlot(0, i, j, InventoryItem.Empty);
                }
            }*/
        }
    }
    #endregion

    #region Hotbar Controlls

    public void OnHotbarKeyPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            string keyName = context.control.name;
            int hotbarSlotIndex = Int32.Parse(keyName) - 1;

            InventoryItem item = hotbar.GetItemAtSlot(hotbarSlotIndex);

            Debug.Log("Hotbar slot index: " + hotbarSlotIndex + " Item: " + item);

            HoldItemServerRpc(item);
        }
    }

    #endregion

    #region Debug Commands
    [Command]
    public void SpawnItemDebug(int x, int y, int z, InventoryItem itemEnum)
    {
        Vector3 position = new Vector3(x, y, z);

        if (!IsServer)
        {
            SpawnItemServerRpc(itemEnum, position);
            return;
        }

        SpawnedObject itemSpawnedObject = itemSoDictionary[itemEnum].GetItemPickupPrefab().GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }

    [Command]
    public void SpawnItemDebug(InventoryItem itemEnum)
    {
        Transform playerTransform = PlayerSpawner.localPlayerSpawner.transform;
        Vector3 position = playerTransform.position + playerTransform.forward * 2;

        if (!IsServer)
        {
            SpawnItemServerRpc(itemEnum, position);
            return;
        }

        SpawnedObject itemSpawnedObject = itemSoDictionary[itemEnum].GetItemPickupPrefab().GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemServerRpc(InventoryItem itemEnum, Vector3 position)
    {
        SpawnedObject itemSpawnedObject = itemSoDictionary[itemEnum].GetItemPickupPrefab().GetComponent<SpawnedObject>();
        ObjectSpawner.Instance.SpawnObject(itemSpawnedObject, position);
    }

    #endregion
}
