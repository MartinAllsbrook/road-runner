using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClothingVisuals : MonoBehaviour
{
    [SerializeField] Transform[] clothingRoots;

    private UniqueItemModel[] clothingModels = new UniqueItemModel[8];

    public void UpdateClothingModel(int slot, int itemID)
    {
        UniqueItemModel clothingModel = GlobalItemDictionary.ItemSODictionary[(GlobalItemDictionary.ItemID)itemID].ModelPrefab;
    
        if (clothingModels[slot] != null)
        {
            Destroy(clothingModels[slot].gameObject);
        }

        Transform root = clothingRoots[slot];
        clothingModels[slot] = Instantiate(clothingModel, root.position, root.rotation, root);
    }
}
