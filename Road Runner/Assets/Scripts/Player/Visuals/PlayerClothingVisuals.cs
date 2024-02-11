using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerClothingVisuals : NetworkBehaviour
{
    [SerializeField] Transform[] clothingRoots;

    private UniqueItemModel[] clothingModels = new UniqueItemModel[8];

    public void UpdateClothingModel(int slot, int itemID)
    {
        UpdateClothingModelServerRpc(slot, itemID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateClothingModelServerRpc(int slot, int itemID)
    {
        UpdateClothingModelClientRpc(slot, itemID);
    }

    [ClientRpc]
    private void UpdateClothingModelClientRpc(int slot, int itemID)
    {
        if (clothingModels[slot] != null)
        {
            Destroy(clothingModels[slot].gameObject);
        }

        UniqueItemModel clothingModel = GlobalItemDictionary.ItemSODictionary[(GlobalItemDictionary.ItemID)itemID].ModelPrefab;

        Transform root = clothingRoots[slot];
        clothingModels[slot] = Instantiate(clothingModel, root.position, root.rotation, root);

        if (IsOwner)
        {
            clothingModels[slot].gameObject.layer = 6; // LocalPlayer Layer / Hidden Layer

            int childCount = clothingModels[slot].transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                clothingModels[slot].transform.GetChild(i).gameObject.layer = 6; // LocalPlayer Layer / Hidden Layer
            }
        }
    }

    public void RemoveClothingModel(int slot)
    {
        RemoveClothingModelServerRpc(slot);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveClothingModelServerRpc(int slot)
    {
        RemoveClothingModelClientRpc(slot);
    }

    [ClientRpc]
    private void RemoveClothingModelClientRpc(int slot)
    {
        if (clothingModels[slot] != null)
        {
            Destroy(clothingModels[slot].gameObject);
        }
    }
}
