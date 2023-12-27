using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClothingSlotUI;

[Serializable]
[CreateAssetMenu(fileName = "ClothingPiece", menuName = "ScriptableObjects/ClothingItem")]

public class ClothingItemSO : ItemSO
{
    [Header("Clothing Stuff")]
    [SerializeField] private ClothingSlot clothingSlot;
    public ClothingSlot ClothingSlot { get { return clothingSlot; } }
 
    [SerializeField] private GameObject clothingPrefab;
    public GameObject ClothingPrefab { get { return clothingPrefab; } }
    
    [SerializeField] private Vector2Int clothingInventoryDimensions;
    public Vector2Int ClothingInventoryDimensions { get { return clothingInventoryDimensions; } }
}
