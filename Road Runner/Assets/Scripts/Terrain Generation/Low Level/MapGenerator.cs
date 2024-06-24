using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MapGenerator : MonoBehaviour
{
    [Header("Global")]
    [SerializeField] int globalScaleDown = 512;
    
    [Header("Height")]
    [SerializeField] float[] heightOctaves;
    [SerializeField] float heightRedistributionFactor;
    [SerializeField] int maxHeight;
    
    [Header("Moisture")]
    [SerializeField] float[] moistureOctaves;
    [SerializeField] float moistureRedistributionFactor;
    [SerializeField] int maxMoisture;

    [Header("Strangeness")]
    [SerializeField] float[] strangenessOctaves;
    [SerializeField] float strangenessRedistributionFactor;
    [SerializeField] int maxStrangeness;
    
    [Header("Density")]
    [SerializeField] float[] densityOctaves;
    [SerializeField] float densityRedistributionFactor;
    [SerializeField] int maxDensity;

    public delegate void GenericDelegate();
    public delegate void GenericDelegate<T>(T variable);

    int milisecondsTillYield = 10;

    #region Map Generation

    public void GenerateMaps(int[] seeds, TerrainData terrainData, GenericDelegate finalCallback)
    {
        StartCoroutine(GenerateMapsRoutine(seeds, terrainData, finalCallback));  
    }

    private IEnumerator GenerateMapsRoutine(int[] seeds, TerrainData terrainData, GenericDelegate finalCallback)
    {
        int size = terrainData.Size;

        float[,] heightMap = new float[size, size];
        float[,] moistureMap = new float[size, size];
        float[,] strangenessMap = new float[size, size];
        float[,] densityMap = new float[size, size];

        yield return GenerateGenraricMapRoutine(seeds[0], heightOctaves, heightRedistributionFactor, maxHeight, size, data => { heightMap = data; });

        yield return GenerateGenraricMapRoutine(seeds[1], moistureOctaves, moistureRedistributionFactor, maxMoisture, size, data => { moistureMap = data; });

        yield return GenerateGenraricMapRoutine(seeds[2], strangenessOctaves, strangenessRedistributionFactor, maxStrangeness, size, data => { strangenessMap = data; });

        yield return GenerateGenraricMapRoutine(seeds[3], densityOctaves, densityRedistributionFactor, maxDensity, size, data => { densityMap = data; });

        terrainData.SetMaps(heightMap, moistureMap, strangenessMap, densityMap);

        finalCallback();

        yield return null;
    }

    IEnumerator GenerateGenraricMapRoutine(int seed, float[] octaves, float redistributionFactor, int maxValue, int size, GenericDelegate<float[,]> callback)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();

        float[,] map = new float[size, size];

        Vector2 offset = new Vector2(seed, seed);

        for (int z = 0; z < size; z++)
        {
            if (timer.ElapsedMilliseconds > milisecondsTillYield)
            {
                yield return null;
                timer.Reset();
                timer.Start();
            }

            for (int x = 0; x < size; x++)
            {
                map[x, z] = CompileNoise(x, z, offset, octaves, redistributionFactor) * maxValue;
            }
        }
        
        timer.Stop();

        callback(map);
        yield return null;
    }
    
    float CompileNoise(int x, int z, Vector2 offset, float[] octaves, float redistributionFactor)
    {
        float value = 0;
        float octaveSum = 0f;
        
        float xNorm = (x + offset.x) / globalScaleDown;
        float zNorm = (z + offset.y) / globalScaleDown;
    
        for (int i = 0; i < octaves.Length; i++)
        {
            value += (1/octaves[i]) * CalculateNoise(xNorm, zNorm, octaves[i]);
            octaveSum += 1/octaves[i];
        }
        value /= octaveSum;

        value = Mathf.Pow(value, redistributionFactor);

        return value;
    }
    
    float CalculateNoise(float xNorm, float zNorm, float scale)
    {
        xNorm *= scale;
        zNorm *= scale;

        return Mathf.PerlinNoise(xNorm, zNorm);
    }

    #endregion
}
