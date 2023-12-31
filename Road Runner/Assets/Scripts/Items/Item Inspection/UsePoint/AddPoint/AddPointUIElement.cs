using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddPointUIElement : UsePointUIElement
{
    [Header("Add Point")]
    [SerializeField] private TextMeshProUGUI countDisplay;


    public override void GenericSet<T>(T point)
    {
        if (!_set)
        {
            base.GenericSet(point);
        }

        AddPoint addPoint = point as AddPoint;

        Debug.Log("Add point: " + addPoint);
        Debug.Log("Add point count: " + addPoint.GetCount());
        SetCount(addPoint.GetCount());
    }

    public void SetCount(int count)
    {
        countDisplay.text = count.ToString();
    }
}
