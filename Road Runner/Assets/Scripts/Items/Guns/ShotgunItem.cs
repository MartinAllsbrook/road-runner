using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunItem : GunItem
{
    [SerializeField] private int numPellets;

    protected override void Fire(float accuracy)
    {
        for (int i = 0; i < numPellets; i++)
        {
            CreateBullet(accuracy);
        }
    }
}
