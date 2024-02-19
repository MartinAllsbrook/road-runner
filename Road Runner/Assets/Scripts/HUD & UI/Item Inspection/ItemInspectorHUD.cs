using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectPoint;

/// <summary>
/// This class is used to display the inspect points of the item being inspected on the HUD
/// </summary>
public class ItemInspectorHUD : MonoBehaviour
{
    private InspectPoint[] itemInspectPoints;

    [SerializeField] private InspectPointUIElement inspectOverlayTester;

    public void StartInspectItem(UseableItem item)
    {
        itemInspectPoints = item.InspectPoints;


        foreach (InspectPoint point in itemInspectPoints)
            point.CreateInspectHUDElement(transform);
    }

    public void StopInspectItem()
    {
        foreach (InspectPoint point in itemInspectPoints)
            point.DestroyUIElement();
    }
}
