using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Inventory;

public class ItemSpawner : NetworkBehaviour
{
    public static ItemSpawner Instance;

    [Header("Item Refs")]
    [SerializeField] private ItemSO[] itemSos;

    private static Dictionary<Inventory.InventoryItem, ItemSO> itemDictionary;
    public static Dictionary<Inventory.InventoryItem, ItemSO> ItemDictionary
    {
        get { return itemDictionary; }
        private set { }
    }

    private void Start()
    {
        if (Instance == null) 
            Instance = this;

        if (itemDictionary == null)
        {
            itemDictionary = new Dictionary<InventoryItem, ItemSO>();
            for (int i = 0; i < itemSos.Length; i++)
                itemDictionary.Add(itemSos[i].GetInventoryItem(), itemSos[i]);
        }
    }

    public void SpawnItem(Vector3 position, Inventory.InventoryItem itemEnum)
    {
        if (!IsServer) 
            return;

        SpawnItemServerRpc(position, itemEnum);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemServerRpc(Vector3 position, Inventory.InventoryItem itemEnum)
    {
        GameObject itemGameObject = Instantiate(itemDictionary[itemEnum].GetItemPickupPrefab(), position + Vector3.up * 2.5f, new Quaternion(0, 0, 0, 0));

        NetworkObject itemNetworkObject = itemGameObject.GetComponent<NetworkObject>();
        itemNetworkObject.Spawn(true);
    }
}
