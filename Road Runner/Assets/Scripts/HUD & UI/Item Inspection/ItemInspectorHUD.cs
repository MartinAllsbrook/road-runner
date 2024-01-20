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
    private InspectPointUIElement[] pointUIs;

    [SerializeField] private InspectPointUIElement inspectOverlayTester;

    public void StartInspectItem(UseableItem item)
    {
        itemInspectPoints = item.InspectPoints;

        pointUIs = new InspectPointUIElement[itemInspectPoints.Length];

        for(int i = 0; i < itemInspectPoints.Length; i++)
        {
            pointUIs[i] = itemInspectPoints[i].CreateInspectHUDElement(transform);
        }
    }

    public void StopInspectItem()
    {
        foreach (InspectPointUIElement inspectOverlay in pointUIs)
            Destroy(inspectOverlay.gameObject);
    }
}
