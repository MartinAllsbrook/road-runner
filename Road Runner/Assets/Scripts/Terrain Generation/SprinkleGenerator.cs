using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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

    private Vector2Int GetUniquePosition(int newSprinkleRadius, int worldSize)
    {
        int placeableAreaSize = worldSize - (2 * newSprinkleRadius);

        int x = newSprinkleRadius + _random.Next(placeableAreaSize); // TODO: Make this use Random.InsideUnitCircle
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

    public List<PlacedSprinkle> GetSprinkleMap()
    {
        return _sprinkleMap;
    }

    public void FindSprinkleHeights(Dictionary<Vector2Int, MeshTerrainChunk> loadedChunks)
    {
        foreach(KeyValuePair<Vector2Int, MeshTerrainChunk> keyValuePair in loadedChunks)
        {
            MeshTerrainChunk chunk = keyValuePair.Value;
            chunk.GetChunkDataRef(out ChunkData chunkData);

            foreach (PlacedSprinkle placedSprinkle in _sprinkleMap)
            {
                if (chunkData.ContainsPoint(placedSprinkle.position))
                {
                    float height = chunkData.GetHeight(placedSprinkle.position.x - chunkData.WorldPosition.x, placedSprinkle.position.y - chunkData.WorldPosition.y);

                    placedSprinkle.height = height;

                    Vector3 sprinkleWorldPosition = new Vector3(placedSprinkle.position.x, height, placedSprinkle.position.y);
                    float roatation = (float)_random.NextDouble() * 360f;
                    Quaternion sprinkleRotation = Quaternion.Euler(0, roatation, 0);

                    Instantiate(placedSprinkle.sprinkle, sprinkleWorldPosition, sprinkleRotation);
                }
            }
        }
    }

    public Vector3 GetSpawnPoint()
    {
        int sprinkleIndex = UnityEngine.Random.Range(0, sprinkleRanks[0].numToPlace);
        PlacedSprinkle placedSprinkle = _sprinkleMap[sprinkleIndex];

        Vector2 position = UnityEngine.Random.insideUnitCircle * placedSprinkle.sprinkle.FlatRadius + placedSprinkle.position;
        return new Vector3(position.x, 100, position.y);
    }
}
