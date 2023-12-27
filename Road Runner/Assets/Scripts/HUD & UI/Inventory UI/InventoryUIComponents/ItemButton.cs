using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : InventoryUIButton
{
    [SerializeField] private Image itemImage;

    private int _inventoryKey;
    private int _itemKey;

    public void Set(int inventoryKey, int itemKey, Vector2Int dimensions, Vector2Int position, int inventorySlotWidth, Sprite itemSprite)
    {
        _inventoryKey = inventoryKey;
        _itemKey = itemKey;

        itemImage.sprite = itemSprite;

        position.y = -position.y;
        StyleRect(position, dimensions * inventorySlotWidth);

        AddListener();
    }

    protected override void OnClick()
    {
        base.OnClick();

        Inventory.Instance.RetrieveItem(_inventoryKey, _itemKey);
    }
}
