using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class RiverTester : MonoBehaviour
{
    [SerializeField] GameObject testingObject;
    [SerializeField] GameObject testingObjectPoint;

    [SerializeField] int drawResolution = 100;
    [SerializeField] int drawHeight = 100;
    public void DrawRiver(CatmullRomSpline spline)
    {
        for (int i = 0; i < drawResolution; i++)
        {
            float t = i / (float)drawResolution;
            Vector2 point = spline.GetPointFromPercent(t);
            Vector3 point3D = new Vector3(point.x, drawHeight, point.y);

            Instantiate(testingObject, point3D, Quaternion.identity);
        }
    }

    public void DrawPoint(Vector2 point)
    {
        Vector3 point3D = new Vector3(point.x, drawHeight, point.y);

        Instantiate(testingObjectPoint, point3D, Quaternion.identity);
    }
}
