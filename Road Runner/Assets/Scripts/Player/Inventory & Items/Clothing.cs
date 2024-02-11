using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothing
{
    private ClothingData[] wornClothingData;

    #region Constructors

    public Clothing(int numClothingSlots)
    {
        wornClothingData = new ClothingData[numClothingSlots];
        for (int i = 0; i < wornClothingData.Length; i++)
        {
            wornClothingData[i] = null;
        }
    }

    #endregion

    public void EquipClothingItem(ClothingData clothingData)
    {
        wornClothingData[(int)clothingData.ClothingSlot] = clothingData;
        LocalPlayerStats.Instance.ChangeResistances(clothingData.Resistances);
        Player.LocalInstance.PlayerClothingVisuals.UpdateClothingModel((int)clothingData.ClothingSlot, (int)clothingData.BaseItemID);
    }

    public ClothingData UnequipClothingItem(int clothingSlot)
    {
        ClothingData clothingData = wornClothingData[clothingSlot];
        wornClothingData[clothingSlot] = null;
        return clothingData;
    }

    public void UnequipClothingItem(ClothingData clothingData)
    {
        wornClothingData[(int)clothingData.ClothingSlot] = null;
        LocalPlayerStats.Instance.ChangeResistances(clothingData.Resistances, true);
        Player.LocalInstance.PlayerClothingVisuals.RemoveClothingModel((int)clothingData.ClothingSlot);

    }

    public ClothingData[] GetWornClothingData()
    {
        return wornClothingData;
    }
}
