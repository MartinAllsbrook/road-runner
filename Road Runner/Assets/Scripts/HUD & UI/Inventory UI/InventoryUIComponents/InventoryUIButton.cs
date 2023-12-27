using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIButton : MonoBehaviour
{
    [SerializeField] protected Button button;
    [SerializeField] protected RectTransform rectTransform;

    protected void AddListener()
    {
        button.onClick.AddListener(OnClick);
    }

    protected void RemoveListener()
    {
        button.onClick.RemoveAllListeners();
    }

    protected void StyleRect(Vector2Int position, Vector2Int dimensions)
    {
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = dimensions;
    }

    protected virtual void OnClick()
    {

    }
}
