using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class NoiseLayer
    {
        [SerializeField] float octave;
        [SerializeField] int contrast;
        [SerializeField] float midPoint;

        float octaveScaled;
        float inverseOctave;
        float inverseOctaveNormalized;
        float oneMinusMidPoint;

        public float Octave { get { return octave; } }
        public float OctaveScaled { get { return octaveScaled; } }
        public float InverseOctave { get { return inverseOctave; } }
        public float InverseOctaveNormalized { get { return inverseOctaveNormalized; } }
        public float MidPoint { get { return midPoint; } }
        public float OneMinusMidPoint { get { return oneMinusMidPoint; } }
        public int Contrast { get { return contrast; } }

        public NoiseLayer(float octave, int contrast, float midPoint)
        {
            this.octave = octave;
            this.contrast = contrast;
            this.midPoint = midPoint;
        }

        public void CalculateValues(int scaleDown, float lowestOctave)
        {
            octaveScaled = octave / scaleDown;
            inverseOctave = 1 / octave;
            inverseOctaveNormalized = 1 / (octave / lowestOctave);
            oneMinusMidPoint = 1 - midPoint;
        }
    }

    [Header("Global")]
    [SerializeField] int globalScaleDown = 512;

    [Header("Height")]
    [SerializeField] NoiseLayer[] heightLayers = { new NoiseLayer(1, 3, 0.5f), new NoiseLayer(4, 3, 0.5f), new NoiseLayer(8, 3, 0.5f) };
    [SerializeField] int maxHeight = 100;
    
    [Header("Moisture")]
    [SerializeField] NoiseLayer[] moistureLayers = { new NoiseLayer(1, 3, 0.5f), new NoiseLayer(4, 3, 0.5f), new NoiseLayer(8, 3, 0.5f) };
    [SerializeField] int maxMoisture = 100;

    [Header("Strangeness")]
    [SerializeField] NoiseLayer[] strangenessLayers = { new NoiseLayer(1, 3, 0.5f), new NoiseLayer(4, 3, 0.5f), new NoiseLayer(8, 3, 0.5f) };
    [SerializeField] int maxStrangeness = 100;
    
    [Header("Density")]
    [SerializeField] NoiseLayer[] densityLayers = { new NoiseLayer(1, 3, 0.5f), new NoiseLayer(4, 3, 0.5f), new NoiseLayer(8, 3, 0.5f) };
    [SerializeField] int maxDensity = 100;

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

        for (int i = 0; i < heightLayers.Length; i++)
            heightLayers[i].CalculateValues(globalScaleDown, heightLayers[0].Octave);
        
        for (int i = 0; i < moistureLayers.Length; i++)
            moistureLayers[i].CalculateValues(globalScaleDown, moistureLayers[0].Octave);

        for (int i = 0; i < strangenessLayers.Length; i++)
            strangenessLayers[i].CalculateValues(globalScaleDown, strangenessLayers[0].Octave);

        for (int i = 0; i < densityLayers.Length; i++)
            densityLayers[i].CalculateValues(globalScaleDown, densityLayers[0].Octave);

        float[,] heightMap = new float[size, size];
        float[,] moistureMap = new float[size, size];
        float[,] strangenessMap = new float[size, size];
        float[,] densityMap = new float[size, size];

        yield return GenerateGenraricMapRoutine(seeds[0], heightLayers, maxHeight, size, data => { heightMap = data; });

        yield return GenerateGenraricMapRoutine(seeds[1], moistureLayers, maxMoisture, size, data => { moistureMap = data; });

        yield return GenerateGenraricMapRoutine(seeds[2], strangenessLayers, maxStrangeness, size, data => { strangenessMap = data; });

        yield return GenerateGenraricMapRoutine(seeds[3], densityLayers, maxDensity, size, data => { densityMap = data; });

        terrainData.SetMaps(heightMap, moistureMap, strangenessMap, densityMap);

        finalCallback();

        yield return null;
    }

    IEnumerator GenerateGenraricMapRoutine(int seed, NoiseLayer[] noiseLayers, int maxValue, int size, GenericDelegate<float[,]> callback)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();

        float[,] map = new float[size, size];

        float inverseOctaveSum = 0f;
        for (int i = 0; i < noiseLayers.Length; i++)
        {
            inverseOctaveSum += noiseLayers[i].InverseOctave;
        }

        for (int z = 0; z < size; z++)
        {
            if (timer.ElapsedMilliseconds > milisecondsTillYield)
            {
                yield return null;
                timer.Reset();
                timer.Start();
            }

            float zNorm = z + seed;
            for (int x = 0; x < size; x++)
            {
                float xNorm = x + seed;
                map[x, z] = CompileNoise(xNorm, zNorm, noiseLayers, inverseOctaveSum) * maxValue;
            }
        }
        
        timer.Stop();

        callback(map);
        yield return null;
    }

    float CompileNoise(float xNorm, float zNorm, NoiseLayer[] noiseLayers, float inverseOctaveSum)
    {
        float value = 0.5f;

        for (int i = 0; i < noiseLayers.Length; i++)
        {
            float midPoint = noiseLayers[i].MidPoint;
            float oneMinusMidPoint = noiseLayers[i].OneMinusMidPoint;

            float rawValue =  CalculateNoise(xNorm, zNorm, noiseLayers[i].OctaveScaled);

            rawValue = (rawValue - midPoint) / oneMinusMidPoint;

            float powValue = rawValue;
            if (rawValue < 0)
                rawValue = -rawValue;

            for (int j = 1; j < noiseLayers[i].Contrast; j++)
                powValue *= rawValue;

            powValue = powValue * noiseLayers[i].InverseOctaveNormalized * oneMinusMidPoint + midPoint; 

            value *= (powValue * 2);         
        }

        //value *= inverseOctaveSum; 

        return value;

        /* Old / Classic method
        float value = 0;

        for (int i = 0; i < noiseLayers.Length; i++)
        {
            float midPoint = noiseLayers[i].MidPoint;
            float oneMinusMidPoint = noiseLayers[i].OneMinusMidPoint;

            float rawValue = CalculateNoise(xNorm, zNorm, noiseLayers[i].OctaveScaled);

            rawValue = (rawValue - midPoint) / oneMinusMidPoint;

            float powValue = rawValue;
            if (rawValue < 0)
                rawValue = -rawValue;

            for (int j = 1; j < noiseLayers[i].Contrast; j++)
                powValue *= rawValue;

            powValue = noiseLayers[i].InverseOctave * (powValue * oneMinusMidPoint + midPoint);

            value += powValue; 
        }

        value /= inverseOctaveSum;
        */
    }


    float CalculateNoise(float x, float z, float scale)
    {
        x *= scale;
        z *= scale;

        return Mathf.PerlinNoise(x, z);
    }

    #endregion
}
