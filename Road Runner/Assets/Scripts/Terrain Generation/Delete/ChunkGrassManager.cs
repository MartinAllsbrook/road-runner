using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class ChunkGrassManager : MonoBehaviour
{
    [SerializeField] private float minGrassHeight;
    [SerializeField] private GameObject[] grass;
    [SerializeField] private float grassVariation;
    
    /*private void Start()
    {
        grass = new GameObject[64 * 64];
        for (int x = 0; x < 64; x++)
        {
            for (int z = 0; z < 64; z++)
            {
                grass[x * 64 + z] = PrefabUtility.InstantiatePrefab(patch, transform) as GameObject;
                // grass[x * 64 + z] = Instantiate(patch, new Vector3(x, 10, z), new Quaternion(0, 0, 0, 0), transform);
            }
        }
    }*/
    
    public delegate void GenericDelegate();

    public void PlaceGrass(ChunkData chunkData, Vector3 position)
    {
        transform.position = position;
        StartCoroutine(PlaceGrassRoutine(chunkData));
    }

    private IEnumerator PlaceGrassRoutine(ChunkData chunkData)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        
        for (int i = 0; i < grass.Length; i++)
        {
            if (timer.ElapsedMilliseconds > 3)
            {
                // Debug.Log("Skip");
                yield return null;
                timer.Reset();
                timer.Start();
            }
            
            var x = Mathf.FloorToInt(i / 64f) + Random.Range(-grassVariation, grassVariation);
            var z = i % 64 + Random.Range(-grassVariation, grassVariation);
            var height = chunkData.GetHeight(x, z);
            var moisture = chunkData.GetMoisture(x, z);

            if (height + Random.Range(-minGrassHeight/2, 0) < minGrassHeight + Random.Range(0f, 0.2f) || moisture < 0.45) 
                grass[i].SetActive(false);
            else if (chunkData.GetSlope(x, z) + Random.Range(-5, 5) > 37)
                grass[i].SetActive(false);
            else
            {
                grass[i].SetActive(true);
                grass[i].transform.position = new Vector3(
                    transform.position.x + x + Random.Range(-0.25f, 0.25f), 
                    height + Random.Range(-0.1f, 0.1f), 
                    transform.position.z + z + Random.Range(-0.25f, 0.25f));
                
                // Working for upVector
                // /*
                // grass[i].transform.rotation = Quaternion.Euler(0,Random.Range(0,360),0);
                // */
                
                // Working for completely random rotation
                grass[i].transform.up = chunkData.GetNormal(x, z);
                grass[i].transform.Rotate(0, Random.Range(0,360), 0);
                // grass[i].transform.rotation *= Quaternion.AngleAxis(Random.Range(0,360), chunkData.GetNormal(x, z));
                
                // Working for completely random rotation
                // grass[i].transform.rotation = Quaternion.LookRotation(Random.insideUnitSphere, Vector3.up);
            }
        }
        timer.Stop();
        yield return null;
    }
    
    public static Quaternion RandomQuaternion(Vector3 upVector)
    {
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = Mathf.Abs(randomDirection.y);
        Quaternion rotation = Quaternion.LookRotation(randomDirection, upVector);
        return rotation;
    }
}
