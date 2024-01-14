using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterData
{
    public string CName;

    // From player stats
    public float Health;
    public float Food;
    public float Water;

    public CharacterData()
    {
        CName = "New Character";
        
        Health = 100;
        Food = 100;
        Water = 100;
    }
}
