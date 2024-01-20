using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GlobalItemDictionary;

public class ModifierPointOption : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button button;

    private StoredItemID _associatedItem;
    private ModifierPoint _associatedPoint;

    public void SetItemOption(StoredItemID item, ModifierPoint point)
    {
        Sprite sprite = ItemSODictionary[item.UniqueItemID.BaseItemID].UISprite;
        int count = item.UniqueItemID.CounterCount;

        itemImage.sprite = sprite;
        countText.text = count.ToString();

        _associatedItem = item;
        _associatedPoint = point;

        button.onClick.AddListener(OnItemOptionClicked);
    }

    private void OnItemOptionClicked()
    {
        _associatedPoint.SelectOption(_associatedItem);
        Destroy(gameObject);
    }
}
