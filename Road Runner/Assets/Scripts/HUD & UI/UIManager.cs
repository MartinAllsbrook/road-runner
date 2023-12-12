using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;

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

    [Header("Info Screens")]
    [SerializeField] private GameObject terrainLoadingScreen;

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

    /// <summary>
    /// Shows the terrain loading screen and disables the server UI
    /// </summary>
    public void StartTerrainLoading()
    {
        serverUI.SetActive(false);
        terrainLoadingScreen.SetActive(true);
    }

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
        titleScreenUI.SetActive(false);
        debugHUD.SetActive(false);
        basicHUD.SetActive(false);
        serverUI.SetActive(false);
        limboUI.SetActive(false);
        pauseUI.SetActive(false);
        inventoryUI.SetActive(false);
        mapUI.SetActive(false);
        terrainLoadingScreen.SetActive(false);
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

    [Command("ToggleTerrainLoadingScreen")]
    public void ToggleTerrainLoadingScreenDebug()
    {
        terrainLoadingScreen.SetActive(!terrainLoadingScreen.activeSelf);
    }

    #endregion
}
