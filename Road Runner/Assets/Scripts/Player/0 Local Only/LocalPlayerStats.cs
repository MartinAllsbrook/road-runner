using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class LocalPlayerStats : MonoBehaviour, IPersistantData
{
    public static LocalPlayerStats Instance;

    [Header("Stat Decay Rates")]
    [SerializeField] private float foodDecayRate = 0.25f;
    [SerializeField] private float waterDecayRate = 0.25f;
    [SerializeField] private float healthDecayRate = 0.5f;

    private HUDController hudController;

    private float health = 100f;
    private float food = 100f;
    private float water = 100f;

    private bool _inLimbo = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("More than one LocalPlayerStats instance exists!");
    }

    private void Start()
    {
        hudController = HUDController.Instance;
    }

    private void Update()
    {
        if (_inLimbo)
            return;

        UpdateFoodAndWater();
    }

    private void UpdateFoodAndWater()
    {
        if (_inLimbo)
            return;

        ChangeFood(-Time.deltaTime * foodDecayRate);
        ChangeWater(-Time.deltaTime * waterDecayRate);

        if (water <= 0)
        {
            ChangeHealth(-Time.deltaTime * healthDecayRate);
        }

        if (food <= 0) 
        { 
            ChangeHealth(-Time.deltaTime * healthDecayRate);
        }
    }

    #region IPersistantData Methods

    public void LoadData(CharacterData allCharacterData)
    {
        health = allCharacterData.Health;
        food = allCharacterData.Food;
        water = allCharacterData.Water;
    }

    public void SaveData(ref CharacterData allCharacterData)
    {
        allCharacterData.Health = health;
        allCharacterData.Food = food;
        allCharacterData.Water = water;
    }

    #endregion

    #region Change Stats Methods
    public void ChangeFood(float value)
    {
        if (_inLimbo)
            return;

        float newfoodValue = food + value;
        food = Mathf.Clamp(newfoodValue, 0f, 100f);
        hudController.UpdateFoodBar(food);
    }

    public void ChangeWater(float value)
    {
        if (_inLimbo)
            return;

        float newWaterValue = water + value;
        water = Mathf.Clamp(newWaterValue, 0f, 100f);
        hudController.UpdateWaterBar(water);
    }

    public void ChangeHealth(float value)
    {
        if (_inLimbo)
            return;

        float newHealthValue = health + value;
        health = Mathf.Clamp(newHealthValue, -1f, 100f);
        hudController.UpdateHealthBar(health);

        if (health <= 0)
            Die();
    }
    #endregion

    private void Die()
    {
        Debug.Log(health);
        Inventory.Instance.DropAllItems();

        _inLimbo = true;
        CharacterPersistanceManager.Instance.DeleteCharacter();
        Player.LocalInstance.EnterLimbo();
    }

    /// <summary>
    /// Resets the player's stats and exits stat limbo, allowing for stat upadates again
    /// </summary>
    public void Spawn()
    {
        hudController.UpdateHealthBar(health);
        hudController.UpdateFoodBar(food);
        hudController.UpdateWaterBar(water);

        _inLimbo = false;
    }

    #region Commands

    [Command]
    public void TakeDamage(float damage)
    {
        ChangeHealth(-damage);
        Player.LocalInstance.GetComponent<PlayerFXController>().PlayHitWithBulletFX();
    }

    #endregion

}
