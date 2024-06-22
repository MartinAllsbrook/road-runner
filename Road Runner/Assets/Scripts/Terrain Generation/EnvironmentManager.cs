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

public class EnvironmentManager : NetworkBehaviour
{
    public static EnvironmentManager Instance; // Singleton
    [SerializeField] private Terrain terrain;

    public static bool terrainGenerated = false; // TODO: Rename to environmentReady
    public static UnityEvent onTerrainGenerated;

    [Header("Refenences")]
    [SerializeField] private NavMeshManager navMeshManager;

    static private NetworkVariable<int> _worldSeed; // This is the seed that is sent to the server, stored accross the network

    #region Loading Screen Debugging Stuff
    [Header("Loading")]
    [SerializeField] private float loadingPauseTime = 0.3f;

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
        onTerrainGenerated = new UnityEvent(); // This needs to hapen as soon as possible so that the things can subscribe to it

        _worldSeed = new NetworkVariable<int>();
        NetworkManager.Singleton.OnClientConnectedCallback += TryGenerateTerrain;
    }

    /// <summary>
    /// Sort of a replacement for Start() for the Server's EnvironmentManager, called by the RelayUI when a new server is created.
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

        terrain.GenerateTerrain(seed, () => { StartCoroutine(OnTerrainGenerated()); }, false); // This is where the fun really starts, all the basic checks have passed and we are ready to generate the terrain
    }

    private IEnumerator OnTerrainGenerated() // TODO: Does this really need to be a coroutine
    {
        if (IsServer)
            navMeshManager.BakeNavMesh();
        else
            Destroy(navMeshManager);

        CompleteSection("NavMesh Baking"); // Reported 8,000ms - 3x3 | 25,000 - 5x5 -> I Don't think we can do much about this
        yield return null;

        timer.Stop(); // TODO: Probaly need to fix this timer shit

        terrainGenerated = true;
        onTerrainGenerated.Invoke();

        Player.LocalInstance.EnterLimbo();
        yield return null;
    }
}
