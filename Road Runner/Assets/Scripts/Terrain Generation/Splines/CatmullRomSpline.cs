using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomSpline
{
    int splineLength;
    public int Length { get { return splineLength; } }

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

    public Vector2 GetPointFromPercent(float percent)
    {
        return GetPointAtDistance(splineLength * percent);
    }

    public Vector2 GetPointAtDistance(float distance)
    {
        if (points == null)
        {
            Debug.LogError("CatmullRomSpline has no points set.");
            return Vector2.zero;
        }

        distance = Mathf.Clamp(distance, 0, splineLength);
        int i = Mathf.FloorToInt(distance);
        float t = distance - i;

        Vector2 p0 = points[i];
        Vector2 p1 = points[i + 1];
        Vector2 p2 = points[i + 2];
        Vector2 p3 = points[i + 3];

        return GetPoint(t, p0, p1, p2, p3);
    }

    public Vector2 GetTangentAtDistance(float distance)
    {
        float epsilon = 0.0001f;

        Vector2 p1 = GetPointAtDistance(distance - epsilon);
        Vector2 p2 = GetPointAtDistance(distance + epsilon);

        return (p2 - p1).normalized;
    }

    public Vector2 GetNormalAtDistance(float distance)
    {
        Vector2 tangent = GetTangentAtDistance(distance);
        return new Vector2(-tangent.y, tangent.x);
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
