using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AreaBlender : MonoBehaviour
{
    [SerializeField] private AnimationCurve authorityCurve;

    public void PlaceAndBlend(ref ChunkData chunkData)
    {
        List<PlacedSprinkle> sprinkleMap = SprinkleGenerator.Instance.GetSprinkleMap();

        foreach (PlacedSprinkle placedSprinkle in sprinkleMap)
            CircleBlend(ref chunkData, placedSprinkle.position, placedSprinkle.height, placedSprinkle.sprinkle.FlatRadius, placedSprinkle.sprinkle.BlendRadius);
        
        chunkData.GeneratePlanes(); // TODO: Not this
    }

    private void CircleBlend(ref ChunkData chunkData, Vector2Int blendCenter, float height, int flatRadius, int blendRadius)
    {
        Vector2Int worldPosition = chunkData.WorldPosition;
        int chunkSize = chunkData.Size;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                Vector2Int point = worldPosition + new Vector2Int(x, z);

                float authority = GetTerraformAuthority(point, blendCenter, flatRadius, blendRadius); // 1 - auth because we want this to be inverted

                float terrainHeight = chunkData.GetHeight(x, z);
                float newHeight = Mathf.Lerp(terrainHeight, height, authority);

                chunkData.SetHeight(x, z, newHeight);

                if (authority >= 1)
                {
                    chunkData.SetDensity(x, z, 0);
                }
            }
        }
    }

    private float GetTerraformAuthority(Vector2Int point, Vector2Int blendCenter, int flatRadius, int blendRadius)
    {
        Vector2Int centerToPoint = point - blendCenter;

        float distance = Vector2.Distance(Vector2.zero, centerToPoint);
        if (distance <= flatRadius)
            return 1f;

        if (distance >= blendRadius)
            return 0f;

        float t = (distance - flatRadius) / (blendRadius - flatRadius);
        return authorityCurve.Evaluate(t);
    }
}
