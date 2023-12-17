using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public static TreeManager Instance;
    
    private TreeGroup[] baseTreeGroups;
    private TreeGroup[] treeGroups;

    public void Initialize(Biome[] biomes)
    {
        if (Instance == null)
            Instance = this;

        baseTreeGroups = new TreeGroup[biomes.Length];
        treeGroups = new TreeGroup[biomes.Length];

        for (int i = 0;i < biomes.Length;i++)
        {
            baseTreeGroups[i] = Instantiate(biomes[i].GetTreeGroup(), transform);
            baseTreeGroups[i].GenerateTreeGroup();
        }

        for (int i = 0; i < baseTreeGroups.Length; i++)
        {
            treeGroups[i] = Instantiate(baseTreeGroups[i], transform);
        }
    }

    public void PlaceTree(Vector3 position, Quaternion rotation, int i)
    {
        GameObject tree = treeGroups[i].GetTree();

        if (tree)
        {
            tree.SetActive(true);
            tree.transform.rotation = rotation;
            tree.transform.position = position;
        }
        else // Make new tree group to place new trees
        {
            treeGroups[i] = Instantiate(baseTreeGroups[i].gameObject, transform).GetComponent<TreeGroup>();
            tree = treeGroups[i].GetTree();
            tree.SetActive(true);
            tree.transform.rotation = rotation;
            tree.transform.position = position;
        }
    }
}
