using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkle : MonoBehaviour
{
    [Header("Terrain Generation")]
    [SerializeField] int flatRadius = 16;
    [SerializeField] int blendRadius = 32;

    [Header("Item Spawning")]
    [SerializeField] private bool spawnItems = true;
    [SerializeField] private float itemSpawnCheckInterval = 90f;
    [SerializeField] private SpawnZone[] itemSpawnZones;

    [Header("Enemy NPC Spawning")]
    [SerializeField] private bool spawnEnemies = true;
    [SerializeField] private float enemyNPCSpawnCheckInterval = 90f;
    [SerializeField] private SpawnZone[] enemyNPCSpawnZones;

    [Header("Vehicle Spawning")]
    [SerializeField] private bool spawnVehicles = true;
    [SerializeField] private float vehicleSpawnCheckInterval = 180f;
    [SerializeField] private SpawnZone[] vehicleSpawnZones;

    private int numNaturalItems;
    private int numEnemyNPCs;

    private void Start()
    {
        TerrainManager.onTerrainGenerated.AddListener(() =>
        {
            if (!TerrainManager.Instance.IsServer)
            { 
                Debug.LogWarning("Disableing Sprinkle on Client");
                foreach (var itemSpawnZone in itemSpawnZones)
                    itemSpawnZone.enabled = false;
                foreach (var enemyNPCSpawnZone in enemyNPCSpawnZones)
                    enemyNPCSpawnZone.enabled = false;
                foreach (var vehicleSpawnZone in vehicleSpawnZones)
                    vehicleSpawnZone.enabled = false;

                enabled = false;

                return;
            }

            if (spawnItems)
                StartCoroutine(SpawnRoutine(itemSpawnCheckInterval, itemSpawnZones));
            if (spawnEnemies)                
                StartCoroutine(SpawnRoutine(enemyNPCSpawnCheckInterval, enemyNPCSpawnZones));
            if (spawnVehicles)
                StartCoroutine(SpawnRoutine(vehicleSpawnCheckInterval, vehicleSpawnZones));
        });
    }

    private IEnumerator SpawnRoutine(float spawnInterval, SpawnZone[] spawnZones)
    {
        while (true)
        { 
            yield return new WaitForSeconds(spawnInterval);

            TrySpawnItem(spawnZones);
        }
    }

    private void TrySpawnItem(SpawnZone[] spawnZones)
    {
        List<int> avialableIndexes = new List<int>();
        for (int i = 0; i < spawnZones.Length; i++)
        {
            if (!spawnZones[i].IsFull())
                avialableIndexes.Add(i);
        }

        if (avialableIndexes.Count <= 0)
            return;

        int randomIndex = avialableIndexes[UnityEngine.Random.Range(0, avialableIndexes.Count)];
        spawnZones[randomIndex].SpawnRandomObject();
    }

    #region Terrain Generation

    public int FlatRadius
    {
        get { return flatRadius; }
        private set { }
    }

    public int BlendRadius
    {
        get { return blendRadius; }
        private set { }
    }

    #endregion
}
