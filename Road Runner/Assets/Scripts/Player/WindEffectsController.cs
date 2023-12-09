using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffectsController : MonoBehaviour
{
    [SerializeField] private ParticleSystem windFXParticleSystem;

    private PlayerStats _playerStats;
    private bool inShelteredArea;

    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.transform.CompareTag("Sheltered Area") && !inShelteredArea)
        {
            EnterShelteredArea();
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.transform.CompareTag("Sheltered Area") && inShelteredArea)
        {
            ExitShelteredArea();
        }
    }

    private void EnterShelteredArea()
    {
        inShelteredArea = true;

        windFXParticleSystem.Stop();

        StopAllCoroutines();
    }

    private IEnumerator DoWeatherDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _playerStats.ChangeHealth(-0.25f);
        }
    }

    private void ExitShelteredArea()
    {
        inShelteredArea = false;
        
        windFXParticleSystem.Play();

        StartCoroutine(DoWeatherDamage());
    }
}
