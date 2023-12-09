using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]

public class InventoryDisplay : MonoBehaviour
{
    public static InventoryDisplay Instance;

    private BaseInventory baseInventory;
    
    [SerializeField] private Button button;
    [SerializeField] private int slotSpacing = 45;
    
    [SerializeField] private Button usingSlotButton;
    [SerializeField] private Image usingSlotImage;
    
    private Dictionary<Inventory.InventoryItem, ItemSO> itemSoDictionary;

    [SerializeField] private Button dropButton;
    
    [SerializeField] private Image handDisplay;

    [SerializeField] private RectTransform inventoryBackdrop;
    // private RectTransform handDisplayTransform;
    private Dictionary<int, RectTransform> invetoryDisplays;
    private Dictionary<int, Image[,]> inventoryImages;
    //private Button[,] inventoryButtons;

    private float nextInventoryYPosition = -10;
    
    private Vector2 dimensions;

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
        itemSoDictionary = ItemSpawner.ItemDictionary;
        
        handDisplay.sprite = itemSoDictionary[Inventory.InventoryItem.Empty].GetSprite();
        
        usingSlotButton.onClick.AddListener(() => HandleUsingSlotClick());
        dropButton.onClick.AddListener(() => DropItem());
        CreateDisplayArrays(0, width, height);
    }

    public void CreateInventoryDisplay(int inventoryKey, int width, int height)
    {
        if (inventoryKey == 0)
        {
            Debug.LogError("inventoryIndex can only be 0 for base invetory");
            return;
        }

        CreateDisplayArrays(inventoryKey, width, height);
    }

    private void CreateDisplayArrays(int inventoryKey, int width, int height)
    {
        Button[,] inventoryButtons = new Button[width, height];
        inventoryImages.Add(inventoryKey, new Image[width, height]);

        invetoryDisplays.Add(inventoryKey, Instantiate(inventoryBackdrop, transform.position, transform.rotation, transform));
        invetoryDisplays[inventoryKey].anchoredPosition = new Vector2(10, nextInventoryYPosition);

        int inventoryHeight = height * slotSpacing + 5;
        invetoryDisplays[inventoryKey].sizeDelta = new Vector2(width * slotSpacing + 5, inventoryHeight);

        nextInventoryYPosition -= inventoryHeight + 10; // Adust position of next iventory!

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // LAGGGG ZONNEEEE... maybe....
                inventoryButtons[x,y] = Instantiate(button, transform.position, transform.rotation, invetoryDisplays[inventoryKey]);
                inventoryButtons[x,y].GetComponent<RectTransform>().anchoredPosition = new Vector2(x * slotSpacing + 5, y * -slotSpacing - 5);
                
                inventoryImages[inventoryKey][x,y] = inventoryButtons[x, y].transform.Find("Image").GetComponent<Image>();
                inventoryImages[inventoryKey][x,y].sprite = itemSoDictionary[Inventory.InventoryItem.Empty].GetSprite();
                
                int xValue = x;
                int yValue = y;
                int inventoryIndexValue = inventoryKey;
                inventoryButtons[x,y].onClick.AddListener(() => {HandleInventoryClick(inventoryIndexValue, xValue, yValue);});
            }
        }
    }

    public void RemoveInvetoryDisplay(int inventoryKey)
    {
        float removedInventoryHeight = invetoryDisplays[inventoryKey].sizeDelta.y + 10;

        nextInventoryYPosition += removedInventoryHeight;

        Destroy(invetoryDisplays[inventoryKey].gameObject);
        inventoryImages.Remove(inventoryKey);
        invetoryDisplays.Remove(inventoryKey);
    }

    private void HandleInventoryClick(int inventoryIndex, int x, int y)
    {
        Debug.Log("Clicked inventory square X: " + x + " Y: " + y);
        
        Inventory.InventoryItem itemForSlot = baseInventory.ClickOnSlot(inventoryIndex, x, y, out Inventory.InventoryItem handItem);

        inventoryImages[inventoryIndex][x,y].sprite = itemSoDictionary[itemForSlot].GetSprite();
        handDisplay.sprite = itemSoDictionary[handItem].GetSprite();
    }

    public void UpdateItemSlot(int inventoryKey, int x, int y, Inventory.InventoryItem inventoryItem)
    {
        inventoryImages[inventoryKey][x,y].sprite = itemSoDictionary[inventoryItem].GetSprite();
    }

    private void HandleUsingSlotClick()
    {
        Inventory.InventoryItem usingItem = baseInventory.SetUsing(out Inventory.InventoryItem handItem);
        usingSlotImage.sprite = itemSoDictionary[usingItem].GetSprite();
        handDisplay.sprite = itemSoDictionary[handItem].GetSprite();
    }

    public void ResetUsingSlot()
    {
        usingSlotImage.sprite = itemSoDictionary[Inventory.InventoryItem.Empty].GetSprite();
    }

    private void DropItem()
    {
        handDisplay.sprite = itemSoDictionary[Inventory.InventoryItem.Empty].GetSprite();
        baseInventory.DropItem();
    }
}
