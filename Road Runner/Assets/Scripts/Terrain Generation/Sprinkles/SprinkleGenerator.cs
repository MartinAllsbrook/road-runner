using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[Serializable]
public class SprinkleRank
{
    public int numToPlace;
    public Sprinkle[] sprinkles;
}

public class PlacedSprinkle
{
    public Sprinkle sprinkle;
    public Vector2Int position;
    public float height = 100;

    public PlacedSprinkle(Sprinkle sprinkle, Vector2Int position)
    {
        this.sprinkle = sprinkle; 
        this.position = position;
    }
}

public class SprinkleGenerator : MonoBehaviour
{
    public static SprinkleGenerator Instance;

    [SerializeField] private SprinkleRank[] sprinkleRanks;

    private List<PlacedSprinkle> _sprinkleMap;

    private System.Random _random;

    #region Sprinkle Generation / Placement

    public void GenerateSprinkles(int chunkWidth, int terrainRadius, int seed)
    {
        if (Instance == null) 
            Instance = this;

        _random = new System.Random(seed);
        _sprinkleMap = new List<PlacedSprinkle>();

        int worldSize = chunkWidth * (2 * terrainRadius + 1);

        foreach (SprinkleRank rank in sprinkleRanks)
        {
            for(int i = 0; i < rank.numToPlace; i++)
            {
                PlaceRandomSprincle(rank, worldSize);
            }
        }
    }

    private void PlaceRandomSprincle(SprinkleRank rank, int worldSize)
    {
        int randomSprinkleIndex = _random.Next(rank.sprinkles.Length - 1);
        Sprinkle newSprinkle = rank.sprinkles[randomSprinkleIndex];

        int sprinkleRadius = newSprinkle.BlendRadius;

        Vector2Int position = GetUniquePosition(sprinkleRadius, worldSize);

        PlacedSprinkle placedSprinkle = new PlacedSprinkle(newSprinkle, position);
        _sprinkleMap.Add(placedSprinkle);
    }

    // Recursive function to get a unique position for a sprinkle
    private Vector2Int GetUniquePosition(int newSprinkleRadius, int worldSize)
    {
        int placeableAreaSize = worldSize - (2 * newSprinkleRadius);

        if (placeableAreaSize <= 0)
        {
            Debug.LogError("Sprinkle radius is too large for the world size. WorldSize: " + worldSize + " Sprinkle Diameter: " + (2 * newSprinkleRadius));
        }

        int x = newSprinkleRadius + _random.Next(placeableAreaSize);
        int z = newSprinkleRadius + _random.Next(placeableAreaSize);
        Vector2Int potentialPosition = new Vector2Int(x, z);

        int worldRadius = worldSize / 2;
        Vector2Int worldCenter = new Vector2Int(worldRadius, worldRadius); 
        float distanceToCenter = (worldCenter - potentialPosition).magnitude;
        
        if (distanceToCenter > worldRadius)
            return GetUniquePosition(newSprinkleRadius, worldSize);

        foreach (PlacedSprinkle placedSprinkle in _sprinkleMap)
        {
            int placedSprinkleRadius = placedSprinkle.sprinkle.BlendRadius;
            int minDistance = placedSprinkleRadius + newSprinkleRadius;

            float distance = (placedSprinkle.position - potentialPosition).magnitude;

            if (distance < minDistance)
                return GetUniquePosition(newSprinkleRadius, worldSize);
        }

        return potentialPosition;
    }

    #endregion

    public List<PlacedSprinkle> GetSprinkleMap()
    {
        return _sprinkleMap;
    }

    public void FindHeightsAndPlace(TerrainData terrainData)
    {
        foreach (PlacedSprinkle placedSprinkle in _sprinkleMap)
        {

            float height = terrainData.GetHeight(placedSprinkle.position.x, placedSprinkle.position.y);

            placedSprinkle.height = height;

            Vector3 sprinkleWorldPosition = new Vector3(placedSprinkle.position.x, height, placedSprinkle.position.y);
            float roatation = (float)_random.NextDouble() * 360f;
            Quaternion sprinkleRotation = Quaternion.Euler(0, roatation, 0);

            Instantiate(placedSprinkle.sprinkle, sprinkleWorldPosition, sprinkleRotation, transform);
        }
    }

    public Vector3 GetSpawnPoint()
    {
        int sprinkleIndex = UnityEngine.Random.Range(0, sprinkleRanks[0].numToPlace);
        PlacedSprinkle placedSprinkle = _sprinkleMap[sprinkleIndex];

        Vector2 position = UnityEngine.Random.insideUnitCircle * placedSprinkle.sprinkle.FlatRadius + placedSprinkle.position;
        return new Vector3(position.x, 100, position.y);
    }

    public Vector3 GetPointInSprinkleOnNavmesh()
    {
        Vector3 position = GetSpawnPoint();

        if (!Physics.Raycast(position, Vector3.down, out RaycastHit raycastHit, 128))
        {
            Debug.Log("Couldn't find a point to spawn enemy at near: " + position);
            return Vector3.zero;
        }

        if (!NavMesh.SamplePosition(raycastHit.point, out NavMeshHit navmeshHit, 128, 1))
        {
            Debug.Log("Couldn't find a NavMesh point to spawn enemy at near: " + raycastHit.point);
            return Vector3.zero;
        }

        return navmeshHit.position;
    }
}
