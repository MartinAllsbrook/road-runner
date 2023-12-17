using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using QFSW.QC;
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

    private void Awake()
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

    // if we are the server, we can skip the server rpc and just spawn the thing
    public void SpawnItem(Vector3 position, Inventory.InventoryItem itemEnum)
    {
        GameObject itemGameObject = Instantiate(itemDictionary[itemEnum].GetItemPickupPrefab(), position + Vector3.up * 2.5f, new Quaternion(0, 0, 0, 0));

        NetworkObject itemNetworkObject = itemGameObject.GetComponent<NetworkObject>();
        itemNetworkObject.Spawn(true);

        //SpawnItemServerRpc(position, itemEnum);
    }

    #region Stuff for Clients, needed for QC & debugging

    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemServerRpc(Vector3 position, Inventory.InventoryItem itemEnum)
    {
        SpawnItem(position, itemEnum);
    }

    [Command]
    public void SpawnItemDebug(int x, int y, int z, InventoryItem itemEnum)
    {
        Vector3 position = new Vector3(x, y, z);

        if (!IsServer)
        {
            SpawnItemServerRpc(position, itemEnum);
            return;
        }

        SpawnItem(position, itemEnum);
    }

    [Command]
    public void SpawnItemDebug(InventoryItem itemEnum)
    {
        Transform playerTransform = PlayerSpawner.localPlayerSpawner.transform;
        Vector3 position = playerTransform.position + playerTransform.forward * 2;
        if (!IsServer)
        {
            SpawnItemServerRpc(position, itemEnum);
            return;
        }
        SpawnItem(position, itemEnum);
    }

    #endregion
}
