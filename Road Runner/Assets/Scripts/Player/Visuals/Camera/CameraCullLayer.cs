using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCullLayer : MonoBehaviour
{
    [SerializeField] private int mediumScatterLayer;
    [SerializeField] private float mediumCullDistance;
    [SerializeField] private int smallScatterLayer;
    [SerializeField] private float smallCullDistance;

    void Start()
    {
        Camera camera = GetComponent<Camera>();
        float[] distances = new float[32];
        distances[mediumScatterLayer] = mediumCullDistance;
        distances[smallScatterLayer] = smallCullDistance;
        camera.layerCullDistances = distances;
    }
}
