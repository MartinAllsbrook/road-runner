using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine
{
    private int maxAmmoCount;
    private int ammoCount;

    public Magazine(int size)
    {
        maxAmmoCount = size;
        ammoCount = size;
    }

    public int ConsumeRound()
    {
        int count = ammoCount;
        if (ammoCount > 0)
        {
            ammoCount--;
        }
        return count;
    }

    public void Reload()
    {
        ammoCount = maxAmmoCount;
    }
}
