using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine
{
    private int maxAmmoCount;
    private int ammoCount;

    public int Count
    {
        get { return ammoCount; }
    }

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

    public bool TryAddRound()
    {
        if (ammoCount < maxAmmoCount)
        {
            ammoCount++;
            return true;
        }
        return false;
    }

    public int TryAddCount(int count)
    {
        int added = 0;
        for (int i = 0; i < count; i++)
        {
            if (TryAddRound())
                added++;
            else
                break;
        }
        return added;
    }

    public void Reload()
    {
        Debug.LogWarning("The reloading method is being depreciated");
        ammoCount = maxAmmoCount;
    }
}
