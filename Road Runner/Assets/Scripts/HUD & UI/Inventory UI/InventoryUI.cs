using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InventoryUI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public class ConnectedInventoryUI
    {
        public RectTransform inventoryDisplay;
        public ConnectedInventoryHeader connectedInventoryHeader;
        public SlotButton[,] slotButtons; // 
    }

    [Header("Settings")]
    [SerializeField] private int inventorySlotWidth;
    [SerializeField] private Sprite emptySlotSprite;

    [Header("Referenced Display Components")]
    [SerializeField] private RectTransform inventoryHand;
    [SerializeField] private Button dropItemButton;
    [SerializeField] private RectTransform hotbarDisplay;
    [SerializeField] private RectTransform inventoriesContainer;

    [Header("Generated Display Components")]
    [SerializeField] private SlotButton inventorySlotPrefab;
    [SerializeField] private ItemButton itemButtonPrefab;
    [SerializeField] private RectTransform hotbarSlotBackdrop;
    [SerializeField] private RectTransform connectedInventoryBackdrop;
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

        ConnectedInventoryUI hotbarUI = new ConnectedInventoryUI();

        hotbarUI.inventoryDisplay = hotbarDisplay;
        hotbarUI.slotButtons = new SlotButton[hotbarSlotCount * hotbarSlotWidth, hotbarSlotHeight];

        for (int slotIndex = 0; slotIndex < hotbarSlotCount; slotIndex++)
        {
            for (int y = 0; y < hotbarSlotHeight; y++)
            {
                for (int slotX = 0; slotX < hotbarSlotWidth; slotX++)
                {
                    int x = slotIndex * hotbarSlotWidth + slotX;

                    SlotButton newSlot = Instantiate(inventorySlotPrefab, hotbarUI.inventoryDisplay);
                    newSlot.Set(0, new Vector2Int(x, y), inventorySlotWidth);

                    hotbarUI.slotButtons[x, y] = newSlot;
                }
            }
        }

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

        ConnectedInventoryUI connectedInventoryUI = new ConnectedInventoryUI();
        connectedInventoryUI.inventoryDisplay = Instantiate(connectedInventoryBackdrop, inventoriesContainer);
        // Deal with height of inventories        
        connectedInventoryUI.slotButtons = new SlotButton[width, height];

/*        ConnectedInventoryHeader newConnectedInventoryHeader = Instantiate(connectedInventoryHeaderPrefab, inventoriesContainer);
        newConnectedInventoryHeader.Set(inventoryID, "Put name here lmao", new Vector2Int(width * inventorySlotWidth, inventorySlotWidth));
        Button headerButton = newConnectedInventoryHeader.GetButton();
        headerButton.onClick.AddListener(() =>
        {
            // TODO: All this \/
            // Remove the inventory
            // Drop the items
            // Drop the inventory
            // Remove the inventory display
            // Move all this to a better place
        });*/

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SlotButton newSlotButton = Instantiate(inventorySlotPrefab, connectedInventoryUI.inventoryDisplay);

                var slot = new Vector2Int(x, y);

                newSlotButton.Set(inventoryKey, slot, inventorySlotWidth);

                connectedInventoryUI.slotButtons[x, y] = newSlotButton;
            }
        }
        conectedInventoryUIs.Add(inventoryKey, connectedInventoryUI);

        StyleConnectedInventories();
    }

/*    private void StyleSlot(RectTransform slot, Vector2Int intPosition, bool addHeaderSpace)
    {
        slot.GetComponent<RectTransform>().sizeDelta = new Vector2(inventorySlotWidth, inventorySlotWidth);

        if (addHeaderSpace)
            intPosition.y = -intPosition.y - 1;
        else
            intPosition.y = -intPosition.y;

        Vector2 positon = intPosition * inventorySlotWidth;
        slot.GetComponent<RectTransform>().anchoredPosition = positon;
    }*/

    private void StyleConnectedInventories()
    {
        float heightSum = 0;
        float widestWidth = 0;

        foreach (ConnectedInventoryUI connectedInventoryUI in conectedInventoryUIs.Values)
        {
            if(connectedInventoryUI.inventoryDisplay == hotbarDisplay)
                continue;

            int width = connectedInventoryUI.slotButtons.GetLength(0);
            int height = connectedInventoryUI.slotButtons.GetLength(1);

            connectedInventoryUI.inventoryDisplay.anchoredPosition = new Vector2(0, -heightSum);

            int totalDisplayHeight = (height) * inventorySlotWidth; // +1 for the header (before)
            connectedInventoryUI.inventoryDisplay.sizeDelta = new Vector2(width * inventorySlotWidth, height * inventorySlotWidth);

            heightSum += connectedInventoryUI.inventoryDisplay.sizeDelta.y;
            if (connectedInventoryUI.inventoryDisplay.sizeDelta.x > widestWidth)
                widestWidth = connectedInventoryUI.inventoryDisplay.sizeDelta.x;
        }

        inventoriesContainer.sizeDelta = new Vector2(widestWidth, heightSum);
    }

    public void RemoveIventoryDisplay(int inventoryID)
    {
        Destroy(conectedInventoryUIs[inventoryID].inventoryDisplay.gameObject);
        conectedInventoryUIs.Remove(inventoryID);
    }

    public void AddItemDisplay(int inventoryKey, int containedItemKey, ItemSO itemSO, Vector2Int topLeft)
    {
        Vector2Int dimensions = itemSO.InInventoryDimensions;
        Vector2Int position = topLeft * inventorySlotWidth;

        ItemButton newItemButton = Instantiate(itemButtonPrefab, conectedInventoryUIs[inventoryKey].inventoryDisplay);
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

    #region Button Area Methods

    private void HideButtonArea(int inventoryKey, Vector2Int topLeft, Vector2Int area)
    {
        ConnectedInventoryUI connectedInventoryUI = conectedInventoryUIs[inventoryKey];

        int width = area.x;
        int height = area.y;

        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                int x = topLeft.x + xi;
                int y = topLeft.y + yi;

                connectedInventoryUI.slotButtons[x, y].gameObject.SetActive(false);
            }
        }
    }

    public void ShowButtonArea(int inventoryID, Vector2Int topLeft, Vector2Int area)
    {
        ConnectedInventoryUI connectedInventoryUI = conectedInventoryUIs[inventoryID];

        int width = area.x;
        int height = area.y;

        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                int x = topLeft.x + xi;
                int y = topLeft.y + yi;

                if (connectedInventoryUI.slotButtons[x, y].gameObject.activeSelf)
                {
                    Debug.LogError("Tried to activate an active button in inventory " + inventoryID + " at slot " + x + ", " + y + ". This suggests there are issues in the inventory code...");
                }

                connectedInventoryUI.slotButtons[x, y].gameObject.SetActive(true);
            }
        }
    }

    #endregion

    #region Reset Methods

    public void ResetInventoryDisplay()
    {
        inventoryHand.GetComponent<Image>().sprite = emptySlotSprite;


        foreach (ConnectedInventoryUI connectedInventoryUI in conectedInventoryUIs.Values)
        {
            foreach (SlotButton slotButton in connectedInventoryUI.slotButtons)
            {
                slotButton.gameObject.SetActive(true);
            }

            foreach (ItemButton itemButton in _itemButtons.Values)
            {
                Destroy(itemButton.gameObject);
            }



            _itemButtons.Clear();
        }
    }

    public void ResetInventoryDisplay(int key)
    {
        ConnectedInventoryUI connectedInventoryUI = conectedInventoryUIs[key];

        foreach (SlotButton slotButton in connectedInventoryUI.slotButtons)
        {
            slotButton.gameObject.SetActive(true);
        }
        foreach (ItemButton itemButton in _itemButtons.Values)
        {
            Destroy(itemButton.gameObject);
        }
        _itemButtons.Clear();
    }

    #endregion
}
