using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalItemDictionary;

[Serializable]
[CreateAssetMenu(fileName = "NewItemInputList", menuName = "Items/Allowed Input List")]
public class AllowedItemInputsSO : ScriptableObject
{
    [SerializeField] private ItemID[] allowedModifications;

    public ItemID[] AllowedModifications
    {
        get { return allowedModifications; }
    }
}
