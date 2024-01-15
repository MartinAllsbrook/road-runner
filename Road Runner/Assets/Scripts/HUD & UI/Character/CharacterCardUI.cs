using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCardUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMPro.TextMeshProUGUI characterNameText;

    public void SetUI(string name)
    {
        characterNameText.text = name;
    }
}
