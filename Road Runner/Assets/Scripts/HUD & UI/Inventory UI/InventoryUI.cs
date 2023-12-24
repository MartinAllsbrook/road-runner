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

    [Header("Generated Display Components")]
    [SerializeField] private Button inventorySlotPrefab;
    [SerializeField] private ItemButton itemButtonPrefab;

    [Header("Hotbar")]
    [SerializeField] private RectTransform hotbarDisplay;
    [SerializeField] private RectTransform hotbarSlotBackdrop;


    private Dictionary<int, ConnectedInventoryUI> conectedInventoryUIs;
    private List<Button> _itemButtons = new List<Button>();
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
        Vector2Int dimensions = itemSO.GetInventoryDimensions();

        inventoryHand.sizeDelta = dimensions * inventorySlotWidth;
        inventoryHand.GetComponent<Image>().sprite = itemSO.GetSprite();
    }

    private void StyleSlot(RectTransform slot, Vector2Int intPosition)
    {
        slot.GetComponent<RectTransform>().sizeDelta = new Vector2(inventorySlotWidth, inventorySlotWidth);

        intPosition.y = -intPosition.y;

        Vector2 positon = intPosition * inventorySlotWidth;
        slot.GetComponent<RectTransform>().anchoredPosition = positon;
    }

    public void CreateInventoryDisplay(int inventoryID, Vector2Int dimensions, int slotHeight)
    {
        int width = dimensions.x;
        int height = dimensions.y;

        ConnectedInventoryUI connectedInventoryUI = new ConnectedInventoryUI();
        connectedInventoryUI.inventoryDisplay = Instantiate(hotbarSlotBackdrop, transform);
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

                connectedInventoryUI.buttons[x, y] = newInventoryButton;
            }
        }

        conectedInventoryUIs.Add(inventoryID, connectedInventoryUI);
    }

    public void AddItemDisplay(ItemSO itemSO, ConnectedInventory.ContainedItem containedItem, int inventoryID)
    {
        Vector2Int dimensions = itemSO.GetInventoryDimensions();
        Vector2Int topLeft = containedItem.topLeft;

        Debug.Log("Adding item to inventory ID: " + inventoryID);

        ItemButton newItemButton = Instantiate(itemButtonPrefab, conectedInventoryUIs[inventoryID].inventoryDisplay);

        Vector2 position = topLeft * inventorySlotWidth;

        Inventory inventory = Inventory.Instance;
        Debug.Log(inventory);
        newItemButton.Set(dimensions, position, inventorySlotWidth, itemSO.GetSprite());

        newItemButton.GetButton().onClick.AddListener(() =>
        {
            Debug.Log(containedItem);
            Debug.Log(inventoryID);
            Debug.Log(inventory);
            if (inventory.RetrieveItem(inventoryID, containedItem))
            {
                Destroy(newItemButton.gameObject);
            }
        });

        _itemButtons.Add(newItemButton.GetButton());
        
        // TODO: Position the item display correctly

        HideButtonArea(inventoryID, topLeft, dimensions);
    }
    
    private void HideButtonArea(int inventoryID, Vector2Int topLeft, Vector2Int area)
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

    public void ResetInventoryDisplay()
    {
        inventoryHand.GetComponent<Image>().sprite = emptySlotSprite;


        foreach (ConnectedInventoryUI connectedInventoryUI in conectedInventoryUIs.Values)
        {
            foreach (Button button in connectedInventoryUI.buttons)
            {
                button.gameObject.SetActive(true);
            }
            foreach (Button itemButton in _itemButtons)
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
        foreach (Button itemButton in _itemButtons)
        {
            Destroy(itemButton.gameObject);
        }
        _itemButtons.Clear();
    }
}
