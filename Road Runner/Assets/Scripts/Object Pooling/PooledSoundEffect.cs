using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledSoundEffect : PooledEffect
{
    [SerializeField] private AudioSource _sound;

    public override void PlayEffects()
    {
        _sound.Play();
    }
}
