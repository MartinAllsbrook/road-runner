using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothingSlotUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image clothingImage;
    [SerializeField] private ClothingSlot clothingSlotType;

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

    public void SetClothingSlot(ClothingItemSO clothingItemSO)
    {
        clothingImage.sprite = clothingItemSO.UISprite;
        clothingSlotType = clothingItemSO.ClothingSlot;
    }
}
