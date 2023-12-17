using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected float maxSpawnedObjects;
    [SerializeField] protected SpawnedObject[] objectsToSpawn;

    protected int numSpawnedObjects;

    protected BoxCollider boxCollider;
    protected Bounds spawnBounds;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public bool IsFull()
    {
        return numSpawnedObjects >= maxSpawnedObjects;
    }

    public void RemoveSpawnedObject()
    {
        numSpawnedObjects--;
    }
    
    public virtual void SpawnRandomObject(Vector3 sprinkleCenter, float sprinkleRadius)
    {
        if (IsFull())
        {
            Debug.LogWarning("Tried to spawn in full spawn zone");
            return;
        }

        numSpawnedObjects++;

        int randomIndex = Random.Range(0, objectsToSpawn.Length);
        Vector3 randomPoint = GetRandomPointInBounds();

        ObjectSpawner.Instance.SpawnObject(objectsToSpawn[randomIndex], randomPoint, this, sprinkleCenter, sprinkleRadius);
    }

    protected Vector3 GetRandomPointInBounds()
    {
        spawnBounds = boxCollider.bounds;

        float x = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
        float y = Random.Range(spawnBounds.min.y, spawnBounds.max.y);
        float z = Random.Range(spawnBounds.min.z, spawnBounds.max.z);

        return new Vector3(x, y, z);
    }
}
