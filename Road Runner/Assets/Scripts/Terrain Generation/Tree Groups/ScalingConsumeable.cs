using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalItemDictionary;

public class ScalingConsumeable : InteractiveScatter
{
    [SerializeField] private float scaleFactor = 0.2f;

    [SerializeField] private ItemID rewardItemID;
    public override void Interact()
    {
        Inventory.Instance.AddItem(new UniqueItemID(rewardItemID));
        base.Interact();
    }

    public override void ConsumeAction()
    {
        transform.localScale = transform.localScale * scaleFactor;
    }

    public override void ReactivateAction()
    {
        transform.localScale = transform.localScale / scaleFactor;
    }
}
