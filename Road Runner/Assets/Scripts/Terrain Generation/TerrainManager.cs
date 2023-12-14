using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class TerrainManager : NetworkBehaviour
{
    public static TerrainManager Instance;

    [Header("Refenences")]
    [SerializeField] private NavMeshManager navMeshManager;

    [Header("Terrain Generation")]
    [SerializeField] private GameObject terrainChunk;
    [SerializeField] private int terrainRadius;
    [SerializeField] private TreeManager treeManager;
    private SprinkleGenerator _sprinkleGenerator;
    [SerializeField] private int chunkSize;

    // Biomes
    [SerializeField] private Biome[] biomes;
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

    // UI 
    //private GameObject _serverUI;

    private readonly Quaternion _zeroRotation = new Quaternion(0, 0, 0, 0);

    static private NetworkVariable<int> _worldSeed; // This is the seed that is sent to the server, stored accross the network

    private void Start()
    {
        _worldSeed = new NetworkVariable<int>();
        NetworkManager.Singleton.OnClientConnectedCallback += TryGenerateTerrain;
    }

    /// <summary>
    /// Sort of a replacement for Start() for the TerrainManager, called by the RelayUI when a new server is created.
    /// </summary>
    /// <param name="seed">The world seed</param>
    public void Set(int seed) 
    {
        _worldSeed.Value = seed;
        TryGenerateTerrain(NetworkManager.Singleton.LocalClientId);
    }

    /// <summary>
    /// Attempts to generate the terrain, making sure not to if anything will cause an error.
    /// Also only generates the terrain for the local client.
    /// </summary>
    /// <param name="clientId">The clients ID</param>
    private void TryGenerateTerrain(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        int seed = _worldSeed.Value; // Get the seed from the network variable
        if (seed == 0)
            return;
        Debug.Log("Seed: " + seed);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are multiple Terrain Managers in the scene!");
            return;
        }

        GenerateTerrain(seed); // This is where the fun really starts, all the basic checks have passed and we are ready to generate the terrain
    }

    /// <summary>
    /// Start
    /// Starts a predictable terrain generation process.
    /// </summary>
    /// <param name="seed">The terrain's seed</param>
    private void GenerateTerrain(int seed)
    {
        UIManager.Instance.StartTerrainLoading();
        
        treeManager.Initialize(biomes); 

        masterSeed = seed;

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

    /// <summary>
    /// Generates predictable random numbers for the terrain generation so that all terrains match.
    /// </summary>
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

    /// <summary>
    /// Tells all the chunks to generate their noise maps (height, moisture, strangeness, etc.)
    /// </summary>
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

    /// <summary>
    /// Tallys the number of maps that have been generated, and when all the maps have been generated, calls WhenMapsGenerated()
    /// </summary>
    private void OnMapGenerated()
    {
        _numMapsGenerated++;
        if (_numMapsGenerated >= _chunksToLoad)
            WhenMapsGenerated();
    }

    /// <summary>
    /// Once the maps have been generated, this function tells the chunks to place sprinkles, blend them, and draw chunk meshes.
    /// </summary>
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

    /// <summary>
    /// Similar to OnMapGenerated(), this function tallies the number of chunks that have been loaded, and when all the chunks have been loaded, calls DoneLoading()
    /// </summary>
    private void OnChunkLoaded()
    {
        _numLoadedChunks++;
        if (_numLoadedChunks >= _chunksToLoad)
            DoneLoading();
    }

    /// <summary>
    /// Tells the TreeScatter to place trees.
    /// Finishes Process.
    /// </summary>
    private void DoneLoading()
    {
        for(int i = 0; i < _terrainSize; i++)
        {
            for (int j = 0; j < _terrainSize; j++)
            {
                _loadedChunks[new Vector2Int(i, j)].GetComponent<TreeScatter>().PlaceTrees(chunkSize, _treeSeed);
            }
        }

        OnTerrainFinished();
    }

    private void OnTerrainFinished()
    {
        // Once finished
        Debug.Log("Done Loading Chunks");

        // Generate NavMesh
        UIManager.Instance.StartGeneratingNavMesh();
        navMeshManager.BakeNavMesh();

        PlayerSpawner.localPlayerSpawner.EnterLimbo();
    }
}
