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

public class Terrain : MonoBehaviour
{
    public static Terrain Instance; // Singleton

    [Header("Refenences")]
    [SerializeField] private TreeManager treeManager;
    
    [SerializeField] private SprinkleGenerator sprinkleGenerator;
    
    [Tooltip("The chunk that will be instantiated to form the terrain")]
    [SerializeField] private GameObject terrainChunk;

    [Header("Terrain Generation")]
    [Tooltip("TrueTerrainSize = (terrainRadius x 2 + 1) * chunksize")]
    [Range(0, 2)][SerializeField] private int terrainRadius;
    
    [Tooltip("The size of an individual chunk")]
    [SerializeField] private int chunkSize;
    
    [Tooltip("A list of biome to be generated on the terrain")]
    [SerializeField] private Biome[] biomes;
    
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

    // All seeds and stuff
    private int masterSeed;
    private int[] _perlinNoiseSeeds;
    private int _sprinkleSeed;
    private int _treeSeed;
    private int[,] poiSeeds;

    private UnityEvent _onMapsGenerated; // This is an event that is called when all the maps have been generated
    private int _numMapsGenerated = 0;

    private UnityEvent _onChunkLoaded; // This is an event that is called when all the chunks have been loaded
    private int _numLoadedChunks = 0;
    private int _chunksToLoad;

    public delegate void GenericDelegate();
    private GenericDelegate _finalCallback;

    #region Loading Screen & Debugging Stuff
    private Stopwatch timer = new Stopwatch(); // Stopwatch for testing and debugging
    private float _totalTimeElapsed = 0f;

    private bool _testingMode = false;
    public bool Testing
    {
        get { return _testingMode; }
        private set { }
    }

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
        Instance = this; // TODO: Make Better
    }

    /// <summary>
    /// Starts a predictable terrain generation process.
    /// </summary>
    /// <param name="seed">The terrain's seed</param>
    public void GenerateTerrain(int seed, GenericDelegate finalCallback, bool testing)
    {
        masterSeed = seed;
        _finalCallback = finalCallback;
        _testingMode = testing;

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
                poiSeeds[x, z] = Random.Range(0, 10000);
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
        if (!_testingMode)
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
            CompleteSection("Noise-Map Generation"); // Reported 2,000ms - 3x3 | 5,500ms - 5x5 -> Eh it's coroutine-ified
        }
    }

    #endregion

    /// <summary>
    /// Once the maps have been generated, this function tells the chunks to place sprinkles, blend them, and draw chunk meshes.
    /// </summary>
    private IEnumerator WhenMapsGenerated()
    {
        if (!_testingMode)
            UIManager.Instance.SetLoadingScreenText(UIManager.LoadingScreenTexts.PlacingLandmarks);
    
        yield return SequencePause();
        timer.Restart();

        sprinkleGenerator.FindHeightsAndPlace(_loadedChunks);

        CompleteSection("Landmark / Sprinkle Placement"); // Reported 9ms - 3x3 | 9ms - 5x5 -> Great

        if (!_testingMode)
            UIManager.Instance.SetLoadingScreenText(UIManager.LoadingScreenTexts.DrawingTerrain);
    
        yield return SequencePause();
        timer.Restart();

        for (int x = 0; x < _terrainSize; x++)
        {
            for (var z = 0; z < _terrainSize; z++)
            {
                MeshTerrainChunk chunk = _loadedChunks[new Vector2Int(x, z)];

                chunk.DecorateAndDraw(_onChunkLoaded);
                yield return null;
            }
        }
    }

    /// <summary>
    /// Similar to OnMapGenerated(), this function tallies the number of chunks that have been loaded, and when all the chunks have been loaded, continues
    /// </summary>
    private void OnChunkLoaded() // TODO: This is a bit of a misnomer, it's not really loaded, it's decorated and drawn, ALSO this is not needed since DecorateAndDraw is not async
    {
        _numLoadedChunks++;
        if (_numLoadedChunks >= _chunksToLoad)
        {
            CompleteSection("Decorating and drawing"); // Reported 10,000ms - 3x3 | 26,000ms - 5x5 -> This is the big one, but we can improve it
            StartCoroutine(FinalLoadingRoutine());
        }
    }

    /// <summary>
    /// Coroutine that places trees, waits, and bakes the navmesh.
    /// </summary>
    private IEnumerator FinalLoadingRoutine()
    {
        if (!_testingMode)
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

        CompleteSection("Tree Placement"); // Reported 28ms - 3x3 | 103ms - 5x5 -> WOWOWOW That's the power of object pooling

        if (!_testingMode)
            UIManager.Instance.SetLoadingScreenText(UIManager.LoadingScreenTexts.GeneratingNavMesh);
    
        yield return SequencePause();
        timer.Restart();

        _finalCallback?.Invoke();
        yield return null;
    }
}
