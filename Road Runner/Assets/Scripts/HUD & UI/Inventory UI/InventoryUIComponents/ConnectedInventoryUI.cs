using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedInventoryUI : MonoBehaviour
{
    public RectTransform uiContainer;
    public RectTransform slotsContainer;
    public ConnectedInventoryHeader connectedInventoryHeader;
    public SlotButton[,] slotButtons;
}
