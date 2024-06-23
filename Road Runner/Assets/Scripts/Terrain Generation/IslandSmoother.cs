using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IslandSmoother : MonoBehaviour
{
    [SerializeField] private AnimationCurve terraformAuthorityCuve;
    [SerializeField] private int transitionWidth;
    [SerializeField] private int seaLevel;

    public void SmoothHeights(ref TerrainData terrainData)
    {
        int terrainSize = terrainData.Size;
        int outterRadius = terrainSize / 2;
        int innerRadius = outterRadius - transitionWidth;
        terrainData.OuterRadius = outterRadius;
        terrainData.InnerRadius = innerRadius;

        Vector2Int center = new Vector2Int(outterRadius, outterRadius);

        for (int x = 0; x < terrainSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                Vector2Int point = new Vector2Int(x, z);
                Vector2Int centerToPoint = point - center;

                float auth = 1 - GetTerraformAuthorityAtPoint(centerToPoint, innerRadius, outterRadius, terraformAuthorityCuve); // 1 - auth because we want this to be inverted

                float terrainHeight = terrainData.GetHeight(x, z);
                float newHeight = Mathf.Lerp(terrainHeight, 0, auth);

                terrainData.SetHeight(x, z, newHeight);

                if (newHeight <= seaLevel)
                {
                    terrainData.SetDensity(x, z, 0);
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
