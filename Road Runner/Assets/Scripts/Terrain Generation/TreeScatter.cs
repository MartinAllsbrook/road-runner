using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class TreeScatter : MonoBehaviour
{
    [Serializable]
    public class MyTreeGroup
    {
        public GameObject[] trees;
        public float minMoisture;
    }
    
    [Header("Trees")]
    [Range(1, 10)] [SerializeField] private float treeUniformity;
    [SerializeField] private int numTrees;
    [SerializeField] private MyTreeGroup[] treeGroups; // TODO: Delete this
    [SerializeField] private float minHeight;
   
    private System.Random _random;
    
    public delegate void GenericDelegate();

    public void PlaceTrees(int size, int treeSeed)
    {
        // TODO: This does not need to be a reference type
        GetComponent<MeshTerrainChunk>().GetChunkDataRef(out ChunkData chunkData);
        _random = new System.Random(treeSeed);

        Vector3 parentOffset = transform.position;

        float treeSpacing = (float)(size - 1) / numTrees; // Subtract 1 from size because the last row overlaps next chunk, random offset would also set some of these trees over the edge of the chunk
        float maxOffset = treeSpacing / treeUniformity; // This is useful for offsetting the trees

        for (int xI = 0; xI < numTrees; xI++)
        {
            for (int zI = 0; zI < numTrees; zI++)
            {
                float x = xI * treeSpacing + ((float)_random.NextDouble() * maxOffset); // These start at 0 because the loop starts at zero so we are building out in the [+,+] direction
                float z = zI * treeSpacing + ((float)_random.NextDouble() * maxOffset);

                PlaceTree(x, z, parentOffset, chunkData);
            }
        }
    }
  

    private void PlaceTree(float x, float z, Vector3 parentOffset, ChunkData chunkData)
    {
        if (_random.Next(100) >= chunkData.GetDensity(x, z))
            return;
        
        float height = chunkData.GetHeight(x, z);
        int biomeIndex = chunkData.GetBiome((int) x, (int) z);

        float roatation = (float) _random.NextDouble() * 360f;
        Quaternion rotation = Quaternion.Euler(0, roatation, 0);
        Vector3 position = new Vector3(x, height, z) + parentOffset;

        TreeManager.Instance.PlaceTree(position, rotation, biomeIndex);
    }
}
