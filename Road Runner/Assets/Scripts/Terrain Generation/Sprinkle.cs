using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkle : MonoBehaviour
{
    [SerializeField] int flatRadius;
    [SerializeField] int blendRadius;

    public int FlatRadius
    {
        get { return flatRadius; }
        private set { }
    }

    public int BlendRadius
    {
        get { return blendRadius; }
        private set { }
    }
}
