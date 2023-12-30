using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightToggle : MonoBehaviour
{
    private bool sightUp = true;

    [SerializeField] private Transform sight;
    [SerializeField] private Transform upTransform;
    [SerializeField] private Transform downTransform;

    public void ToggleSight()
    {
        if (sightUp)
        {
            sight.rotation = downTransform.rotation;
            sightUp = false;
        }
        else
        {
            sight.rotation = upTransform.rotation;
            sightUp = true;
        }
    }
}