using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class TerrainManager : NetworkBehaviour
{
    public static TerrainManager Instance; // Singleton

    [Header("Refenences")]
    [SerializeField] private NavMeshManager navMeshManager;
    [SerializeField] private TreeManager treeManager;
    [SerializeField] private SprinkleGenerator sprinkleGenerator;
    [Tooltip("The chunk that will be instantiated to form the terrain")] [SerializeField] private GameObject terrainChunk;

    [Header("Terrain Generation")]
    [Tooltip("TrueTerrainSize = (terrainRadius x 2 + 1) * chunksize")] [SerializeField] private int terrainRadius;
    [Tooltip("The size of an individual chunk")] [SerializeField] private int chunkSize;
    [Tooltip("A list of biome to be generated on the terrain")] [SerializeField] private Biome[] biomes;
    public Biome[] Biomes // TODO: Should this just be a getter?
    {
        get { return biomes; }
        private set { }
    }

    [Header("Loading")]
    [SerializeField] private float loadingPauseTime = 0.3f;

    private int _terrainSize; // Size

    //private GameObject[,] _activeChunks; 
    private Dictionary<Vector2Int, MeshTerrainChunk> _loadedChunks;

    private int masterSeed; // All seeds and stuff
    private int[] _perlinNoiseSeeds;
    private int _sprinkleSeed;
    private int _treeSeed;
    private int[,] poiSeeds;

    private UnityEvent _onMapsGenerated; // This is an event that is called when all the maps have been generated
    private int _numMapsGenerated = 0;

    private UnityEvent _onChunkLoaded; // This is an event that is called when all the chunks have been loaded
    private int _numLoadedChunks = 0;
    private int _chunksToLoad;

    static private NetworkVariable<int> _worldSeed; // This is the seed that is sent to the server, stored accross the network

    #region Loading Screen Debugging Stuff
    private Stopwatch timer = new Stopwatch(); // Stopwatch for testing and debugging
    private float _totalTimeElapsed = 0f;
    
    private void CompleteSection(string sectionName)
    {
        float elapsedMS = timer.ElapsedMilliseconds;
        _totalTimeElapsed += elapsedMS;
        Debug.Log("[Terrain Generation Sequence] " + sectionName + " completed in " + elapsedMS.ToString() + "ms, Total time elapsed: " + _totalTimeElapsed.ToString() + "ms");
        timer.Restart();
    }

    private WaitForSeconds SequencePause()
    {
        Debug.Log("[Terrain Waiting] About to Wait for " + loadingPauseTime.ToString() + "s");
        return new WaitForSeconds(loadingPauseTime);
    }
    #endregion
    
    private void Start()
    {
        _worldSeed = new NetworkVariable<int>();
        NetworkManager.Singleton.OnClientConnectedCallback += TryGenerateTerrain;
    }

    /// <summary>
    /// Sort of a replacement for Start() for the Server's TerrainManager, called by the RelayUI when a new server is created.
    /// Only used by the server.
    /// </summary>
    /// <param name="seed">The world seed</param>
    public void Set(int seed) 
    {
        _worldSeed.Value = seed;
        TryGenerateTerrain(NetworkManager.Singleton.LocalClientId);
    }

    /// <summary>
    /// Attempts to generate the terrain, making sure not to if anything will cause an error.
    /// Generates the terrain for the local client and the server.
    /// </summary>
    /// <param name="clientId">The clients ID</param>
    private void TryGenerateTerrain(ulong clientId)
    {
        timer.Start();
        _totalTimeElapsed = 0;

        Debug.Log("[Terrain] TryGenerateTerrain attempt made");
        // TryGenerateTerrain is called every time a client connects to the server,
        // so we need this check to make sure that the client that just connected is the local client,
        // because if the localClient just connected they probably need a terrain to play on.
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        int seed = _worldSeed.Value; // Get the seed from the network variable
        if (seed == 0)
        {
            // TODO: Maybe check if the server host entered 0 as the seed and if so, generate a random seed / do something about it?
            //Debug.LogError("[Terrain] Seed is 0, this should never happen! If you set the seed to 0 please don't, because right now my program is asuming shit is broken because of you");
            return;
        }
        Debug.Log("[Terrain] Seed: " + seed);

        if (Instance == null)
            Instance = this;       
        else
        {
            Debug.LogError("[Terrain] There are multiple Terrain Managers in the scene!");
            return;
        }

        CompleteSection("Pregeneration Checks"); // Reported 2ms

        GenerateTerrain(seed); // This is where the fun really starts, all the basic checks have passed and we are ready to generate the terrain
    }

    /// <summary>
    /// Starts a predictable terrain generation process.
    /// </summary>
    /// <param name="seed">The terrain's seed</param>
    private void GenerateTerrain(int seed)
    {
        masterSeed = seed;

        _loadedChunks = new Dictionary<Vector2Int, MeshTerrainChunk>();

        treeManager.Initialize(biomes);

        _terrainSize = terrainRadius * 2 + 1;
        _chunksToLoad = _terrainSize * _terrainSize;

        _onMapsGenerated = new UnityEvent();
        _onMapsGenerated.AddListener(OnMapGenerated);
        _onChunkLoaded = new UnityEvent();
        _onChunkLoaded.AddListener(OnChunkLoaded);

        CompleteSection("Variable Initialization"); // Reported ~57ms -> supprized this is the longest part but 57ms is like nothing so it's fine

        GenerateSeeds(); 

        CompleteSection("Seed Generation"); // Reported ~0ms

        sprinkleGenerator.GenerateSprinkles(chunkSize - 1, terrainRadius, _sprinkleSeed); 

        CompleteSection("Sprinkle position generation"); // Reported ~0ms

        StartCoroutine(GenerateMaps());
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

    #region SECTION: Generate Maps
    /// <summary>
    /// Tells all the chunks to generate their noise maps (height, moisture, strangeness, etc.)
    /// This could be turned into a coroutine to make the loading screen more accurate. TODO: Think about it
    /// </summary>
    private IEnumerator GenerateMaps()
    {
        UIManager.Instance.SetLoadingScreenText(UIManager.LoadingScreenTexts.GeneratingTerrainMaps);
        yield return SequencePause();
        timer.Restart();

        int chunkWidth = chunkSize - 1;

        for (int x = 0; x < _terrainSize; x++)
        {
            for (var z = 0; z < _terrainSize; z++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, z);

                GameObject newChunk = Instantiate(terrainChunk, new Vector3(chunkPosition.x * (chunkWidth), 0, chunkPosition.y * (chunkWidth)), Quaternion.identity, transform);
                MeshTerrainChunk chunk = newChunk.GetComponent<MeshTerrainChunk>();

                chunk.CreateMaps(_perlinNoiseSeeds, _onMapsGenerated, chunkSize, terrainRadius); // This calls coroutines under the hood

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
        {
            StartCoroutine(WhenMapsGenerated());
            CompleteSection("Noise-Map Generation"); // Reported ~2,000ms
        }
    }

    #endregion

    /// <summary>
    /// Once the maps have been generated, this function tells the chunks to place sprinkles, blend them, and draw chunk meshes.
    /// </summary>
    private IEnumerator WhenMapsGenerated()
    {
        UIManager.Instance.SetLoadingScreenText(UIManager.LoadingScreenTexts.PlacingLandmarks);
        yield return SequencePause();
        timer.Restart();

        sprinkleGenerator.FindHeightsAndPlace(_loadedChunks); 

        CompleteSection("Landmark / Sprinkle Placement"); // Reported ~9ms -> Great

        UIManager.Instance.SetLoadingScreenText(UIManager.LoadingScreenTexts.DrawingTerrain);
        yield return SequencePause();
        timer.Restart();

        for (int x = 0; x < _terrainSize; x++)
        {
            for (var z = 0; z < _terrainSize; z++)
            {
                MeshTerrainChunk chunk = _loadedChunks[new Vector2Int(x,z)];

                chunk.DecorateAndDraw(_onChunkLoaded);
            }
        }

        CompleteSection("Decorating and drawing"); // Reported ~10,000ms
    }

    /// <summary>
    /// Similar to OnMapGenerated(), this function tallies the number of chunks that have been loaded, and when all the chunks have been loaded, continues
    /// </summary>
    private void OnChunkLoaded()
    {
        _numLoadedChunks++;
        if (_numLoadedChunks >= _chunksToLoad)
            StartCoroutine(FinalLoadingRoutine());
    }

    /// <summary>
    /// Coroutine that places trees, waits, and bakes the navmesh.
    /// </summary>
    private IEnumerator FinalLoadingRoutine()
    {
        UIManager.Instance.SetLoadingScreenText(UIManager.LoadingScreenTexts.ScatteringTrees);
        yield return SequencePause(); 
        timer.Restart();

        for (int i = 0; i < _terrainSize; i++)
        {
            for (int j = 0; j < _terrainSize; j++)
            {
                _loadedChunks[new Vector2Int(i, j)].GetComponent<TreeScatter>().PlaceTrees(chunkSize, _treeSeed);
            }
        }

        CompleteSection("Tree Placement"); // Reported ~28ms -> WOWOWOW That's the power of object pooling

        UIManager.Instance.SetLoadingScreenText(UIManager.LoadingScreenTexts.GeneratingNavMesh);
        yield return SequencePause(); 
        timer.Restart();

        navMeshManager.BakeNavMesh();

        CompleteSection("NavMesh Baking"); // Reported 8,000ms -> I Don't think we can do much about this, I wonder how long it will take on the 5x5 map
        yield return null;

        PlayerSpawner.localPlayerSpawner.EnterLimbo();
        yield return null;
    }


}
