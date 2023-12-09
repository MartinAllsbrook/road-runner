using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Display References")]
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private Animator reloadAnimator;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private InventoryDisplay inventoryDisplay;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject mapCamera;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image foodBar;
    [SerializeField] private Image waterBar;

    [Header("Audio")]
    [SerializeField] private AudioSource hitMarkerAudio;

    [Header("Inputs")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private KeyCode inventoryKey = KeyCode.E;
    [SerializeField] private KeyCode mapKey = KeyCode.M;

    private bool escMenuOpen = true;
    private bool inventoryOpen = false;
    private bool mapOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
            ToggleEscMenu();
        
        if (Input.GetKeyDown(inventoryKey))
            ToggleInvetory();

        if (Input.GetKeyDown(mapKey))
            ToggleMap();
    }

    public void ToggleEscMenu()
    {
        if (!escMenuOpen)
        {
            escMenuOpen = true;
            pauseMenu.SetActive(true);
            PlayerSpawner.localPlayerSpawner.Pause();
        }
        else
        {
            escMenuOpen = false;
            pauseMenu.SetActive(false);
            PlayerSpawner.localPlayerSpawner.Unpause();
        }
    }

    private void ToggleInvetory()
    {
        if (!inventoryOpen)
        {
            inventoryOpen = true;
            inventoryDisplay.gameObject.SetActive(true);
            PlayerSpawner.localPlayerSpawner.Pause();
        }
        else
        {
            inventoryOpen = false;
            inventoryDisplay.gameObject.SetActive(false);
            PlayerSpawner.localPlayerSpawner.Unpause();
        }
    }

    private void ToggleMap()
    {
        if (!mapOpen)
        {
            mapOpen = true;
            mapCamera.SetActive(true);
            map.SetActive(true);
            PlayerSpawner.localPlayerSpawner.Pause();
        }
        else
        {
            mapOpen = false;
            mapCamera.SetActive(false);
            map.SetActive(false);
            PlayerSpawner.localPlayerSpawner.Unpause();
        }
    }

    public void SetAmmoCountDisplay(int ammoCount, int maxAmmoCount)
    {
        string ammoCountString = ammoCount.ToString();
        string maxAmmoString = maxAmmoCount.ToString();

        ammoCountText.text = ammoCountString + " / " + maxAmmoString;
    }

    public void PlayReloadUIAnimation(float duration)
    {
        reloadAnimator.gameObject.SetActive(true);
        reloadAnimator.SetFloat("Speed", 1/duration);
        reloadAnimator.Play("Reload");
    }

    public void StopReloadUIAnimation()
    {
        reloadAnimator.gameObject.SetActive(false);
    }
    
    public void PlayHitMarker()
    {
        StartCoroutine(PlayHitmarkerCoroutine(0.1f, Color.green));
    }

    private IEnumerator PlayHitmarkerCoroutine(float duration, Color color)
    {
        hitMarkerAudio.Play();
        hitMarker.SetActive(true);
        hitMarker.GetComponent<Image>().color = color;
        
        yield return  new WaitForSeconds(duration);
        
        hitMarker.SetActive(false);
        
    }

    public void UpdateHealthBar(float healthValue)
    {
        healthBar.rectTransform.sizeDelta = new Vector2(healthValue * 5, 30);
    }

    public void UpdateFoodBar(float foodValue)
    {
        foodBar.rectTransform.sizeDelta = new Vector2(foodValue * 5, 30);
    }

    public void UpdateWaterBar(float waterValue)
    {
        waterBar.rectTransform.sizeDelta = new Vector2(waterValue * 5, 30);
    }
}
