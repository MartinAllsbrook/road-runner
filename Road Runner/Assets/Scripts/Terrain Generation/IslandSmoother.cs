using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IslandSmoother : MonoBehaviour
{
    [SerializeField] private AnimationCurve terraformAuthorityCuve;
    [SerializeField] private int transitionWidth;

    public void SmoothHeights(MeshTerrainChunk meshTerrainChunk, int terrainRadius)
    {
        meshTerrainChunk.GetChunkDataRef(out ChunkData chunkData);

        int chunkSize = chunkData.Size;
        Vector2Int chunkPosition = chunkData.ChunkPosition;
        Vector2Int worldPosition = chunkData.WorldPosition;

        int worldRadius = (chunkSize - 1) * terrainRadius + ((chunkSize - 1) / 2);
        int innerWorldRadius = worldRadius - transitionWidth;
        Vector2Int worldCenter = new Vector2Int(worldRadius, worldRadius);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                Vector2Int point = worldPosition + new Vector2Int(x, z);
                Vector2Int centerToPoint = point - worldCenter;

                float auth = 1 - GetTerraformAuthorityAtPoint(centerToPoint, innerWorldRadius, worldRadius, terraformAuthorityCuve); // 1 - auth because we want this to be inverted

                float terrainHeight = chunkData.GetHeight(x, z);
                float newHeight = Mathf.Lerp(terrainHeight, 0, auth);

                chunkData.SetHeight(x, z, newHeight);

                if (newHeight <= 2)
                {
                    chunkData.SetDensity(x, z, 0);
                }
            }
        }
    }

    private float GetTerraformAuthorityAtPoint(Vector2 centerToPoint, int innerRadius, int outerRadius, AnimationCurve authorityCuve)
    {
        float distance = Vector2.Distance(Vector2.zero, centerToPoint);
        if (distance <= innerRadius)
            return 1f;

        if (distance >= outerRadius)
            return 0f;

        float t = (distance - innerRadius) / (outerRadius - innerRadius);
        return authorityCuve.Evaluate(t);
    }
}
