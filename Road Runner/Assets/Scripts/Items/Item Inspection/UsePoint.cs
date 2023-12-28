using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsePoint : InspectPoint
{
    [Header("Use Point")]
    [SerializeField] private Button useButton;
    public Button UseButton
    {
        get { return useButton; }
    }

    [SerializeField] private string callToAction;
    public string CallToAction
    {
        get { return callToAction; }
    }
}
