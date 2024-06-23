using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainData
{    
    private int _size;

    // Maps
    private float[,] _heightMap;
    private float[,] _moistureMap;
    private float[,] _strangenessMap;
    private float[,] _densityMap;
    private int[,] _biomeMap;
    private Plane[,] _planes;

    private static Biome[] _biomes;

    public int Size
    {
        get { return _size; }
        private set { }
    }

    public TerrainData(int size)
    {
        if (_biomes == null)
            _biomes = Terrain.Instance.Biomes;

        _size = size;
    }

    public void SetMaps(float[,] heightMap, float[,] moistureMap, float[,] strangenessMap, float[,] densityMap)
    {
        _heightMap = heightMap;
        _moistureMap = moistureMap;
        _strangenessMap = strangenessMap;
        _densityMap = densityMap;

        GeneratePlanes();
    }

    public void GeneratePlanes()
    {
        _planes = new Plane[_size - 1, _size - 1];

        for (int x = 0; x < _size - 1; x++)
        {
            for (int z = 0; z < _size - 1; z++)
            {
                Vector3 a = new Vector3(x, _heightMap[x, z], z);
                Vector3 b = new Vector3(x + 1, _heightMap[x + 1, z], z);
                Vector3 c = new Vector3(x, _heightMap[x, z + 1], z + 1);
                _planes[x, z] = new Plane(a, b, c);
            }
        }
    }

    public void CalculateBiomes()
    {
        _biomeMap = new int[_size, _size];

        for (int x = 0; x < _size; x++)
        {
            for (int z = 0; z < _size; z++)
            {
                _biomeMap[x, z] = FindBiome(x, z);
            }
        }
    }

    private int FindBiome(int x, int z)
    {
        float height = _heightMap[x, z];
        float moisture = _moistureMap[x, z];
        float strangeness = _strangenessMap[x, z];

        Vector3 splot = new Vector3(height, moisture, strangeness);

        float lowestDistance = 100000; // Keeping track of the distance to the closest biome 
        int closestIndex = 0; // Index of the closest biome

        for (int i = 0; i < _biomes.Length; i++)
        {
            float distance = (splot - _biomes[i].GetSplot()).magnitude;

            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    public int GetBiome(int x, int z)
    {
        return _biomeMap[x, z];
    }

    public float GetHeight(float x, float z)
    {
        int xFloor = (int)Mathf.Floor(x);
        int zFloor = (int)Mathf.Floor(z);

        if (x == xFloor && z == zFloor)
            return _heightMap[xFloor, zFloor];

        // Debug.Log("xFloor: " + xFloor + " zFloor: " + zFloor);
        Plane plane = _planes[xFloor, zFloor];
        Ray ray = new Ray(new Vector3(x, 0, z), Vector3.up);
        plane.Raycast(ray, out float y);
        // Debug.Log(y);
        return y;
    }

    public float GetMoisture(float x, float z)
    {
        int xFloor = (int)Mathf.Floor(x);
        int zFloor = (int)Mathf.Floor(z);

        return _moistureMap[xFloor, zFloor];
    }

    public float GetStrangeness(float x, float z)
    {
        int xFloor = (int)Mathf.Floor(x);
        int zFloor = (int)Mathf.Floor(z);

        return _strangenessMap[xFloor, zFloor];
    }

    public float GetDensity(float x, float z)
    {
        int xFloor = (int)Mathf.Floor(x);
        int zFloor = (int)Mathf.Floor(z);

        return _densityMap[xFloor, zFloor];
    }

    public float GetSlope(float x, float z)
    {
        int xFloor = (int)Mathf.Floor(x);
        int zFloor = (int)Mathf.Floor(z);

        Vector3 normal = -_planes[xFloor, zFloor].normal;

        return Vector3.Angle(normal, Vector3.up);
    }

    public Vector3 GetNormal(float x, float z)
    {
        int xFloor = (int)Mathf.Floor(x);
        int zFloor = (int)Mathf.Floor(z);

        return -_planes[xFloor, zFloor].normal;
    }

    public void SetHeight(int x, int z, float height)
    {
        _heightMap[x, z] = height;
    }

    public void SetMoisture(int x, int z, float moisture)
    {
        _moistureMap[x, z] = moisture;
    }

    public void SetDensity(int x, int z, float density)
    {
        _densityMap[x, z] = density;
    }

}