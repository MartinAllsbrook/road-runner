using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TerrainUtils;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class TerrainManager : NetworkBehaviour
{
    [SerializeField] private GameObject terrainChunk;
    [SerializeField] private int terrainRadius;
    [SerializeField] private TreeManager treeManager;
    private SprinkleGenerator _sprinkleGenerator;
    [SerializeField] private int chunkSize;

    // Biomes
    [SerializeField] private Biome[] biomes;
    public static TerrainManager Instance;
    public Biome[] Biomes
    {
        get { return biomes; }
        private set { }
    }

    // Sizes LMAO
    private int _terrainSize;

    // Tracking chunks
    private GameObject[,] _activeChunks;
    private Dictionary<Vector2Int, MeshTerrainChunk> _loadedChunks;

    // Seeding & Random Numbers
    private int masterSeed;
    private int[] _perlinNoiseSeeds;
    private int _sprinkleSeed;
    private int _treeSeed;
    private int[,] poiSeeds;

    // Tracking chunk loading
    private UnityEvent _onMapsGenerated;
    private int _numMapsGenerated = 0;
    private UnityEvent _onChunkLoaded;
    private int _numLoadedChunks = 0;
    private int _chunksToLoad;
    private GameObject serverUI;
    // UI 
    private GameObject _serverUI;

    private readonly Quaternion _zeroRotation = new Quaternion(0, 0, 0, 0);

    static private NetworkVariable<int> _worldSeed;

    private void Awake()
    {
    }

    private void Start()
    {
        _worldSeed = new NetworkVariable<int>();
        NetworkManager.Singleton.OnClientConnectedCallback += GenerateTerrain;
    }

    public void Set(GameObject serverUI)
    {
        this.serverUI = serverUI;
    }

    public void Set(int seed, GameObject serverUI) 
    {
        _worldSeed.Value = seed;
        this.serverUI = serverUI;
        GenerateTerrain(NetworkManager.Singleton.LocalClientId);
    }

    public void GenerateTerrain(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return; 

        int seed = _worldSeed.Value;
        if (seed == 0)
            return;

        Debug.Log("Seed: " + seed);

        if (Instance == null) 
            Instance = this;

        treeManager.Initialize(biomes);

        masterSeed = seed;
        _serverUI = serverUI;

        _terrainSize = terrainRadius * 2 + 1;
        _chunksToLoad = _terrainSize * _terrainSize;

        _onMapsGenerated = new UnityEvent();
        _onMapsGenerated.AddListener(OnMapGenerated);
        _onChunkLoaded = new UnityEvent();
        _onChunkLoaded.AddListener(OnChunkLoaded);

        _sprinkleGenerator = GetComponent<SprinkleGenerator>();

        GenerateSeeds();

        _sprinkleGenerator.GenerateSprinkles(chunkSize - 1, terrainRadius, _sprinkleSeed);

        _loadedChunks = new Dictionary<Vector2Int, MeshTerrainChunk>();

        InitializeTerrain();
    }

    private void GenerateSeeds()
    {
        // TODO: Switch this to non static System.Random
        Random.InitState(masterSeed); // Order you getting random number matters

        _perlinNoiseSeeds = new int[4]; // These seeds are for the noise maps
        _perlinNoiseSeeds[0] = Random.Range(2000, 10000);
        _perlinNoiseSeeds[1] = Random.Range(2000, 10000);
        _perlinNoiseSeeds[2] = Random.Range(2000, 10000);
        _perlinNoiseSeeds[3] = Random.Range(2000, 10000);

        _sprinkleSeed = Random.Range(0, 10000);
        _treeSeed = Random.Range(0, 10000);

        poiSeeds = new int[_terrainSize, _terrainSize];
        for (int x = 0; x < _terrainSize; x++)
        {
            for (var z = 0; z < _terrainSize; z++)
            {
                poiSeeds[x, z] = Random.Range(0,10000);
            }
        }
    }

    private void InitializeTerrain()
    {
        //_activeChunks = new GameObject[_terrainSize, _terrainSize];
        int chunkWidth = chunkSize - 1;

        for (int x = 0; x < _terrainSize; x++)
        {
            for (var z = 0; z < _terrainSize; z++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, z);

                GameObject newChunk = Instantiate(terrainChunk, new Vector3(chunkPosition.x * (chunkWidth), 0, chunkPosition.y * (chunkWidth)), _zeroRotation, transform);
                MeshTerrainChunk chunk = newChunk.GetComponent<MeshTerrainChunk>();

                chunk.CreateMaps(_perlinNoiseSeeds, _onMapsGenerated, chunkSize, terrainRadius); // Need batter name for this function, Initiialize chunk?

                _loadedChunks.Add(chunkPosition, chunk);
            }
        }
    }

    private void OnMapGenerated()
    {
        _numMapsGenerated++;
        if (_numMapsGenerated >= _chunksToLoad)
            WhenMapsGenerated();
    }

    private void WhenMapsGenerated()
    {
        _sprinkleGenerator.FindSprinkleHeights(_loadedChunks);

        for (int x = 0; x < _terrainSize; x++)
        {
            for (var z = 0; z < _terrainSize; z++)
            {
                MeshTerrainChunk chunk = _loadedChunks[new Vector2Int(x,z)];

                chunk.DecorateAndDraw(_onChunkLoaded);
            }
        }
    }

    private void OnChunkLoaded()
    {
        _numLoadedChunks++;
        if (_numLoadedChunks >= _chunksToLoad)
            DoneLoading();
    }

    private void DoneLoading()
    {
        for(int i = 0; i < _terrainSize; i++)
        {
            for (int j = 0; j < _terrainSize; j++)
            {
                _loadedChunks[new Vector2Int(i, j)].GetComponent<TreeScatter>().PlaceTrees(chunkSize, _treeSeed);
            }
        }

        Debug.Log("Done Loading Chunks");
        _serverUI.SetActive(false);

        PlayerSpawner.localPlayerSpawner.UnfreezePlayer();
    }
}
