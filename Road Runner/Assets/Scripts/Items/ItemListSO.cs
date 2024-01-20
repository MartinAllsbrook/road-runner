using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "0_ItemList", menuName = "ScriptableObjects/Items/ItemList")]
public class ItemListSO : ScriptableObject
{
    [SerializeField] protected ItemSO[] itemScriptableObjects;

    public ItemSO[] ItemSOs
    {
        get { return itemScriptableObjects; }
    }
}