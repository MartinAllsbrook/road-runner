using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public class ConnectedInventoryUI
    {
        public RectTransform inventoryDisplay;
        public Button[,] buttons;
    }

    [Header("Settings")]
    [SerializeField] private int inventorySlotWidth;
    [SerializeField] private Sprite emptySlotSprite;

    [Header("Referenced Display Components")]
    [SerializeField] private RectTransform inventoryHand;
    [SerializeField] private Button dropItemButton;
    [SerializeField] private RectTransform hotbarDisplay;

    [Header("Generated Display Components")]
    [SerializeField] private Button inventorySlotPrefab;
    [SerializeField] private ItemButton itemButtonPrefab;
    [SerializeField] private RectTransform hotbarSlotBackdrop;
    [SerializeField] private RectTransform connectedInventoryBackdrop;

    private Dictionary<int, ConnectedInventoryUI> conectedInventoryUIs;
    private Dictionary<int, ItemButton> _itemButtons = new Dictionary<int, ItemButton>();
    private Inventory _inventory;

    // TODO: Create an initialisation function that takes in the inventory and runs the basic setup for the inventory UI

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
        hotbarUI.buttons = new Button[hotbarSlotCount * hotbarSlotWidth, hotbarSlotHeight];

        for (int slotIndex = 0; slotIndex < hotbarSlotCount; slotIndex++)
        {
            for (int y = 0; y < hotbarSlotHeight; y++)
            {
                for (int slotX = 0; slotX < hotbarSlotWidth; slotX++)
                {

                    Button newSlot = Instantiate(inventorySlotPrefab, hotbarUI.inventoryDisplay);

                    int x = slotIndex * hotbarSlotWidth + slotX;
                    int yCopy = y;
                    newSlot.onClick.AddListener(() =>
                    {
                        Inventory.Instance.TryPlaceInSlot(0, new Vector2Int(x, yCopy));
                    });

                    StyleSlot(newSlot.GetComponent<RectTransform>(), new Vector2Int(x, y));

                    hotbarUI.buttons[x, y] = newSlot;
                }
            }
        }

        conectedInventoryUIs.Add(0, hotbarUI);
    }

    private void Update()
    {
        inventoryHand.transform.position = Input.mousePosition + new Vector3(inventorySlotWidth / 2, inventorySlotWidth / 2, 0);
    }

    public void SetInventoryHand(Inventory.InventoryItem inventoryItem)
    {
        ItemSO itemSO = Inventory.ItemSODictionary[inventoryItem];
        Vector2Int dimensions = itemSO.InInventoryDimensions;

        inventoryHand.sizeDelta = dimensions * inventorySlotWidth;
        inventoryHand.GetComponent<Image>().sprite = itemSO.UISprite;
    }

    private void StyleSlot(RectTransform slot, Vector2Int intPosition)
    {
        slot.GetComponent<RectTransform>().sizeDelta = new Vector2(inventorySlotWidth, inventorySlotWidth);

        intPosition.y = -intPosition.y;

        Vector2 positon = intPosition * inventorySlotWidth;
        slot.GetComponent<RectTransform>().anchoredPosition = positon;
    }

    private void StyleConnectedInventory(int inventoryIndex, RectTransform connectedInventory, Vector2Int dimensions)
    {
        float yOffest = 0;
        for (int x = 0; x < inventoryIndex; x++)
        {
            if (conectedInventoryUIs.ContainsKey(x))
            {
                yOffest += conectedInventoryUIs[x].inventoryDisplay.sizeDelta.y;
            }
        }
        connectedInventory.sizeDelta = dimensions * inventorySlotWidth;
        connectedInventory.anchoredPosition = new Vector2(0, -yOffest);
    }

    public void CreateInventoryDisplay(int inventoryID, Vector2Int dimensions)
    {
        int width = dimensions.x;
        int height = dimensions.y;

        ConnectedInventoryUI connectedInventoryUI = new ConnectedInventoryUI();
        connectedInventoryUI.inventoryDisplay = Instantiate(connectedInventoryBackdrop, transform);
        // Deal with height of inventories        
        connectedInventoryUI.buttons = new Button[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // TODO: Make the next line not use GetComponent
                // TODO: Make all instantiations of inventorySlotPrefab set the size of the button
                Button newInventoryButton = Instantiate(inventorySlotPrefab, connectedInventoryUI.inventoryDisplay).GetComponent<Button>();

                newInventoryButton.onClick.AddListener(() => 
                { 
                    Inventory.Instance.TryPlaceInSlot(inventoryID, new Vector2Int(x, y)); 
                });

                StyleSlot(newInventoryButton.GetComponent<RectTransform>(), new Vector2Int(x, y));

                connectedInventoryUI.buttons[x, y] = newInventoryButton;
            }
        }

        conectedInventoryUIs.Add(inventoryID, connectedInventoryUI);
    }

    public void RemoveIventoryDisplay(int inventoryID)
    {
        Destroy(conectedInventoryUIs[inventoryID].inventoryDisplay.gameObject);
        conectedInventoryUIs.Remove(inventoryID);
    }

    public void AddItemDisplay(int inventoryKey, int containedItemKey, ItemSO itemSO, Vector2Int topLeft)
    {
        Vector2Int dimensions = itemSO.InInventoryDimensions;

        Debug.Log("Adding item to inventory: " + inventoryKey);

        ItemButton newItemButton = Instantiate(itemButtonPrefab, conectedInventoryUIs[inventoryKey].inventoryDisplay);

        Vector2 position = topLeft * inventorySlotWidth;

        var cInventory = Inventory.Instance;
        var cInvetotyUI = this;
        newItemButton.Set(dimensions, position, inventorySlotWidth, itemSO.UISprite);

        newItemButton.GetButton().onClick.AddListener(() =>
        {
            // TODO: Maybe use the dictionary here
            cInventory.RetrieveItem(inventoryKey, containedItemKey);
        });

        int uniqueItemKey = CalculateUniqueItemKey(inventoryKey, containedItemKey);
        _itemButtons.Add(uniqueItemKey, newItemButton);        

        HideButtonArea(inventoryKey, topLeft, dimensions);
    }

    private int CalculateUniqueItemKey(int inventoryKey, int containedItemKey)
    {
        return containedItemKey * 20 + inventoryKey;
    }

    public void DestroyItemDisplay(int inventoryKey, int itemKey)
    {
        int uniqueItemKey = CalculateUniqueItemKey(inventoryKey, itemKey);
        ItemButton itemButton = _itemButtons[uniqueItemKey];
        _itemButtons.Remove(uniqueItemKey);
        Destroy(itemButton.gameObject);
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

                connectedInventoryUI.buttons[x, y].gameObject.SetActive(false);
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

                if (connectedInventoryUI.buttons[x, y].gameObject.active)
                {
                    Debug.LogError("Tried to activate an active button in inventory " + inventoryID + " at slot " + x + ", " + y + ". This suggests there are issues in the inventory code...");
                }

                connectedInventoryUI.buttons[x, y].gameObject.SetActive(true);
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
            foreach (Button button in connectedInventoryUI.buttons)
            {
                button.gameObject.SetActive(true);
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

        foreach (Button button in connectedInventoryUI.buttons)
        {
            button.gameObject.SetActive(true);
        }
        foreach (ItemButton itemButton in _itemButtons.Values)
        {
            Destroy(itemButton.gameObject);
        }
        _itemButtons.Clear();
    }

    #endregion
}
