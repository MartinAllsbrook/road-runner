
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SplineTester : MonoBehaviour
{
    [SerializeField] Transform[] pointTransforms;

    CatmullRomSpline spline;

    [SerializeField] GameObject testingObject;
    [SerializeField] int drawResolution = 100;

    GameObject[] testObjects;

    // Start is called before the first frame update
    void Start()
    {
        // Create a CatmullRomSpline from the points
        Vector2[] points = new Vector2[pointTransforms.Length];
        
        for (int i = 0; i < pointTransforms.Length; i++)
        {
            points[i] = new Vector2(pointTransforms[i].position.x, pointTransforms[i].position.z);
        }

        spline = new CatmullRomSpline(points);

        // Create the test objects
        testObjects = new GameObject[drawResolution];

        for (int i = 0; i < drawResolution; i++)
        {
            float t = i / (float)drawResolution;
            Vector2 point = spline.GetPointFromPercent(t);
            Vector3 point3D = new Vector3(point.x, 0, point.y);

            testObjects[i] = Instantiate(testingObject, point3D, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2[] points = new Vector2[pointTransforms.Length];

        for (int i = 0; i < pointTransforms.Length; i++)
        {
            points[i] = new Vector2(pointTransforms[i].position.x, pointTransforms[i].position.z);
        }

        spline.SetPoints(points);

        for (int i = 0; i < drawResolution; i++)
        {
            float t = i / (float)drawResolution;
            Vector2 point = spline.GetPointFromPercent(t);
            Vector3 point3D = new Vector3(point.x, 0, point.y);

            testObjects[i].transform.position = point3D;
        }
    }
}
