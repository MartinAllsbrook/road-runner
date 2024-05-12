using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TreeManager : NetworkBehaviour
{
    public static TreeManager Instance;
    
    private TreeGroup[] baseTreeGroups; // To be instantiated
    private TreeGroup[] availableTreeGroups; // Tree groups with available trees
    private List<TreeGroup> generatedTreeGroupsList; // Tree groups that have been generated
    //private TreeGroup[] treeGroups; // Tree groups that have been generated stored in an array for easy access

    private int nextTreeGroupIndex = 0;

    public async void Initialize(Biome[] biomes)
    {
        if (Instance == null)
            Instance = this;

        baseTreeGroups = new TreeGroup[biomes.Length];
        availableTreeGroups = new TreeGroup[biomes.Length];
        generatedTreeGroupsList = new List<TreeGroup>();

        for (int i = 0;i < biomes.Length;i++)
        {
            baseTreeGroups[i] = Instantiate(biomes[i].GetTreeGroup(), transform);
            baseTreeGroups[i].GenerateTreeGroup();
        }

        for (int i = 0; i < baseTreeGroups.Length; i++)
        {
            CreateNewTreeGroup(i);
        }
    }

    private TreeGroup CreateNewTreeGroup(int biomeIndex)
    {
        TreeGroup newTreeGroup = Instantiate(baseTreeGroups[biomeIndex], transform);
        
        availableTreeGroups[biomeIndex] = newTreeGroup;        
        generatedTreeGroupsList.Add(newTreeGroup);

        availableTreeGroups[biomeIndex].TreeGroupIndex = nextTreeGroupIndex;
        nextTreeGroupIndex++;

        return newTreeGroup;
    }

    public void PlaceTree(Vector3 position, Quaternion rotation, int biomeIndex)
    {
        InteractiveScatter tree = availableTreeGroups[biomeIndex].GetTree();

        if (tree)
        {
            tree.transform.rotation = rotation;
            tree.transform.position = position;
        }
        else // Make new tree group to place new trees
        { 
            TreeGroup newTreeGroup = CreateNewTreeGroup(biomeIndex);
            tree = newTreeGroup.GetTree();
            tree.transform.rotation = rotation;
            tree.transform.position = position;
        }

    }

    public void ConsumeScatter(ScatterAddress scatterAddress)
    {
        ConsumeScatterServerRPC(scatterAddress.groupIndex, scatterAddress.treeIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ConsumeScatterServerRPC(int groupIndex, int scatterIndex)
    {
        ConsumeScatterClientRPC(groupIndex, scatterIndex);
    }

    [ClientRpc]
    private void ConsumeScatterClientRPC(int groupIndex, int scatterIndex)
    {
        InteractiveScatter scatter = generatedTreeGroupsList[groupIndex].Scatter[scatterIndex];
        
        scatter.ConsumeAction();
    }

    public void ReactivateScatter(ScatterAddress scatterAddress)
    {
        ReactivateScatterServerRPC(scatterAddress.groupIndex, scatterAddress.treeIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReactivateScatterServerRPC(int groupIndex, int scatterIndex)
    {
        ReactivateScatterClientRPC(groupIndex, scatterIndex);
    }

    [ClientRpc]
    private void ReactivateScatterClientRPC(int groupIndex, int scatterIndex)
    {
        InteractiveScatter scatter = generatedTreeGroupsList[groupIndex].Scatter[scatterIndex];
        
        scatter.ReactivateAction();
    }

}
