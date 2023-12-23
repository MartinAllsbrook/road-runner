/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]

public class InventoryDisplay : MonoBehaviour
{
    public static InventoryDisplay Instance;

    private BaseInventory baseInventory;

    [SerializeField] private Button button; // Button for inventory slot
    [SerializeField] private int slotSpacing = 45; // Spacing between inventory slots

    [SerializeField] private Button usingSlotButton; // Button for the slot currently in use
    [SerializeField] private Image usingSlotImage; // Image for the slot currently in use

    private Dictionary<Inventory.InventoryItem, ItemSO> itemSoDictionary;

    [SerializeField] private Button dropButton; // Button to drop an item

    [SerializeField] private Image handDisplay; // Display for the item in hand

    [SerializeField] private RectTransform inventoryBackdrop; // Backdrop for the inventory display
    private Dictionary<int, RectTransform> invetoryDisplays; // Dictionary to hold all inventory displays
    private Dictionary<int, Image[,]> inventoryImages; // Dictionary to hold all inventory images

    private float nextInventoryYPosition = -10; // Position for the next inventory display

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        itemSoDictionary = new Dictionary<Inventory.InventoryItem, ItemSO>();
        invetoryDisplays = new Dictionary<int, RectTransform>();
        inventoryImages = new Dictionary<int, Image[,]>();

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        handDisplay.transform.position = Input.mousePosition;
    }

    // Base inventory can drop and pickup items, it is also always there
    public void CreateBaseInventoryDisplay(int width, int height, BaseInventory baseInventoryRef)
    {
        baseInventory = baseInventoryRef;
        itemSoDictionary = Inventory.ItemSODictionary;

        handDisplay.sprite = itemSoDictionary[Inventory.InventoryItem.Empty].GetSprite();

        usingSlotButton.onClick.AddListener(() => HandleUsingSlotClick());
        dropButton.onClick.AddListener(() => DropItem());
        CreateDisplayArrays(0, width, height); // Create the display for the base inventory
    }

    public void CreateInventoryDisplay(int inventoryKey, int width, int height)
    {
        if (inventoryKey == 0)
        {
            Debug.LogError("inventoryIndex can only be 0 for base invetory");
            return;
        }

        CreateDisplayArrays(inventoryKey, width, height); // Create the display for the additional inventory
    }

    private void CreateDisplayArrays(int inventoryKey, int width, int height)
    {
        Button[,] inventoryButtons = new Button[width, height]; // Array to hold all inventory slots
        inventoryImages.Add(inventoryKey, new Image[width, height]); // Add this inventory to the dictionary of inventory display's images

        invetoryDisplays.Add(inventoryKey, Instantiate(inventoryBackdrop, transform.position, transform.rotation, transform));
        invetoryDisplays[inventoryKey].anchoredPosition = new Vector2(10, nextInventoryYPosition); // Positioning the inventory display

        int inventoryHeight = height * slotSpacing + 5;
        invetoryDisplays[inventoryKey].sizeDelta = new Vector2(width * slotSpacing + 5, inventoryHeight); // Setting the size of the inventory display

        nextInventoryYPosition -= inventoryHeight + 10; // Adjusting the position for the next inventory display

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // LAGGGG ZONNEEEE... maybe....
                inventoryButtons[x, y] = Instantiate(button, transform.position, transform.rotation, invetoryDisplays[inventoryKey]);
                // inventoryButtons[x, y].GetComponent<RectTransform>().anchoredPosition = new Vector2(x * slotSpacing + 5, y * -slotSpacing - 5); // Positioning the inventory slot

                inventoryImages[inventoryKey][x, y] = inventoryButtons[x, y].transform.Find("Image").GetComponent<Image>();
                inventoryImages[inventoryKey][x, y].sprite = itemSoDictionary[Inventory.InventoryItem.Empty].GetSprite(); // Setting the default image for the inventory slot

                int xValue = x;
                int yValue = y;
                int inventoryIndexValue = inventoryKey;
                inventoryButtons[x, y].onClick.AddListener(() => { HandleInventoryClick(inventoryIndexValue, xValue, yValue); });
            }
        }
    }

    public void RemoveInvetoryDisplay(int inventoryKey)
    {
        float removedInventoryHeight = invetoryDisplays[inventoryKey].sizeDelta.y + 10;

        nextInventoryYPosition += removedInventoryHeight; // Adjusting the position for the next inventory display after removing an inventory display

        Destroy(invetoryDisplays[inventoryKey].gameObject);
        inventoryImages.Remove(inventoryKey);
        invetoryDisplays.Remove(inventoryKey);
    }

    private void HandleInventoryClick(int inventoryIndex, int x, int y)
    {
        Debug.Log("Clicked inventory square X: " + x + " Y: " + y);

        Inventory.InventoryItem itemForSlot = baseInventory.ClickOnSlot(inventoryIndex, x, y, out Inventory.InventoryItem handItem);

        inventoryImages[inventoryIndex][x, y].sprite = itemSoDictionary[itemForSlot].GetSprite(); // Updating the image for the clicked inventory slot
        handDisplay.sprite = itemSoDictionary[handItem].GetSprite(); // Updating the image for the item in hand
    }

    public void UpdateItemSlot(int inventoryKey, int x, int y, Inventory.InventoryItem inventoryItem)
    {
        inventoryImages[inventoryKey][x, y].sprite = itemSoDictionary[inventoryItem].GetSprite(); // Updating the image for the inventory slot
    }

    private void HandleUsingSlotClick()
    {
        Inventory.InventoryItem usingItem = baseInventory.SetUsing(out Inventory.InventoryItem handItem);
        usingSlotImage.sprite = itemSoDictionary[usingItem].GetSprite(); // Updating the image for the slot currently in use
        handDisplay.sprite = itemSoDictionary[handItem].GetSprite(); // Updating the image for the item in hand
    }

    public void ResetUsingSlot()
    {
        usingSlotImage.sprite = itemSoDictionary[Inventory.InventoryItem.Empty].GetSprite(); // Resetting the image for the slot currently in use
    }

    private void DropItem()
    {
        handDisplay.sprite = itemSoDictionary[Inventory.InventoryItem.Empty].GetSprite(); // Resetting the image for the item in hand after dropping an item
        baseInventory.DropItem();
    }
}
*/