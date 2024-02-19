using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UsePointUIElement : InspectPointUIElement
{
    [SerializeField] private Button useButton;
    [SerializeField] private TextMeshProUGUI callToActionText;

    public delegate void GenericDelegate();
    protected GenericDelegate _callBack;

    public void SetUsePoint(string callToAction, GenericDelegate callBack)
    {
        useButton.onClick.AddListener(OnUseClicked);

        _callBack = callBack;

        SetCallToAction(callToAction);
    }

    private void OnUseClicked()
    {
        Debug.Log("Use clicked");
        _callBack.Invoke();
    }

    private void SetCallToAction(string callToAction)
    {
        callToActionText.text = callToAction;
    }
}
