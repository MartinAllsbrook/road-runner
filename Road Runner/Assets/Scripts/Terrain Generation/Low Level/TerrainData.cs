using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TerrainData
{    
    private int _size;
    public int Size { get { return _size; } }

    private int _outerRadius;
    public int OuterRadius { get { return _outerRadius; } set { _outerRadius = value; } }

    private int _innerRadius;
    public int InnerRadius { get { return _innerRadius; } set { _innerRadius = value; } }

    private Vector2Int[] peaks;
    public Vector2Int[] Peaks { get { return peaks; } }


    // Maps
    private float[,] _heightMap;
    private float[,] _moistureMap;
    private float[,] _strangenessMap;
    private float[,] _densityMap;
    private int[,] _biomeMap;
    private Plane[,] _planes;

    private static Biome[] _biomes;


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

    #region Peaks
    public void FindPeaks(int numPeaks, int seed, int peakSpacing, int seaLevel, int peakStepDistance)
    {
        Random random = new Random(seed);

        List<Vector2Int> peakList = new List<Vector2Int>();
        
        int numTries = 0;
        int maxTries = 100;

        for (int i = 0; i < numPeaks; i++)
        {
            numTries++;
            if (numTries > maxTries)
            {
                Debug.LogError("Max tries reached for finding peaks.");
                break;
            }

            Vector2Int point = new Vector2Int(random.Next(_size), random.Next(_size)); // TODO: this could be optimized so it picks points within inner radius
            if (GetHeight(point) < seaLevel)
            {
                i--;
                continue;
            }

            Vector2Int newPeak = FindPeakFrom(point, peakStepDistance);

            bool uniquePeak = true;
            foreach (Vector2Int peak in peakList)
            {
                if (Vector2Int.Distance(peak, newPeak) < peakSpacing)
                {
                    uniquePeak = false;
                    break;
                }
            }

            if (!uniquePeak)
            {
                i--;
                continue;
            }

            peakList.Add(newPeak);
        }

        peaks = peakList.ToArray();
    }

    private Vector2Int FindPeakFrom(Vector2Int point, int stepDistance)
    {
        Vector2Int highestPoint = point;
        float highestHeight = _heightMap[point.x, point.y];

        Vector2Int nextPoint = point;
        float height = highestHeight;

        int stepsTaken = 0;
        int maxSteps = 250;

        while (height >= highestHeight && highestHeight > 1)
        {
            stepsTaken++;
            if (stepsTaken > maxSteps)
            {
                Debug.LogWarning("Max steps taken to find peak.");
                break;
            }

            highestPoint = nextPoint;
            highestHeight = height;

            nextPoint = GoUpSlope(highestPoint, stepDistance);
            height = _heightMap[nextPoint.x, nextPoint.y];

            //Debug.Log("Highest height: " + highestHeight + ", Highest point: " + highestPoint + ", Next Height: " + height + ", Next Point: " + nextPoint);
        }

        return highestPoint;
    }


    private Vector2Int GoUpSlope(Vector2Int point, int stepDistance)
    {
        Plane plane = _planes[point.x, point.y];

        Vector3 normal = plane.normal;
        Vector2 roughDirection = new Vector2(normal.x, normal.z);
        roughDirection.Normalize();

        Vector2Int direction = new Vector2Int(Mathf.RoundToInt(roughDirection.x * stepDistance), Mathf.RoundToInt(roughDirection.y * stepDistance));
        
        return point + direction;
    }

    #endregion

    #region Biomes
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

    #endregion

    public float GetHeight(Vector2 point)
    {
        return GetHeight(point.x, point.y);
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

    public float GetHeight(Vector2Int point)
    {
        return GetHeight(point.x, point.y);
    }

    public float GetHeight(int x, int z)
    {
        return _heightMap[x, z];
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