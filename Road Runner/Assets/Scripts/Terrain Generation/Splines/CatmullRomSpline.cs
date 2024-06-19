using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomSpline
{
    int splineLength;
    int numPoints;

    Vector2[] points;

    public CatmullRomSpline()
    {
        points = null;
    }

    public CatmullRomSpline(Vector2[] points)
    {
        SetPoints(points);
    }

    public void SetPoints(Vector2[] points)
    {
        Debug.Log("Setting points");

        if (points.Length < 4)
        {
            Debug.LogError("CatmullRomSpline requires at least 4 points to be set.");
            return;
        }

        numPoints = points.Length;
        splineLength = numPoints - 3;

        this.points = points;
    }

    public Vector2 GetPointFromPercent(float t)
    {
        if (points == null)
        {
            Debug.LogError("CatmullRomSpline has no points set.");
            return Vector2.zero;
        }

        if (t < 0)
        {
            Debug.LogWarning("CatmullRomSpline t < 0, setting t = 0");
            t = 0;
        }
        else if (t >= 1)
        {
            Debug.LogWarning("CatmullRomSpline t > 1, setting t = 1");
            t = 0.99f;
        }

        int i = Mathf.FloorToInt(t * splineLength);
        t = t * splineLength - i;

        Vector2 p0 = points[i];
        Vector2 p1 = points[i + 1];
        Vector2 p2 = points[i + 2];
        Vector2 p3 = points[i + 3];

        Debug.Log(p1);

        return GetPoint(t, p0, p1, p2, p3);
    }

    private Vector2 GetPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return 
            0.5f * (
                (2 * p1) +
                t * (-p0 + p2) +
                t * t * (2 * p0 - 5 * p1 + 4 * p2 - p3) +
                t * t * t * (-p0 + 3 * p1 - 3 * p2 + p3) 
            );
    }
}
