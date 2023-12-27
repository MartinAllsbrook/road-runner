using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : ConnectedInventoryUI
{
    protected override void Style(Vector2Int gridDimensions, int slotWidth, bool hasHeader)
    {
        base.Style(gridDimensions, slotWidth, hasHeader);
        slotsContainer.anchoredPosition = new Vector2(0, 0);
    }
}
