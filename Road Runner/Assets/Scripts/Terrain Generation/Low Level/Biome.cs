using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] 
[CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/Biome")]
public class Biome : ScriptableObject
{
    [SerializeField] public float height;
    [SerializeField] public float moisture;
    [SerializeField] public float strangeness;
    [SerializeField] private TreeGroup treeGroup;

    public string name;

    private Vector3 splot;
    
    // Desity Map?
    
    // Biome controlling maps
    // Height
    // Moisture
    // Strange

    public Biome()
    {
        splot = new Vector3(height, moisture, strangeness);
    }


    public Vector3 GetSplot()
    {
        return new Vector3(height, moisture, strangeness);
    }
    
    public TreeGroup GetTreeGroup()
    {
        return treeGroup;
    }
}
