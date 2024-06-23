using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshCreator : MonoBehaviour
{
    [SerializeField] ProceduralMesh meshPrefab;

    protected void CreateMesh(CatmullRomSpline spline, int resolution, float width,  TerrainData terrainData)
    {
        Vector3[,] vertexPoints = new Vector3[spline.Length * resolution, 3];

        for (int i = 0; i < spline.Length; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float distance = (float)j / resolution + i;

                Vector2 point = spline.GetPointAtDistance(distance);
                Vector2 normal = spline.GetNormalAtDistance(distance);

                Vector2 left = point - normal * width / 2;
                Vector2 right = point + normal * width / 2;
                int row = i * resolution + j;
                vertexPoints[row, 0] = new Vector3(left.x, terrainData.GetHeight(left) + 0.1f, left.y);
                vertexPoints[row, 1] = new Vector3(point.x, terrainData.GetHeight(point) + 0.1f, point.y);
                vertexPoints[row, 2] = new Vector3(right.x, terrainData.GetHeight(right) + 0.1f, right.y);
            }

        }

        ProceduralMesh mesh = Instantiate(meshPrefab, Vector3.zero, Quaternion.identity);
        mesh.CreateMesh(vertexPoints);
    }
}
