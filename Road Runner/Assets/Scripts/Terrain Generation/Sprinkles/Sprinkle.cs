using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkle : MonoBehaviour
{
    [Serializable]
    public class SpawnSettings
    {
        [SerializeField] public SpawnType Type;
        [SerializeField] public bool Spawn = true;
        [SerializeField] public float SpawnCheckInterval = 90f;
        [SerializeField] public SpawnZone[] SpawnZones;
        [SerializeField] public int MaxSpawnCount = 5;
        public int SpawnedCount = 0;
    }

    [Header("Terrain Generation")]
    [SerializeField] int flatRadius = 16;
    [SerializeField] int blendRadius = 32;

    [Header("Spawn Settings")]
    [SerializeField] private SpawnSettings[] spawnSettings = new SpawnSettings[3];

    public enum SpawnType
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

    private void AfterTerrainLoadedStart()
    {
        if (!TerrainManager.Instance.IsServer)
        {
            Debug.LogWarning("Disableing Sprinkle on Client");

            foreach (var spawnSetting in spawnSettings)
            {
                foreach (var spawnZone in spawnSetting.SpawnZones)
                    spawnZone.enabled = false;
            }

            enabled = false;
            return;
        }

        InitializeSpawnZones();
        FillSprinkle();

        foreach (var spawnSetting in spawnSettings)
        {
            if (spawnSetting.Spawn)
                StartCoroutine(SpawnRoutine(spawnSetting.SpawnCheckInterval, spawnSetting.SpawnZones, spawnSetting.Type));
        }
    }

    #region Counter Inc/Dec
    private void IncrementCounter(int spawnedObjectType)
    {
        spawnSettings[spawnedObjectType].SpawnedCount++;
    }

    public void DecrementCounter(int spawnedObjectType)
    {
        spawnSettings[spawnedObjectType].SpawnedCount--;
    }
    #endregion

    #region Initialization
    private void InitializeSpawnZones()
    {
        foreach (var spawnSetting in spawnSettings)
        {
            InitializeSpawnZoneSet(spawnSetting.SpawnZones, spawnSetting.Type);            
        }
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
    private void FillSprinkle()
    {
        foreach (var spawnSetting in spawnSettings)
        {
            if (spawnSetting.Spawn)
            {
                int maxSpawns = spawnSetting.MaxSpawnCount;
                for (int j = 0; j < maxSpawns; j++)
                {
                    TrySpawnObject(spawnSetting.SpawnZones, spawnSetting.Type);
                }
            }
        }
    }

    private IEnumerator SpawnRoutine(float spawnInterval, SpawnZone[] spawnZones, SpawnType objectType)
    {
        while (true)
        { 
            yield return new WaitForSeconds(spawnInterval);

            int numSpawned = spawnSettings[(int)objectType].SpawnedCount;
            int maxSpawned = spawnSettings[(int)objectType].MaxSpawnCount;

            if (numSpawned < maxSpawned)
                TrySpawnObject(spawnZones, objectType);
        }
    }

    private bool TrySpawnObject(SpawnZone[] spawnZones, SpawnType objectType)
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

        IncrementCounter((int)objectType);
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
