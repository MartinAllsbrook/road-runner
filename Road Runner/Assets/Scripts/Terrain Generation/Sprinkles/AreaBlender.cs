using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AreaBlender : MonoBehaviour
{
    [SerializeField] private AnimationCurve authorityCurve;

    public void PlaceAndBlend(ref TerrainData terrainData)
    {
        List<PlacedSprinkle> sprinkleMap = SprinkleGenerator.Instance.GetSprinkleMap();

        foreach (PlacedSprinkle placedSprinkle in sprinkleMap)
            CircleBlend(ref terrainData, placedSprinkle.position, placedSprinkle.height, placedSprinkle.sprinkle.FlatRadius, placedSprinkle.sprinkle.BlendRadius);
        
        terrainData.GeneratePlanes(); // TODO: Not this
    }

    private void CircleBlend(ref TerrainData terrainData, Vector2Int blendCenter, float height, int flatRadius, int blendRadius)
    {
        int terrainSize = terrainData.Size;

        for (int x = 0; x < terrainSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                Vector2Int point = new Vector2Int(x, z);

                float authority = GetTerraformAuthority(point, blendCenter, flatRadius, blendRadius); // 1 - auth because we want this to be inverted

                float terrainHeight = terrainData.GetHeight(x, z);
                float newHeight = Mathf.Lerp(terrainHeight, height, authority);

                terrainData.SetHeight(x, z, newHeight);

                if (authority >= 1)
                {
                    terrainData.SetDensity(x, z, 0);
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
