using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalItemDictionary;

public class ClothingSlotUI : InventoryUIButton
{
    [SerializeField] private Image clothingImage;
    [SerializeField] private ClothingSlot clothingSlotType;

    [SerializeField] private Sprite emptySlotSprite;

    private ClothingData clothingData;

    public enum ClothingSlot
    {
        Helmet,
        FaceCover,
        Glasses,
        Shirt,
        Vest,
        Backpack,
        Pants,
        Shoes
    }

    public void Set(ClothingData clothingData)
    {
        this.clothingData = clothingData;

        ItemID itemID = clothingData.BaseItemID;
        ItemSO clothingItemSO = ItemSODictionary[itemID];

        clothingSlotType = clothingData.ClothingSlot;

        clothingImage.sprite = clothingItemSO.UISprite;

        AddListener();
    }

    public void Reset()
    {
        clothingImage.sprite = emptySlotSprite;

        RemoveListener();
    }

    protected override void OnClick()
    {
        Inventory.Instance.RemoveClothingInventory(clothingData);
    }

}
