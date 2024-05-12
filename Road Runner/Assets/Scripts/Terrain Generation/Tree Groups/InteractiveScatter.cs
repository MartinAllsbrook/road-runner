using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ScatterAddress
{
    public int groupIndex;
    public int treeIndex;

    public ScatterAddress(int groupIndex, int treeIndex)
    {
        this.groupIndex = groupIndex;
        this.treeIndex = treeIndex;
    }
}

public class InteractiveScatter : MonoBehaviour
{
    private ScatterAddress scatterAddress;

    public void SetScatterAddress(int treeGroup, int treeIndex)
    {
        scatterAddress = new ScatterAddress(treeGroup, treeIndex);
    }

    public ScatterAddress GetScatterAddress()
    {
        return scatterAddress;
    }

    public virtual void Interact()
    {
        ConsumeScatter();
    }

    private void ConsumeScatter()
    {
        TreeManager.Instance.ConsumeScatter(scatterAddress);
    }

    public virtual void ConsumeAction()
    {

    }

    public void ReactivateScatter()
    {
        TreeManager.Instance.ReactivateScatter(scatterAddress);
    }

    public virtual void ReactivateAction()
    {

    }
}
