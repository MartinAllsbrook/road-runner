using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerStats : NetworkBehaviour
{
    public static PlayerStats Instance;
    
    [SerializeField] private int mapSize;
    [SerializeField] private float foodDecayRate;
    [SerializeField] private float waterDecayRate;
    [SerializeField] private float healthDecayRate;

    private HUDController hudController;

    private float health = 100f;
    private float food = 100f;
    private float water = 100f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            Destroy(GetComponent<PlayerInput>());
            return;
        }

        if (Instance == null)
            Instance = this;


        hudController = GameObject.Find("HUD").GetComponent<HUDController>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        UpdateFoodAndWater();
    }

    private void UpdateFoodAndWater()
    {
        if (!IsOwner)
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

    public void PlayHitMarker()
    {
        if (!IsOwner)
            return;
        
        hudController.PlayHitMarker();
    }

    public void ChangeFood(float value)
    {
        if (!IsOwner)
            return;

        float newfoodValue = food + value;
        food = Mathf.Clamp(newfoodValue, 0f, 100f);
        hudController.UpdateFoodBar(food);
    }

    public void ChangeWater(float value)
    {
        if (!IsOwner)
            return;

        float newWaterValue = water + value;
        water = Mathf.Clamp(newWaterValue, 0f, 100f);
        hudController.UpdateWaterBar(water);
    }

    public void ChangeHealth(float value)
    {
        if (!IsOwner)
            return;

        float newHealthValue = health + value;
        health = Mathf.Clamp(newHealthValue, -1f, 100f);
        hudController.UpdateHealthBar(health);

        if (health <= 0)
            Die();
    }
    
    private void Die()
    {
        Debug.Log(health);
        GetComponent<BaseInventory>().DropAllItems();
        Respawn();
    }

    private void Respawn()
    {
        health = 100f;
        hudController.UpdateHealthBar(health);

        food = 100f;
        hudController.UpdateFoodBar(food);

        water = 100f;
        hudController.UpdateWaterBar(water);

        GetComponent<PlayerSpawner>().SpawnPlayer();
    }
}
