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

    private UnityEvent pointOnUseEvent;
    public override void GenericSet<T>(T point)
    {
        base.GenericSet(point);

        UsePoint usePoint = point as UsePoint;

        pointOnUseEvent = usePoint.OnUse;
        SetUsePoint(usePoint.CallToAction);
    }
    private void SetUsePoint(string callToAction)
    {
        useButton.onClick.AddListener(OnUseClicked);

        SetCallToAction(callToAction);
    }

    private void OnUseClicked()
    {
        Debug.Log("Use clicked");
        pointOnUseEvent.Invoke();
    }

    private void SetCallToAction(string callToAction)
    {
        callToActionText.text = callToAction;
    }
}
