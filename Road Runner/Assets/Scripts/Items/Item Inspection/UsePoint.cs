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
    public string CallToAction
    {
        get { return callToAction; }
    }

    [SerializeField] public UnityEvent OnUse = new UnityEvent();

    public virtual void Use()
    {
        OnUse.Invoke();
    }
}
