using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LandmarkGenerator : MonoBehaviour
{
    [Serializable]
    public class Landmark
    {
        public Vector2Int chunkPosition;
        public GameObject prefab;
        public int height;
        public int innerRadius;
        public int outerRadius;
    }

    [SerializeField] private AnimationCurve terraformAuthorityCuve;
    [SerializeField] private Landmark[] landmarks;

    private void Awake()
    {
        
    }

    public bool PlaceLandMark(ChunkData chunkData)
    {
        bool hasLandamrk = false;

        for (int i = 0; i < landmarks.Length; i++)
        {
            Vector2Int chunkPosition = chunkData.ChunkPosition;
            int chunkWidth = chunkData.Width;
            
            Vector2Int chunkWorldPosition = chunkPosition * chunkWidth;
            Vector2Int landmarkWorldPosition = (landmarks[i].chunkPosition * chunkWidth) + new Vector2Int(chunkWidth / 2, chunkWidth / 2);

            if (chunkPosition == landmarks[i].chunkPosition)
            {
                Vector3 position = new Vector3(landmarkWorldPosition.x, landmarks[i].height, landmarkWorldPosition.y);
                Instantiate(landmarks[i].prefab, position, new Quaternion(0, 0, 0, 0), transform);

                hasLandamrk = true;
            }

            // TODO: make code more efficent by only calling BlendTerrain() on the chnks that need it

            Vector2Int baseChunkOffset = chunkWorldPosition - landmarkWorldPosition;
            BlendTerrain(baseChunkOffset, landmarks[i].height, terraformAuthorityCuve, landmarks[i].innerRadius, landmarks[i].outerRadius);
        }

        return hasLandamrk;
    }

    public void BlendTerrain(Vector2Int baseOffset, int baseHeight, AnimationCurve terraformAuthorityCuve, int innerRadius, int outerRadius)
    {
        MeshTerrainChunk meshTerrainChunk = GetComponent<MeshTerrainChunk>();
        meshTerrainChunk.GetChunkDataRef(out ChunkData chunkData);

        for (int x = 0; x < chunkData.Size; x++)
        {
            for (int z = 0; z < chunkData.Size; z++)
            {
                Vector2 pointOffset = new Vector2(x, z);
                Vector2 landmarkToPoint = baseOffset + pointOffset;

                float authorityPercent = GetTerraformAuthorityAtPoint(landmarkToPoint, innerRadius, outerRadius, terraformAuthorityCuve);

                float terrainHeight = chunkData.GetHeight(x, z);
                float newHeight = Mathf.Lerp(terrainHeight, baseHeight, authorityPercent);

                chunkData.SetHeight(x, z, newHeight);

                if (authorityPercent >= 0.99)
                    chunkData.SetDensity(x, z, 0);
            }
        }
        chunkData.GeneratePlanes(); // TODO: Not this
    }
    
    private float GetTerraformAuthorityAtPoint(Vector2 landmarkToPoint, int flatRadius, int blendRadius, AnimationCurve authorityCuve)
    {
        float distance = Vector2.Distance(Vector2.zero, landmarkToPoint);
        if (distance <= flatRadius)
            return 1f;
        
        if (distance >= blendRadius)
            return 0f;
        
        float t = (distance - flatRadius) / (blendRadius - flatRadius);
        return authorityCuve.Evaluate(t);
    }
}
