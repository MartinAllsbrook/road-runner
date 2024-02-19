using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [Header("HUDs")]
    [SerializeField] private ItemInspectorHUD itemInspectorHUD;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private Animator reloadAnimator;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private GameObject pauseMenu;
    //[SerializeField] private InventoryDisplay inventoryDisplay;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject mapCamera;
    [SerializeField] private CrosshaireController crosshaireController;

    [Header("Stat Displays")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image foodBar;
    [SerializeField] private Image waterBar;

    [Header("Audio")]
    [SerializeField] private AudioSource hitMarkerAudio;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    #region Item Inspector HUD
    public void StartInspectItem(UseableItem item)
    {
        itemInspectorHUD.gameObject.SetActive(true);
        itemInspectorHUD.StartInspectItem(item);
    }

    public void StopInspectItem()
    {
        itemInspectorHUD.StopInspectItem();
        itemInspectorHUD.gameObject.SetActive(false);
    }
    #endregion

    #region Gun HUD Stuff
    public void SetAmmoCountDisplay(int ammoCount, int maxAmmoCount)
    {
        string ammoCountString = ammoCount.ToString();
        string maxAmmoString = maxAmmoCount.ToString();

        ammoCountText.text = ammoCountString + " / " + maxAmmoString;
    }

    public void SetCrosshaireInaccuracy(float inaccurcy)
    {
        crosshaireController.SetCrosshaireInaccuracy(inaccurcy);
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
    #endregion

    #region Hitmarker
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
    #endregion

    #region Update Stat Displays
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
    #endregion
}
