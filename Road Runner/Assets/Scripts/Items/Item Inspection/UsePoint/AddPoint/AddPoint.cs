using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPoint : UsePoint
{
    [SerializeField] private UseableItem useableItem; // TODO: Make this a general counter class

    public override InspectPointUIElement CreateInspectHUDElement(Transform hudTransform)
    {
        AddPointUIElement uiElement = base.CreateInspectHUDElement(hudTransform) as AddPointUIElement;
        
        uiElement.SetCount(GetCount());

        return uiElement;
    }

    public int GetCount()
    {
        return useableItem.UniqueItemID.CounterCount;
    }

    public override void Use()
    {
        base.Use();

        AddPointUIElement uiElement = _uiElement as AddPointUIElement;
        uiElement.SetCount(GetCount());
    }
}
