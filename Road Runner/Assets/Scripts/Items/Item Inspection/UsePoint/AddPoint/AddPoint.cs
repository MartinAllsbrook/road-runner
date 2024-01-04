using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPoint : UsePoint
{
    [SerializeField] private UseableItem useableItem; // TODO: Make this a general counter class

    public int GetCount()
    {
        return useableItem.UniqueItemID.CounterCount;
    }

    public override void Use()
    {
        base.Use();

        _uiElement.GenericSet(this); // A bit of a hacky way to get the UI to update but I like it for now
    }
}
