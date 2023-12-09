using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHolePool : ObjectPool
{
    public static BulletHolePool Instance;

    private int _currentIndex;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlaceBulletHole(Vector3 position, Quaternion rotation)
    {
        if (_currentIndex >= poolSize)
            _currentIndex = 0;
        
        GameObject bulletHole = pooledObjects[_currentIndex];
        bulletHole.transform.position = position;
        bulletHole.transform.rotation = rotation;
        
        bulletHole.SetActive(true);
        
        bulletHole.GetComponent<ParticleSystem>().Play();
        
        _currentIndex++;
    }
}
