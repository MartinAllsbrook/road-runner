using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTester : MonoBehaviour
{
    [SerializeField] Terrain terrain;

    [SerializeField] private int seed = 0; // The seed for the terrain generation
    [SerializeField] private bool randomSeed = true; // If true, the seed will be random

    private void Start()
    {
        if (randomSeed)
        {
            seed = Random.Range(0, 999999);
        }
        terrain.GenerateTerrain(seed, () => { }, true);
    }
}
