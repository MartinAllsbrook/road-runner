using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    [SerializeField] private ParticleSystem ParticleSystem;
    [SerializeField] private AudioSource AudioSource;

    public void PlayEffects()
    {
        ParticleSystem.Play();
        AudioSource.Play();
    }
}
