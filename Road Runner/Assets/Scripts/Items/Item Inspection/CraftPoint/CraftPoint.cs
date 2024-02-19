using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalItemDictionary;

public class CraftPoint : PointWithInputs
{
    [SerializeField] private CraftingRecipiesSO craftingRecipies;

    private CraftPointUIElement craftPointUI;

    private List<StoredItemID> currentItems = new List<StoredItemID>();

    public override InspectPointUIElement CreateInspectHUDElement(Transform hudTransform)
    {
        craftPointUI = base.CreateInspectHUDElement(hudTransform) as CraftPointUIElement;

        craftPointUI.InitializeCraftUI(TryCraft);

        return craftPointUI;
    }

    public override void DestroyUIElement()
    {
        StoredItemID[] currentItemsCopy = currentItems.ToArray();
        foreach (StoredItemID item in currentItemsCopy)
        {
            RemoveItemFromList(item);
        }
        base.DestroyUIElement();
    }

    // Add the item to the current items list
    public override void SelectOption(StoredItemID item)
    {
        // Add the item to the current items list
        currentItems.Add(item);
        craftPointUI.AddItemToInputUI(item, RemoveItemFromList);

        // Remove the item from the inventory
        Inventory.Instance.RemoveItem(item);

        base.SelectOption(item);
    }

    private void RemoveItemFromList(StoredItemID item)
    {
        currentItems.Remove(item);
        craftPointUI.AddItemOptionUI(item, SelectOption);
        craftPointUI.RemoveItemFromInputUI(item);
        // Remove the item from the inventory
        Inventory.Instance.AddItem(item.UniqueItemID);
    }

    public void TryCraft()
    {
        CraftingRecipie[] recipies = craftingRecipies.CraftingRecipies;

        foreach (CraftingRecipie recipie in recipies)
        {
            // If the recipie requires more items than we have, skip it
            if (recipie.RequiredItems.Length != currentItems.Count) 
            {
                continue;
            }

            // Create a copy of the current items list as ItemID (Also allows us to remove items from the list)
            List<ItemID> currentItemsCopy = new List<ItemID>();
            foreach (StoredItemID item in currentItems)
            {
                currentItemsCopy.Add(item.UniqueItemID.BaseItemID);
            }

            // Check if all the required items are in the current items list
            bool allItemsMatch = true;
            ItemID[] requiredItems = recipie.RequiredItems;
            for (int i = 0; i < requiredItems.Length; i++) 
            {
                if (currentItemsCopy.Contains(requiredItems[i]))
                {
                    currentItemsCopy.Remove(requiredItems[i]);
                }
                else
                {
                    allItemsMatch = false;
                    break; // No need to continue checking if an item is missing
                }
            }

            // If so, craft the item
            if (allItemsMatch)
            {
                CraftItem(recipie);
                break; // We only want to craft one item at a time
            }
        }
    }

    private void CraftItem(CraftingRecipie recipie)
    {
        foreach (StoredItemID item in currentItems)
        {
            craftPointUI.RemoveItemFromInputUI(item);
        }
        // Clear current items list (Items were removed from the inventory when added to list)
        currentItems.Clear();

        Inventory.Instance.AddItem(new UniqueItemID(recipie.ResultItem));
    }
}
