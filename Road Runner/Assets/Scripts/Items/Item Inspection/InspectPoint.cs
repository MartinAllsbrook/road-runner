using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InspectPoint : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [SerializeField] private InspectPointUIElement inspectPointUIElement;

    [Header("Inspect Point Settings")]
    [SerializeField] private string inspectPointName;
    public string InspectPointName
    {
        get { return inspectPointName; }
    }
    [SerializeField] private string inspectPointDescription;
    public string InspectPointDescription
    {
        get { return inspectPointDescription; }
    }
    [SerializeField] private PointType inspectPointType;
    public PointType InspectPointType
    {
        get { return inspectPointType; }
    }

    protected InspectPointUIElement _uiElement;
    #endregion

    public enum PointType
    {
        Inspector,
        User,
        Consumer,
        Transformer,
        Modifier,
        Adder
    }

    public InspectPointUIElement CreateInspectHUDElement(Transform hudTransform)
    {

        Vector3 worldPosition = transform.position;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
         
        _uiElement = Instantiate(inspectPointUIElement, screenPosition, Quaternion.identity, hudTransform);
        _uiElement.GenericSet(this);

        return _uiElement;
    }

}
