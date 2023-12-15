using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackFX : CustomEffect
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem deathParticleSystem;

    public override void PlayEffects()
    {
        audioSource.Play();
        deathParticleSystem.Play();
    }
}
