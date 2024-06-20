using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PointOfInterestGenerator : MonoBehaviour
{
    [SerializeField] private int innerRadius = 10;
    [SerializeField] private int outerRadius = 20;

    [SerializeField] private int numPOIs;

    [SerializeField] private GameObject[] pointOfInterestOptions;
    [SerializeField] private AnimationCurve terraformAuthority;

    private System.Random random;

    LinkedList<Vector2Int> poiPositions;
    public void TryPlacePOIs(int seed, ref ChunkData chunkData)
    {
        random = new System.Random(seed);

        poiPositions = new LinkedList<Vector2Int>();

        int size = chunkData.Size;
        int positionStart = outerRadius;
        int positionRange = size - outerRadius * 2;

        for (int i = 0; i < numPOIs; i++)
        {
            poiPositions.AddLast(GetUniquePosition(positionStart, positionRange));
        }

        foreach (Vector2Int position in poiPositions)
        {
            PlaceLandMark(ref chunkData, position.x, position.y);
        }
    }

    private Vector2Int GetUniquePosition(int positionStart, int positionRange)
    {
        int x = positionStart + random.Next(positionRange);
        int z = positionStart + random.Next(positionRange);

        Vector2Int potentialPosition = new Vector2Int(x, z);

        foreach (Vector2Int position in poiPositions)
        {
            float distance = (potentialPosition - position).magnitude;

            if (distance < outerRadius)
                return GetUniquePosition(positionStart, positionRange);
        }

        return potentialPosition;
    }

    public void PlaceLandMark(ref ChunkData chunkData, int xPosition, int zPosition)
    {    
        GameObject poiGameObject = pointOfInterestOptions[random.Next(pointOfInterestOptions.Length)];

        //float slope = chunkData.GetSlope(xPosition, zPosition);
        float height = chunkData.GetHeight(xPosition, zPosition);
        
        if (height <= 2)
        {
            return;
        }

        /*
         * if (slope > 25)
            return;
        
        if (height < 4)
            return;
        */    

        for (int x = -outerRadius; x <= outerRadius; x++)
        {
            for (int z = -outerRadius; z <= outerRadius; z++)
            {
                float percent = DistanceBetweenCircles(innerRadius, outerRadius, new Vector2(x, z));

                float newHeight = Mathf.Lerp(chunkData.GetHeight(xPosition + x, zPosition + z), height, percent);

                chunkData.SetHeight(xPosition + x, zPosition + z, newHeight);
                if (percent >= 0.99)
                    chunkData.SetDensity(xPosition + x, zPosition + z, 0);
            }
        }
        
        chunkData.GeneratePlanes();
        GameObject newLandMark = Instantiate(poiGameObject, new Vector3(xPosition + transform.position.x, height, zPosition + transform.position.z), new Quaternion(0, 0, 0, 0), transform);
        newLandMark.transform.Rotate(0f, (float) random.NextDouble() * 360f, 0f);
    }
    
    private float DistanceBetweenCircles(int innerRadius, int outerRadius, Vector2 testPoint)
    {
        float distance = Vector2.Distance(Vector2.zero, testPoint);
        if (distance <= innerRadius)
            return 1f;
        
        if (distance >= outerRadius)
            return 0f;
        
        float t = (distance - innerRadius) / (outerRadius - innerRadius);
        return terraformAuthority.Evaluate(t);
    }
}
