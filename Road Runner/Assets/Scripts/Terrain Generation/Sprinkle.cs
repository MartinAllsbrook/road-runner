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

    [SerializeField] private int[] maxSpawnCounts = new int[3];

    private int[] numSpawnedCounters = new int[3];

    private enum SpawnType
    {
        Item = 0,
        EnemyNPC = 1,
        Vehicle = 2
    }

    private void Start()
    {
        TerrainManager.onTerrainGenerated.AddListener(() =>
        {
            AfterTerrainLoadedStart();
        });
    }

    public void DecrementCounter(int spawnedObjectType)
    {
        numSpawnedCounters[spawnedObjectType]--;
    }

    #region Start and Initialization
    private void AfterTerrainLoadedStart()
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

        InitializeSpawnZones();

        if (spawnItems)
            StartCoroutine(SpawnRoutine(itemSpawnCheckInterval, itemSpawnZones, SpawnType.Item));
        if (spawnEnemies)
            StartCoroutine(SpawnRoutine(enemyNPCSpawnCheckInterval, enemyNPCSpawnZones, SpawnType.EnemyNPC));
        if (spawnVehicles)
            StartCoroutine(SpawnRoutine(vehicleSpawnCheckInterval, vehicleSpawnZones, SpawnType.Vehicle));
    }

    private void InitializeSpawnZones()
    {
        InitializeSpawnZoneSet(itemSpawnZones, SpawnType.Item);
        InitializeSpawnZoneSet(enemyNPCSpawnZones, SpawnType.EnemyNPC);
        InitializeSpawnZoneSet(vehicleSpawnZones, SpawnType.Vehicle);
    }

    private void InitializeSpawnZoneSet(SpawnZone[] spawnZones, SpawnType spawnType)
    {
        foreach (var spawnZone in spawnZones)
        {
            spawnZone.Initialize(this, (int)spawnType);
        }
    }
    #endregion

    #region Item Spawning
    private IEnumerator SpawnRoutine(float spawnInterval, SpawnZone[] spawnZones, SpawnType objectType)
    {
        while (true)
        { 
            yield return new WaitForSeconds(spawnInterval);

            bool objectSpawned = false;

            if (numSpawnedCounters[(int)objectType] < maxSpawnCounts[(int)objectType])
                objectSpawned = TrySpawnObject(spawnZones);

            if (objectSpawned)
                numSpawnedCounters[(int)objectType]++;
        }
    }

    private bool TrySpawnObject(SpawnZone[] spawnZones)
    {
        // Find all spawn zones that are not full
        List<int> avialableSpawnZones = new List<int>();
        for (int i = 0; i < spawnZones.Length; i++)
        {
            if (!spawnZones[i].IsFull())
                avialableSpawnZones.Add(i);
        }

        // If there are no spawn zones that are not full, return false
        if (avialableSpawnZones.Count <= 0)
            return false;

        // Pick a random spawn zone from the list and spawn an object in it
        int randomIndex = avialableSpawnZones[UnityEngine.Random.Range(0, avialableSpawnZones.Count)];
        spawnZones[randomIndex].SpawnRandomObject(transform.position, flatRadius);
        return true;
    }
    #endregion

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
