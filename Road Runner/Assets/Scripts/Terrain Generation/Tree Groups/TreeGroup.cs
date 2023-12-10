using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGroup : MonoBehaviour
{
    [SerializeField] private int numTrees;
    [SerializeField] private GameObject[] treesToPlace;

    [SerializeField] private GameObject[] trees; // TODO: make private / remove serialize field

    private int _treeIndex = -1;
    
    public GameObject GetTree()
    {
        if (_treeIndex < numTrees-1)
        {
            // GameObject tree = ;
            _treeIndex++;
            return trees[_treeIndex];
        }

        return null;
    }

    public void GenerateTreeGroup()
    {
        int treeIndex = 0;
        trees = new GameObject[numTrees];
        
        for (int i = 0; i < numTrees; i++)
        {
            trees[i] = Instantiate(treesToPlace[treeIndex], transform);
            trees[i].SetActive(false);

            if (treeIndex < treesToPlace.Length - 1)
                treeIndex++;
            else
                treeIndex = 0;
        }
    }
}
