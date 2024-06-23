using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RiverCreator : SplineMeshCreator
{
    [SerializeField] float riverStartCentrality = 2f;
    [SerializeField] int searchRadius = 25;
    [SerializeField] int numSamples = 10;

    [SerializeField] float riverWidth = 1f;
    [SerializeField] int riverMeshResolution = 10;

    CatmullRomSpline river;
    Random random;

    [SerializeField] RiverTester riverTester;
    
    public void CreateRandomRiverTest(TerrainData terrainData, int riverSeed)
    {
        random = new Random(riverSeed);

        int terrainSize = terrainData.Size;
        int radius = terrainSize / 2;
        int innerRadius = terrainData.InnerRadius;
        int riverStartRadius = Mathf.FloorToInt(innerRadius / riverStartCentrality);

        int offset = radius - riverStartRadius;
        Vector2Int startingPoint = new Vector2Int(offset + random.Next(innerRadius), offset + random.Next(innerRadius));

        List<Vector2> points = CreateRiver(startingPoint, terrainData);

        Vector2[] pointsArray = points.ToArray();
/*        foreach (Vector2 point in pointsArray)
        {
            riverTester.DrawPoint(point);
        }*/
        if (pointsArray.Length < 4)
        {
            Debug.LogWarning("River has less than 4 points.");
            return;
        }
        river = new CatmullRomSpline(pointsArray);
        
        //riverTester.DrawRiver(river);
        CreateMesh(river, riverMeshResolution, riverWidth, terrainData);
    }



    private List<Vector2> CreateRiver(Vector2Int startingPoint, TerrainData terrainData)
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(startingPoint);

        Vector2Int currentPoint = startingPoint;
        Vector2Int nextPoint = GetNextRiverPoint(currentPoint, terrainData);

        while (currentPoint != nextPoint)
        {
            points.Add(nextPoint);
            currentPoint = nextPoint;
            nextPoint = GetNextRiverPoint(currentPoint, terrainData);
        }

        return points;
    }

    private Vector2Int GetNextRiverPoint(Vector2Int currentPoint, TerrainData terrainData)
    {
        Vector2Int nextPoint = currentPoint;
        float bestScore = 0;

        int min = terrainData.OuterRadius - terrainData.InnerRadius;
        int max = terrainData.OuterRadius + terrainData.InnerRadius;

        for (int i = 0; i < numSamples; i++)
        {
            Vector2 sample = GetRandomPointOnUnitCircle() * searchRadius;
            Vector2Int sampleInt = new Vector2Int(Mathf.RoundToInt(sample.x), Mathf.RoundToInt(sample.y));
            Vector2Int candidate = currentPoint + sampleInt;

            if (candidate.x < min || candidate.x >= max || candidate.y < min || candidate.y >= max)
                continue;

            float distance = sample.magnitude;
            float currentHeight = terrainData.GetHeight(currentPoint.x, currentPoint.y);
            float score = GetRiverScore(candidate, distance, currentHeight, terrainData);

            Debug.Log("Score: " + score + ", BestScore: " + bestScore);

            if (score > bestScore)
            {
                bestScore = score;
                nextPoint = candidate;
            }
        }

        return nextPoint;
    }

    private float GetRiverScore(Vector2Int point, float distance, float currentHeight, TerrainData terrainData)
    {
        float score = 0;

        float nextHeight = terrainData.GetHeight(point);
        float deltaHeight = nextHeight - currentHeight;

        score = -(deltaHeight / distance);

        return score;
    }

    private Vector2 GetRandomPointOnUnitCircle()
    {
        float angle = (float) random.NextDouble() * 360f;
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        return new Vector2(x, y);
    }
}
