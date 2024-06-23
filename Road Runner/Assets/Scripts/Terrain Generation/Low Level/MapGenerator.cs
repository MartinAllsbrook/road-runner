using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MapGenerator : MonoBehaviour
{
    [Header("Height")]
    [SerializeField] private float[] heightOctaves;
    [SerializeField] private float heightRedistributionFactor;
    [SerializeField] private int maxHeight;
    
    [Header("Moisture")]
    [SerializeField] private float[] moistureOctaves;
    [SerializeField] private float moistureRedistributionFactor;
    [SerializeField] private int maxMoisture;

    [Header("Strangeness")]
    [SerializeField] private float[] strangenessOctaves;
    [SerializeField] private float strangenessRedistributionFactor;
    [SerializeField] private int maxStrangeness;
    
    [Header("Density")]
    [SerializeField] private float[] densityOctaves;
    [SerializeField] private float densityRedistributionFactor;
    [SerializeField] private int maxDensity;
    
    private float[,] _generatedMap;
    
    private int _size = 513;

    public delegate void GenericDelegate();
    public delegate void GenericDelegate<T>(T variable);

    #region Map Generation

    // Step 1: Generate Heights
    public void GenerateMap(Vector2Int tile, int[] seeds, TerrainData chunkData, GenericDelegate finalCallback)
    {
        _size = chunkData.Size;

        GenerateNoise(seeds[0], tile, heightOctaves, heightRedistributionFactor, maxHeight, () =>
        {
            GenerateMap(tile, seeds, _generatedMap, chunkData, finalCallback);
        });
    }

    // Step 2: Generate Moisture
    private void GenerateMap(Vector2Int tile, int[] seeds, float[,] heightMap, TerrainData chunkData, GenericDelegate finalCallback)
    {
        GenerateNoise(seeds[1], tile, moistureOctaves, moistureRedistributionFactor, maxMoisture, () =>
        {
            GenerateMap(tile, seeds, heightMap, _generatedMap, chunkData, finalCallback);
        });
    }

    // Step 3: Generate Strangeness
    private void GenerateMap(Vector2Int tile, int[] seeds, float[,] heightMap, float[,] moistureMap, TerrainData chunkData, GenericDelegate finalCallback)
    {
        GenerateNoise(seeds[2], tile, strangenessOctaves, strangenessRedistributionFactor, maxStrangeness, () =>
        {
            GenerateMap(tile, seeds, heightMap, moistureMap, _generatedMap, chunkData, finalCallback);
        });
    }
    
    // Step 4: Generate Density
    private void GenerateMap(Vector2Int tile, int[] seeds, float[,] heightMap, float[,] moistureMap, float[,] strangenessMap, TerrainData chunkData, GenericDelegate finalCallback)
    {
        GenerateNoise(seeds[3], tile, densityOctaves, densityRedistributionFactor, maxDensity, () =>
        {
            GenerateMap(heightMap, moistureMap, strangenessMap, _generatedMap, chunkData, finalCallback);
        });
    }
    
    // Step 5: Compile into biome map
    private void GenerateMap(float[,] heightMap, float[,] moistureMap, float[,] strangenessMap, float[,] densityMap, TerrainData chunkData, GenericDelegate finalCallback)
    {
        chunkData.SetMaps(heightMap, moistureMap, strangenessMap, densityMap);
        
        finalCallback();
    }

    #endregion
    
    #region Noise Generation

    private void GenerateNoise(int seed, Vector2Int position, float[] octaves, float redistributionFactor, int maxValue, GenericDelegate onFinishedCallback)
    {
        StartCoroutine(GenerateNoiseCoroutine(seed, position, octaves, redistributionFactor, maxValue, data =>
            {
                _generatedMap = data;
                onFinishedCallback?.Invoke();
            }
        ));
    }

    IEnumerator GenerateNoiseCoroutine(int seed, Vector2Int position, float[] octaves, float redistributionFactor, int maxValue, GenericDelegate<float[,]> callback)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();

        float[,] noise = new float[_size, _size];

        Vector2 seedOffset = new Vector2(seed, seed);
        Vector2 positionOffset = position * (_size - 1);
        Vector2 offset = positionOffset + seedOffset;

        for (int z = 0; z < _size; z++)
        {
            if (timer.ElapsedMilliseconds > 3)
            {
                yield return null;
                timer.Reset();
                timer.Start();
            }

            for (int x = 0; x < _size; x++)
            {
                noise[x, z] = (maxValue * CompileNoise(x, z, offset, octaves, redistributionFactor));
            }
        }
        timer.Stop();
        // Debug.Log("Total time: " + timer.ElapsedMilliseconds);
        
        callback(noise);
        yield return null;
    }
    
    float CompileNoise(int x, int z, Vector2 offset, float[] octaves, float redistributionFactor)
    {
        float value = 0;
        float octaveSum = 0f;
        
        float xNorm = (x + offset.x) / _size;
        float zNorm = (z + offset.y) / _size;
    
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
