using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InventoryUI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Settings")]
    [SerializeField] private int inventorySlotWidth;
    [SerializeField] private Sprite emptySlotSprite;

    [Header("Referenced Display Components")]
    [SerializeField] private RectTransform inventoryHand;
    [SerializeField] private Button dropItemButton;
    [SerializeField] private RectTransform hotbarDisplay;
    [SerializeField] private RectTransform inventoriesContainer;
    [SerializeField] private ClothingSlotUI[] clothingSlotUIs;

    [Header("Generated Display Components")]
    [SerializeField] private SlotButton inventorySlotPrefab;
    [SerializeField] private ItemButton itemButtonPrefab;
    [SerializeField] private RectTransform hotbarSlotBackdrop;
    [SerializeField] private ConnectedInventoryUI connectedInventoryUIPrefab;
    [SerializeField] private HotbarUI hotbarUIPrefab;
    [SerializeField] private ConnectedInventoryHeader connectedInventoryHeaderPrefab;

    private Dictionary<int, ConnectedInventoryUI> conectedInventoryUIs;
    private Dictionary<int, ItemButton> _itemButtons = new Dictionary<int, ItemButton>();
    private Inventory _inventory;

    // TODO: Create an initialisation function that takes in the inventory and runs the basic setup for the inventory UI

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        inventoryHand.transform.position = Input.mousePosition + new Vector3(inventorySlotWidth / 2, inventorySlotWidth / 2, 0);
    }

    public void InitializeInventoryDisplay(Inventory inventory)
    {
        _inventory = inventory;

        dropItemButton.onClick.AddListener(() =>
        {
            inventoryHand.GetComponent<Image>().sprite = emptySlotSprite;
            _inventory.DropItem();
        });
    }

    public void CreateHotbarSlotUIs(int hotbarSlotCount, int hotbarSlotWidth, int hotbarSlotHeight)
    {
        if (conectedInventoryUIs == null)
            conectedInventoryUIs = new Dictionary<int, ConnectedInventoryUI>();

        ConnectedInventoryUI hotbarUI = Instantiate(hotbarUIPrefab, hotbarDisplay);

        Vector2Int dimensions = new Vector2Int(hotbarSlotCount * hotbarSlotWidth, hotbarSlotHeight);
        hotbarUI.Set(0, dimensions, inventorySlotPrefab, inventorySlotWidth, false, "Hotbar");

        conectedInventoryUIs.Add(0, hotbarUI);
    }

    public void SetInventoryHand(Inventory.InventoryItem inventoryItem)
    {
        ItemSO itemSO = Inventory.ItemSODictionary[inventoryItem];
        Vector2Int dimensions = itemSO.InInventoryDimensions;

        inventoryHand.sizeDelta = dimensions * inventorySlotWidth;
        inventoryHand.GetComponent<Image>().sprite = itemSO.UISprite;
    }

    public void CreateInventoryDisplay(int inventoryKey, Vector2Int dimensions)
    {
        int width = dimensions.x;
        int height = dimensions.y;

        ConnectedInventoryUI connectedInventoryUI = Instantiate(connectedInventoryUIPrefab, inventoriesContainer);

        connectedInventoryUI.Set(inventoryKey, dimensions, inventorySlotPrefab, inventorySlotWidth, true, "Put name here lmao");
        
        conectedInventoryUIs.Add(inventoryKey, connectedInventoryUI);

        StyleConnectedInventories();
    }

    private void StyleConnectedInventories()
    {
        int heightSum = 0;

        foreach (KeyValuePair<int, ConnectedInventoryUI> keyValuePair in conectedInventoryUIs)
        {
            // Skip the hotbar
            if(keyValuePair.Key == 0)
                continue;

            ConnectedInventoryUI connectedInventoryUI = keyValuePair.Value;

            connectedInventoryUI.SetYOffset(heightSum);

            heightSum += connectedInventoryUI.GetHeight();
        }

        inventoriesContainer.sizeDelta = new Vector2(inventoriesContainer.sizeDelta.x, heightSum);
    }

    public void RemoveIventoryDisplay(int inventoryID)
    {
        Destroy(conectedInventoryUIs[inventoryID].gameObject);
        conectedInventoryUIs.Remove(inventoryID);
    }

    public void AddItemDisplay(int inventoryKey, int containedItemKey, ItemSO itemSO, Vector2Int topLeft)
    {
        Vector2Int dimensions = itemSO.InInventoryDimensions;
        Vector2Int position = topLeft * inventorySlotWidth;

        ItemButton newItemButton = Instantiate(itemButtonPrefab, conectedInventoryUIs[inventoryKey].GetSlotsRect());
        newItemButton.Set(inventoryKey, containedItemKey, dimensions, position, inventorySlotWidth, itemSO.UISprite);

        int uniqueItemKey = CalculateUniqueItemKey(inventoryKey, containedItemKey);
        _itemButtons.Add(uniqueItemKey, newItemButton);        

        HideButtonArea(inventoryKey, topLeft, dimensions);
    }

    public void DestroyItemDisplay(int inventoryKey, int itemKey)
    {
        int uniqueItemKey = CalculateUniqueItemKey(inventoryKey, itemKey);

        ItemButton itemButton = _itemButtons[uniqueItemKey];
        _itemButtons.Remove(uniqueItemKey);
        Destroy(itemButton.gameObject);
    }

    // Calculates a unique key for each item display only used in the UI system
    private int CalculateUniqueItemKey(int inventoryKey, int containedItemKey)
    {
        return containedItemKey * 20 + inventoryKey;
    }

    public void SetClothingSlot(ClothingItemSO clothingItemSO)
    {
        ClothingSlotUI clothingSlotUI = clothingSlotUIs[(int)clothingItemSO.ClothingSlot];

        clothingSlotUI.Set(clothingItemSO);
    }

    public void RemoveClothingSlot(int slot)
    {
        ClothingSlotUI clothingSlotUI = clothingSlotUIs[slot];

        clothingSlotUI.Reset();
    }

    #region Button Area Methods

    private void HideButtonArea(int inventoryKey, Vector2Int topLeft, Vector2Int area)
    {
        ConnectedInventoryUI connectedInventoryUI = conectedInventoryUIs[inventoryKey];

        connectedInventoryUI.HideArea(topLeft, area);
    }

    public void ShowButtonArea(int inventoryID, Vector2Int topLeft, Vector2Int area)
    {
        ConnectedInventoryUI connectedInventoryUI = conectedInventoryUIs[inventoryID];

        connectedInventoryUI.ShowArea(topLeft, area);
    }

    #endregion

    #region Reset Methods

    public void ResetInventoryDisplay()
    {
        inventoryHand.GetComponent<Image>().sprite = emptySlotSprite;

        foreach (ConnectedInventoryUI connectedInventoryUI in conectedInventoryUIs.Values)
        {
            ResetInventoryDisplay(connectedInventoryUI);
        }
    }

    public void ResetInventoryDisplay(int key)
    {
        ConnectedInventoryUI connectedInventoryUI = conectedInventoryUIs[key];

        ResetInventoryDisplay(connectedInventoryUI);
    }

    private void ResetInventoryDisplay(ConnectedInventoryUI connectedInventoryUI)
    {
        connectedInventoryUI.ShowAll();

        foreach (ItemButton itemButton in _itemButtons.Values)
        {
            Destroy(itemButton.gameObject);
        }
        _itemButtons.Clear();
    }

    #endregion
}
