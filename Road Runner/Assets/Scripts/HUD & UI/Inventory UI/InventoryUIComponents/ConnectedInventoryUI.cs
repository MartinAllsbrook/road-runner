using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedInventoryUI : MonoBehaviour
{
    [SerializeField] protected RectTransform uiContainer;
    [SerializeField] protected RectTransform slotsContainer;
    [SerializeField] protected ConnectedInventoryHeader connectedInventoryHeader;

    protected SlotButton[,] slotButtons;
    protected int _inventoryKey;
    protected int _slotWidth;

    // TODO: Make some sort of inventory data type to organize and pass info
    // TODO: And / Or get slot width from inventorySlotPrefab
    public void Set(int inventoryKey, Vector2Int dimensions, SlotButton inventorySlotPrefab, int slotWidth, bool hasHeader, string name)
    {
        _inventoryKey = inventoryKey;
        _slotWidth = slotWidth;

        int width = dimensions.x;
        int height = dimensions.y;

        SetHeader(name, slotWidth, hasHeader);

        CreateSlotButtons(width, height, inventorySlotPrefab, slotWidth);

        Style(dimensions, slotWidth, hasHeader);
    }

    private void SetHeader(string name, int slotWidth, bool hasHeader)
    {
        // TODO: We really only need to define a height if eaven that
        if (hasHeader)
            connectedInventoryHeader.Set(_inventoryKey, name); 
        else
            connectedInventoryHeader.gameObject.SetActive(false);
    }

    protected virtual void CreateSlotButtons(int width, int height, SlotButton inventorySlotPrefab, int slotWidth)
    {
        slotButtons = new SlotButton[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SlotButton slotButton = Instantiate(inventorySlotPrefab, slotsContainer);

                slotButton.Set(_inventoryKey, new Vector2Int(x, y), slotWidth);

                slotButtons[x, y] = slotButton;
            }
        }
    }

    protected virtual void Style(Vector2Int gridDimensions, int slotWidth, bool hasHeader)
    {
        Vector2Int slotsContainerDimensions = gridDimensions * slotWidth;

        Vector2Int uiContainerDimensions = slotsContainerDimensions;
        if (hasHeader)
            uiContainerDimensions.y += slotWidth;

        slotsContainer.sizeDelta = slotsContainerDimensions;
        uiContainer.sizeDelta = uiContainerDimensions;
    }

    #region Button Control Methods

    public void HideArea(Vector2Int topLeft, Vector2Int area)
    {
        int width = area.x;
        int height = area.y;

        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                int x = topLeft.x + xi;
                int y = topLeft.y + yi;

                SetSlotActive(new Vector2Int(x, y), false);
            }
        }
    }

    public void ShowArea(Vector2Int topLeft, Vector2Int area)
    {
        int width = area.x;
        int height = area.y;

        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                int x = topLeft.x + xi;
                int y = topLeft.y + yi;

                if (IsSlotActive(new Vector2Int(x, y)))
                {
                    Debug.LogError("Tried to activate an active button in inventory " + _inventoryKey + " at slot " + x + ", " + y + ". This suggests there are issues in the inventory code...");
                }

                SetSlotActive(new Vector2Int(x, y), true);
            }
        }
    }

    public void ShowAll()
    {
        foreach (SlotButton slotButton in slotButtons)
        {
            slotButton.gameObject.SetActive(true);
        }
    }

    // Helper Methods
    private void SetSlotActive(Vector2Int position, bool active)
    {
        slotButtons[position.x, position.y].gameObject.SetActive(active);
    }

    private bool IsSlotActive(Vector2Int position)
    {
        return slotButtons[position.x, position.y].gameObject.activeSelf;
    }

    #endregion

    public void SetYOffset(int yPosition)
    {
        Vector2Int position = new Vector2Int(0, -yPosition);

        uiContainer.anchoredPosition = position;
    }

    public int GetHeight()
    {
        int height = slotButtons.GetLength(1);

        if (connectedInventoryHeader.gameObject.activeSelf)
            height++;

        height *= _slotWidth;

        return height;
    }



    public RectTransform GetSlotsRect()
    {
        return slotsContainer;
    }
}
