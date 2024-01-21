using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerFXController : MonoBehaviour
{
    [Header("Postprocessing References")]
    [SerializeField] private PostProcessVolume overridePostProcessVolume;
    private PostProcessProfile _overridePostProcessProfile;
    private Vignette _vingetteOverride;

    [Header("Damage FX")]
    [SerializeField] private float damageEffectTime = 0.1f;
    [SerializeField] private AudioSource damageAudioSource;

    [Header("Wind FX")]
    [SerializeField] private ParticleSystem windFXParticleSystem;

    private LocalPlayerStats _playerStats;
    private bool inShelteredArea;

    private void Start()
    {
        // Singleton References
        _playerStats = LocalPlayerStats.Instance;

        overridePostProcessVolume.weight = 0f;

        // Postprocess Caching
        _overridePostProcessProfile = overridePostProcessVolume.profile;
        _vingetteOverride = _overridePostProcessProfile.GetSetting<Vignette>();
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

    #region DamageFX
    [Command]
    public void PlayHitWithBulletFX()
    {
        damageAudioSource.Play();
        StartCoroutine(DoHitWithBulletFX());
    }

    private IEnumerator DoHitWithBulletFX()
    {
        _vingetteOverride.intensity.value = 0.5f;
        _vingetteOverride.color.value = Color.red;
        overridePostProcessVolume.weight = 1f;

        yield return new WaitForSeconds(damageEffectTime);

        _vingetteOverride.intensity.value = 0f;
        _vingetteOverride.color.value = Color.black;
        overridePostProcessVolume.weight = 0f;

        yield return null;
    }

    #endregion

    #region WindFX
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
            _playerStats.ChangeHealth(-0.25f); // This should probably somewhere else? and not be a magic number?
        }
    }

    private void ExitShelteredArea()
    {
        inShelteredArea = false;
        
        windFXParticleSystem.Play();

        StartCoroutine(DoWeatherDamage());
    }
    #endregion
}
