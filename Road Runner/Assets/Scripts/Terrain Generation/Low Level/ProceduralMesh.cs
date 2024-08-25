using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralMesh : MonoBehaviour
{
    [SerializeField] bool test = false;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    private void Start()
    {
        if (test)
        {
            CreateMesh(new Vector3[3, 3] { 
                { new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 0, 2) }, 
                { new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 2) }, 
                { new Vector3(2, 0, 0), new Vector3(2, 0, 1), new Vector3(2, 0, 2) } 
            });
        }
    }

    public void CreateMesh(Vector3[,] vertexPoints)
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        CreateVertecies(vertexPoints);
        CreateTriangles(vertexPoints);
        CreateUVs(vertexPoints);
    }

    private void CreateVertecies(Vector3[,] vertexPoints)
    {
        vertices = new Vector3[vertexPoints.GetLength(0) * vertexPoints.GetLength(1)];

        for (int i = 0, z = 0; z < vertexPoints.GetLength(1); z++)
        {
            for (int x = 0; x < vertexPoints.GetLength(0); x++)
            {
                vertices[i] = vertexPoints[x, z];
                i++;
            }
        }

        mesh.vertices = vertices; // Don't know if this is necessary yet
    }

    private void CreateTriangles(Vector3[,] vertexPoints)
    {
        triangles = new int[(vertexPoints.GetLength(0) - 1) * (vertexPoints.GetLength(1) - 1) * 6];
        int vertexIndex = 0;
        int trangleIndex = 0;

        for (int z = 0; z < vertexPoints.GetLength(1) - 1; z++)
        {
            for (int x = 0; x < vertexPoints.GetLength(0) - 1; x++)
            {
                triangles[trangleIndex + 0] = vertexIndex + 0;
                triangles[trangleIndex + 1] = vertexIndex + vertexPoints.GetLength(0);
                triangles[trangleIndex + 2] = vertexIndex + 1;
                triangles[trangleIndex + 3] = vertexIndex + 1;
                triangles[trangleIndex + 4] = vertexIndex + vertexPoints.GetLength(0);
                triangles[trangleIndex + 5] = vertexIndex + vertexPoints.GetLength(0) + 1;

                vertexIndex++;
                trangleIndex += 6;
            }
            vertexIndex++;
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void CreateUVs(Vector3[,] vertexPoints)
    {
        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z < vertexPoints.GetLength(1); z++)
        {
            for (int x = 0; x < vertexPoints.GetLength(0); x++)
            {
                uvs[i] = new Vector2((float)x / vertexPoints.GetLength(0), (float)z / vertexPoints.GetLength(1));
                i++;
            }
        }

        mesh.uv = uvs;
    }
}
