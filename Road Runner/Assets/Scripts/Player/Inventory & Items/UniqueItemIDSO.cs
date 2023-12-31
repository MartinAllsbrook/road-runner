using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/UniqueItemID")]
public class UniqueItemIDSO : ScriptableObject
{
    [SerializeField] public UniqueItemID uniqueItemID;
}
