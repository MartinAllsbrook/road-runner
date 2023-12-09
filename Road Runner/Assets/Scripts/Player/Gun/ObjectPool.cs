using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    protected GameObject[] pooledObjects;
    [SerializeField] protected GameObject objectToPool;
    [SerializeField] protected int poolSize;

    private void Start()
    {
        pooledObjects = new GameObject[poolSize];
        GameObject tmp;
        
        for (int i = 0; i < poolSize; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects[i] = tmp;
        }
    }
    
    protected GameObject GetPooledObject()
    {
        for(int i = 0; i < poolSize; i++)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
