using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunshotEffect : PooledEffect
{
    //[SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private AudioSource _gunshotSound;

    public override void PlayEffects()
    {
        //_muzzleFlash.Play();
        _gunshotSound.Play();
    }
}
