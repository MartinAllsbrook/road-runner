using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGroup : MonoBehaviour
{
    [SerializeField] private int numTrees;
    [SerializeField] private InteractiveScatter[] treesToPlace;

    [SerializeField] private InteractiveScatter[] trees; // TODO: make private / remove serialize field
    public InteractiveScatter[] Scatter
    {
        get { return trees; }
    }

    private int treeIndex = -1;

    private int treeGroupIndex = -1;
    public int TreeGroupIndex
    {
        set { treeGroupIndex = value; }
    }
    
    public InteractiveScatter GetTree()
    {
        if (treeIndex < numTrees-1)
        {
            // GameObject tree = ;
            treeIndex++;
            InteractiveScatter tree = trees[treeIndex];
            
            tree.gameObject.SetActive(true);
            tree.SetScatterAddress(treeGroupIndex, treeIndex);

            return tree;
        }

        return null;
    }

    public void GenerateTreeGroup()
    {
        int treeIndex = 0;
        trees = new InteractiveScatter[numTrees];
        
        for (int i = 0; i < numTrees; i++)
        {
            trees[i] = Instantiate(treesToPlace[treeIndex], transform);
            trees[i].gameObject.SetActive(false);

            if (treeIndex < treesToPlace.Length - 1)
                treeIndex++;
            else
                treeIndex = 0;
        }
    }
}
