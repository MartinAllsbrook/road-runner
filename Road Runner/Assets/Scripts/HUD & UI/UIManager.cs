using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// The UI Manager tracks, eneables and disables the games UI elements.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Sigleton instance

    [Header("HUD Elements")]
    [SerializeField] private GameObject debugHUD;
    [SerializeField] private GameObject basicHUD;

    [Header("UI Elements")]
    [SerializeField] private GameObject titleScreenUI; // This is only seen once when the game starts
    [SerializeField] private GameObject serverUI; // This is only seen once when the game starts
    [SerializeField] private GameObject limboUI; // This is seen when the player dies, and is waiting to respawn
    [SerializeField] private GameObject pauseUI; // This is seen when the player pauses the game
    [SerializeField] private GameObject inventoryUI; // This is seen when the player opens their inventory
    [SerializeField] private GameObject mapUI; // This is seen when the player opens their map

    [Header("Loading Screens")]
    [SerializeField] private GameObject loadingScreen; // This is seen when the game is loading
    [SerializeField] private TextMeshProUGUI loadingScreenText; // This is the text that is displayed on the loading screen
    [SerializeField] private Slider loadingBar ; // This is the loading bar that is displayed on the loading screen

    private string[] _loadingScreenTexts =
    {
        "Connecting to server",
        "Joining Server",
        "Generating Terrain Maps", 
        "Placing Landmarks", 
        "Drawing Terrain",
        "Scattering Trees",
        "Generating NavMesh",
    };

    public enum LoadingScreenTexts
    {
        ConnectingToServer,
        JoiningServer,
        GeneratingTerrainMaps,
        PlacingLandmarks,
        DrawingTerrain,
        ScatteringTrees,
        GeneratingNavMesh,
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are multiple UI Managers in the scene!");
            Destroy(this);
        }

        // Setting up loading screen
        loadingBar.value = 0;
        loadingBar.maxValue = _loadingScreenTexts.Length;
    }

    private void Start()
    {
        titleScreenUI.SetActive(true); // Make sure the title screen is active when the game starts
    }

    public void ContinueFromTitleScreen()
    {
        DisableAll();
        serverUI.SetActive(true);
    }

    #region Loading screens

    public void StartLoadingScreen()
    {
        serverUI.SetActive(false);
        loadingScreen.SetActive(true);
    }

    public void SetLoadingScreenText(LoadingScreenTexts text)
    {
        Debug.Log("[Loading Screen] Setting loading screen text to: " + _loadingScreenTexts[(int)text]);
        loadingScreenText.text = _loadingScreenTexts[(int)text];
        loadingBar.value = (int)text;
    }

/*    public void StartJoiningServer()
    {
        serverUI.SetActive(false);
        joiningServerScreen.SetActive(true);
    }

    public void StartCreatingServer()
    {
        serverUI.SetActive(false);
        creatingServerScreen.SetActive(true);
    }

    /// <summary>
    /// Shows the terrain loading screen and disables the server UI
    /// </summary>
    public void StartTerrainLoading()
    {
        joiningServerScreen.SetActive(false);
        creatingServerScreen.SetActive(false);
        terrainLoadingScreen.SetActive(true);
    }

    public void StartGeneratingNavMesh()
    {
        terrainLoadingScreen.SetActive(false);
        generatingNavMeshScreen.SetActive(true);
    }*/

    #endregion

    public void EnterLimbo()
    {
        DisableAll();
        limboUI.SetActive(true);
    }

    public void ExitLimbo()
    {
        DisableAll();
        basicHUD.SetActive(true);
    }

    /// <summary>
    /// Disables all UI elements
    /// </summary>
    private void DisableAll()
    {
        // Loading Screens
        loadingScreen.SetActive(false);

        // UIs
        titleScreenUI.SetActive(false);
        serverUI.SetActive(false);
        limboUI.SetActive(false);
        pauseUI.SetActive(false);
        inventoryUI.SetActive(false);
        mapUI.SetActive(false);
        
        // HUDs
        basicHUD.SetActive(false);
        debugHUD.SetActive(false);
    }

    // Basic UI Element Toggling for QC and Debugging
    #region UI Element Toggling
    [Command("ToggleDebugHUD")]
    public void ToggleDebugHUD()
    {
        debugHUD.SetActive(!debugHUD.activeSelf);
    }

    [Command("ToggleBasicHUD")]
    public void ToggleBasicHUDDebug()
    {
        basicHUD.SetActive(!basicHUD.activeSelf);
    }

    [Command("ToggleServerUI")]
    public void ToggleServerUIDebug()
    {
        serverUI.SetActive(!serverUI.activeSelf);
    }

    [Command("ToggleLimboUI")]
    public void ToggleLimboUIDebug()
    {
        limboUI.SetActive(!limboUI.activeSelf);
    }

    [Command("TogglePauseUI")]
    public void TogglePauseUIDebug()
    {
        pauseUI.SetActive(!pauseUI.activeSelf);
    }

    [Command("ToggleInventoryUI")]
    public void ToggleInventoryUIDebug()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    [Command("ToggleMapUI")]
    public void ToggleMapUIDebug()
    {
        mapUI.SetActive(!mapUI.activeSelf);
    }

    #endregion
}
