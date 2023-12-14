using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathEffect : CustomEffect
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private float destroyDelay = 1f;

    public override void PlayEffects()
    {
        audioSource.Play();
        deathParticleSystem.Play();

        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
