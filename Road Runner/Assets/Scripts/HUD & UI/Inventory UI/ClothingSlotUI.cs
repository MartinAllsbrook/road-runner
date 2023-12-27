using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothingSlotUI : InventoryUIButton
{
    [SerializeField] private Image clothingImage;
    [SerializeField] private ClothingSlot clothingSlotType;

    [SerializeField] private Sprite emptySlotSprite;

    private ClothingItemSO _clothingItemSO;

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

    public void Set(ClothingItemSO clothingItemSO)
    {
        _clothingItemSO = clothingItemSO;

        clothingImage.sprite = clothingItemSO.UISprite;
        clothingSlotType = clothingItemSO.ClothingSlot;

        AddListener();
    }

    public void Reset()
    {
        clothingImage.sprite = emptySlotSprite;

        RemoveListener();
    }

    protected override void OnClick()
    {
        Inventory.Instance.RemoveClothingInventory(_clothingItemSO);
    }

}
