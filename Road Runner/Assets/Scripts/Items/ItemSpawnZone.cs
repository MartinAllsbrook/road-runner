using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawnZone : MonoBehaviour
{
    [SerializeField] private Inventory.InventoryItem[] possibleItems;
    [SerializeField] private float itemSpawnTimer;

    private BoxCollider boxCollider;
    private Bounds spawnBounds;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator SpawnItemRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(itemSpawnTimer);
            SpawnRandomItem();
        }
    }

    private void SpawnRandomItem()
    {
        Vector3 itemPosition = GetRandomPointInBounds();
        Inventory.InventoryItem item = possibleItems[Random.Range(0, possibleItems.Length)];    
        Debug.Log("Spawning " + item + " at " + itemPosition);

        ItemSpawner.Instance.SpawnItem(itemPosition, item);
    }

    private Vector3 GetRandomPointInBounds()
    {
        spawnBounds = boxCollider.bounds;

        float x = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
        float y = Random.Range(spawnBounds.min.y, spawnBounds.max.y);
        float z = Random.Range(spawnBounds.min.z, spawnBounds.max.z);

        return new Vector3(x, y, z);
    }
}
