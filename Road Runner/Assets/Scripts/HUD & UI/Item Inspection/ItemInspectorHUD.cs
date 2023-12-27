using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInspectorHUD : MonoBehaviour
{
    private InspectPoint[] itemInspectPoints;
    private GameObject[] inspectOverlays;

    [SerializeField] private GameObject inspectOverlayTester;

    public void StartInspectItem(UseableItem item)
    {
        itemInspectPoints = item.GetInspectPoints();

        inspectOverlays = new GameObject[itemInspectPoints.Length];

        for (int i = 0; i < itemInspectPoints.Length; i++)
        {
            Vector3 worldPosition = itemInspectPoints[i].transform.position;
            Vector2 screenPosition = WorldToScreenPosition(worldPosition);

            GameObject newInspectOverlay = Instantiate(inspectOverlayTester, screenPosition, Quaternion.identity, transform);
            inspectOverlays[i] = newInspectOverlay;
        }
    }

    public void StopInspectItem()
    {
        foreach (GameObject inspectOverlay in inspectOverlays)
            Destroy(inspectOverlay);
    }

    private Vector2 WorldToScreenPosition(Vector3 worldPosition)
    {
        /*float minX = image.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = image.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;*/

        Vector2 position = Camera.main.WorldToScreenPoint(worldPosition);

        /*position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);*/

        return position;
    }
}
