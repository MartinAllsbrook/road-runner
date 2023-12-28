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
    private InspectPointUIElement[] inspectOverlays;

    [SerializeField] private InspectPointUIElement inspectOverlayTester;

    public void StartInspectItem(UseableItem item)
    {
        itemInspectPoints = item.InspectPoints;

        inspectOverlays = new InspectPointUIElement[itemInspectPoints.Length];

        for (int i = 0; i < itemInspectPoints.Length; i++)
        {
            CreateInspectHUDElement(i);
        }
    }

    private void CreateInspectHUDElement(int index)
    {
        InspectPoint inspectPoint = itemInspectPoints[index];

        Vector3 worldPosition = inspectPoint.transform.position;
        Vector2 screenPosition = WorldToScreenPosition(worldPosition);

        PointType pointType = inspectPoint.InspectPointType;
        // TODO: Create a switch statement to determine which inspect overlay to create based on the point type

        InspectPointUIElement newInspectOverlay = Instantiate(inspectOverlayTester, screenPosition, Quaternion.identity, transform);
        newInspectOverlay.SetPoint(inspectPoint.InspectPointName, inspectPoint.InspectPointDescription);

        inspectOverlays[index] = newInspectOverlay;
    }

    public void StopInspectItem()
    {
        foreach (InspectPointUIElement inspectOverlay in inspectOverlays)
            Destroy(inspectOverlay.gameObject);
    }

    private Vector2 WorldToScreenPosition(Vector3 worldPosition)
    {
        Vector2 position = Camera.main.WorldToScreenPoint(worldPosition);

        return position;
    }
}
