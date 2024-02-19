using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class GlobalItemDictionary : MonoBehaviour
{
    public static GlobalItemDictionary Instance { get; private set; }

    private static bool IsInitialized = false;

    protected static Dictionary<ItemID, ItemSO> itemSoDictionary;
    public static Dictionary<ItemID, ItemSO> ItemSODictionary 
    {
        get 
        { 
            if (!IsInitialized)
            {
                Instance.CreateItemDictionary();
                IsInitialized = true;
            }

            return itemSoDictionary; 
        } 
    }

    [Header("Item Refs")]
    [SerializeField] private AllItemsSO allItemSOsSO;

    public enum ItemID // Item IDs used with the itemSODictionary. I don't think these could represent a modified item.
    {
        Empty = 0,

        // Guns 1 - 100
        Gun_M48 = 1,
        Gun_Ak74 = 2,
        Gun_BenneliM4 = 3,
        Gun_M107 = 4,
        Gun_M1911 = 5,
        Gun_Rpg7 = 6,
        Gun_Uzi = 7,
        Gun_M249 = 8,

        // Consumables 101 - 200
        Consumable_Apple = 101,
        Consumable_WaterBottle = 102,
        Consumable_Beans = 103,
        Consumable_Medkit = 104,
        Consumable_Pills = 105,
        Consumable_Egg = 106,

        // Clothing 201 - 300
        Clothing_Backpack = 201,
        Clothing_Hat = 202,
        Clothing_Vest = 203,
        Clothing_Shirt = 204,
        Clothing_Pants = 205,
        Clothing_Shoes = 206,
        Clothing_Mask = 207,
        Clothing_Goggles = 208,

        // Ammo & Attachments 301 - 400
        Attachment_M48Magazine = 301, // TODO: Rename to Attachment_M48Magaizne
        Attachment_M48IronSight = 302,
        Attachment_LargeScope = 303,
        Attachment_AK74Magazine = 304,
        Attachment_M107Magazine = 305,
        Attachment_M249Magazine = 306,
        Attachment_UZIMagazine = 307,

        // Ammo 401 - 500, Kinda just for testing rn
        Bullet_556 = 401,

        // Tools 501 - 600
        Tool_FryingPan = 501,
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateItemDictionary()
    {
        Debug.Log(" -===============-=-=-=-=================== Creating item dictionary");
        itemSoDictionary = new Dictionary<ItemID, ItemSO>();

        ItemSO[] allItemSOs = allItemSOsSO.GetAllItemSOs();

        foreach (ItemSO itemSO in allItemSOs)
        {
            itemSoDictionary.Add(itemSO.ItemID, itemSO);
        }
    }
}
