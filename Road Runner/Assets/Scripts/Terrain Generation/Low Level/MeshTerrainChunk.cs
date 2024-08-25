using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(MeshFilter))]
public class MeshTerrainChunk : MonoBehaviour
{
    [Header("References")]
    private MapGenerator _mapGenerator;
    private LandmarkGenerator _landmarkGenerator;
    private IslandSmoother _islandSmoother;

    private PointOfInterestGenerator pointOfInterestGenerator;
    private TreeScatter _treeScatter;

    [Header("General Data")]
    private float[,] _heightMap;
    private int[,] _biomeMap;

    [Header("Mesh Data")]
    [SerializeField] private int numSubMeshes;
    [SerializeField] private float uvScale;
    [SerializeField] private int[] textureIndexes;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[][] _triangles;
    private Vector2[] _uvs;

    private ChunkData _chunkData;

/*    public void CreateMaps(int[] noiseSeeds, UnityEvent onFinished, int size, int terrainRadius)
    {
        _mapGenerator = GetComponent<MapGenerator>();
        _landmarkGenerator = GetComponent<LandmarkGenerator>();
        _islandSmoother = GetComponent<IslandSmoother>();

        //Vector2Int worldPosition = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        Vector3 rawPosition = transform.position / (size - 1);
        Vector2Int chunkPosition = new Vector2Int((int) rawPosition.x, (int) rawPosition.z);

        _chunkData = new ChunkData(size, chunkPosition);

*//*        _mapGenerator.GenerateMap(chunkPosition, noiseSeeds, _chunkData, () =>
        {
            _islandSmoother.SmoothHeights(this, terrainRadius);
            onFinished.Invoke();
            //PlaceLandMarks(onFinished, poiSeed, terrainRadius); // Does more than just placing landmarks
        });*//*
    }*/

    public void DecorateAndDraw(float[,] heightMap, int[,] biomeMap, UnityEvent onFinished)
    {
        //GetComponent<AreaBlender>().PlaceAndBlend(ref _chunkData);

        // _chunkData.CalculateBiomes();

        _heightMap = heightMap;
        _biomeMap = biomeMap;

        CreateMesh(_heightMap.GetLength(1));
        UpdateMesh();

        onFinished.Invoke();
    }

    private void CreateMesh(int size)
    {
        _mesh = new Mesh();
        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = _mesh;

        CreateVertecies(size);
        CreateTriangles(size);
        CreateUVs(size);
    }
    private void CreateVertecies(int size)
    {
        _vertices = new Vector3[(size) * (size)];

        for (int i = 0, z = 0; z <= size - 1; z++)
        {
            for (int x = 0; x <= size - 1; x++)
            {
                _vertices[i] = new Vector3(x, _heightMap[x, z], z);
                i++;
            }
        }
    }

    private void CreateTriangles(int size)
    {
        _triangles = new int[numSubMeshes][];
        List<int>[] triangleSet = new List<int>[numSubMeshes];

        for (int i = 0; i < triangleSet.Length; i++)
        {
            triangleSet[i] = new List<int>();
        }
        
        int vertexIndex = 0;
        int trangleIndex = 0;

        for (int z = 0; z < size - 1; z++)
        {
            for (int x = 0; x < size - 1; x++)
            {
                var biomeCode = _biomeMap[x,z];
                
                int setIndex;

                setIndex = textureIndexes[biomeCode];

                triangleSet[setIndex].Add(vertexIndex + 0);
                triangleSet[setIndex].Add(vertexIndex + size);
                triangleSet[setIndex].Add(vertexIndex + 1);
                triangleSet[setIndex].Add(vertexIndex + 1);
                triangleSet[setIndex].Add(vertexIndex + size);
                triangleSet[setIndex].Add(vertexIndex + size + 1);

                vertexIndex++;
                trangleIndex += 6;

            }
            // yield return null;

            vertexIndex++;
        }
        
        for (int i = 0; i < triangleSet.Length; i++)
        {
            _triangles[i] = new int[triangleSet[i].Count];
            for (int j = 0; j < triangleSet[i].Count; j++)
            {
                _triangles[i][j] = triangleSet[i][j];
            }
        }
    }

    private void CreateUVs(int size)
    {
        _uvs = new Vector2[_vertices.Length];
        
        for (int i = 0, z = 0; z <= size - 1; z++)
        {
            for (int x = 0; x <= size - 1; x++)
            {
                _uvs[i] = new Vector2(x * uvScale / (size - 1), z * uvScale / (size - 1));
                i++;
            }
        }
    }

    private void UpdateMesh()
    {
        _mesh.Clear();
        
        _mesh.vertices = _vertices;

        _mesh.subMeshCount = _triangles.Length;
        for (int i = 0; i < _triangles.Length; i++)
        {
            _mesh.SetTriangles(_triangles[i], i);
        }
        _mesh.uv = _uvs;
        
        MeshCollider collider = gameObject.AddComponent<MeshCollider>(); //collider.material = physicMaterial;

        collider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase;
        collider.convex = false;
        collider.sharedMesh = _mesh;
        collider.enabled = true;

        _mesh.RecalculateNormals();
    }

    public void GetChunkDataRef(out ChunkData chunkData)
    {
        chunkData = _chunkData;
        return;
    }
}
