using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;

[Serializable]
[CreateAssetMenu(fileName = "AllowedModifications", menuName = "ScriptableObjects/Items/Allowed Modification List")]
public class AllowedModificationsSO : ScriptableObject
{
    [SerializeField] private ItemID[] allowedModifications;

    public ItemID[] AllowedModifications
    {
        get { return allowedModifications; }
    }
}
