using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UsePoint : InspectPoint
{
    [Header("Use Point")]
    [SerializeField] private string callToAction;

    [SerializeField] public UnityEvent OnUse = new UnityEvent();

    public override InspectPointUIElement CreateInspectHUDElement(Transform hudTransform)
    {
        UsePointUIElement usePointUIElement = (UsePointUIElement) base.CreateInspectHUDElement(hudTransform); 
        
        usePointUIElement.SetUsePoint(callToAction, Use);
        return usePointUIElement;
    }

    public virtual void Use()
    {
        OnUse.Invoke();
    }
}
