using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddPointUIElement : UsePointUIElement
{
    [Header("Add Point")]
    [SerializeField] private TextMeshProUGUI countDisplay;

    public void SetCount(int count)
    {
        countDisplay.text = count.ToString();
    }
}
