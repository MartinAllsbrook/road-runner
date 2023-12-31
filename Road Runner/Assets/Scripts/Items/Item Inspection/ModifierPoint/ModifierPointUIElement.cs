using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;
using static ConnectedInventory;

public class ModifierPointUIElement : InspectPointUIElement
{
    [SerializeField] private ModifierPointOption itemOptionPrefab;
    [SerializeField] private Transform itemOptionParent;

    public override void GenericSet<T>(T point)
    {
        if (!_set)
        {
            base.GenericSet(point);
        }

        ModifierPoint modifierPoint = point as ModifierPoint;

        Debug.Log("Modifier point: " + modifierPoint);

        ContainedItem[] itemOptions = modifierPoint.ItemsThatFit;
        foreach (ContainedItem item in itemOptions)
        {
            Sprite sprite = ItemSODictionary[item.inventoryItem].UISprite;
            SpawnItemOptionUI(sprite, item.count, item, modifierPoint);
        }
    }

    private void SpawnItemOptionUI(Sprite itemSprite, int count, ContainedItem item, ModifierPoint point)
    {
        ModifierPointOption itemOption = Instantiate(itemOptionPrefab, itemOptionParent);
        itemOption.SetItemOption(itemSprite, count, item, point);
    }
}
