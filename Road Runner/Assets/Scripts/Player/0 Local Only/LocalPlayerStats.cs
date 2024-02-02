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

    // Clothing / resistance
    private float[] resistances = new float[5] {0,0,0,0,0};

    public enum BodyArea
    {
        Global,
        Head,
        Torso,
        Arms,
        Legs
    }

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

    #region Clothing / Resistance Methods
    [Command]
    public void ChangeResistance(BodyArea bodyArea, int deltaResistance)
    {
        int bodyAreaIndex = (int)bodyArea;

        resistances[bodyAreaIndex] += deltaResistance;

        Debug.Log("Resistance changed to " + resistances[bodyAreaIndex] + " for " + bodyArea);
    }

    [Command]
    public void DealDamage(BodyArea bodyArea, float damage, bool playEffect)
    {
        int bodyAreaIndex = (int)bodyArea;

        float rawResistancePercent = resistances[bodyAreaIndex] / 100; // Can be over 100% resistance
        float finalResistancePercent = 1/(-1 - rawResistancePercent) + 1; // Approach 1 as resistance approaches infinity

        float damageMultiplier = 1 - finalResistancePercent;

        if (playEffect)
            Player.LocalInstance.GetComponent<PlayerFXController>().PlayHitWithBulletFX();

        ChangeHealth(-damage * damageMultiplier);
    }
    #endregion

    [Command]
    public void AddHealth(float value)
    {
        ChangeHealth(value);
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

    private void ChangeHealth(float value)
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
}
