using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedInventoryHeader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Button rootButton;

    private int _inventoryKey;

    public Button GetButton()
    {
        return rootButton;
    }

    public void Set(int inventoryKey, string header, Vector2Int dimensions)
    {
        headerText.text = header;
        _inventoryKey = inventoryKey;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = dimensions;
    }
}
