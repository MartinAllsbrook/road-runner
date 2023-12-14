using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHolePool : EffectPool
{
    public static BulletHolePool Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Debug.LogError("Depreciated please delete me");
    }
}
