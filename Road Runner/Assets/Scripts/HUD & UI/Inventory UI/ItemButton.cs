using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Button rootButton;

    public void Set(Vector2Int dimensions, Vector2 position, float inventorySlotWidth, Sprite itemSprite)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(inventorySlotWidth * dimensions.x, inventorySlotWidth * dimensions.y);
        rectTransform.anchoredPosition = position;
        itemImage.sprite = itemSprite;
    }

    public Button GetButton()
    {
        return rootButton;
    }
}
