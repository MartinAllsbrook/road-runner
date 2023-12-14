using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : CustomEffect
{
    [SerializeField] private ParticleSystem ParticleSystem;
    [SerializeField] private AudioSource AudioSource;

    public override void PlayEffects()
    {
        ParticleSystem.Play();
        AudioSource.Play();
    }
}
