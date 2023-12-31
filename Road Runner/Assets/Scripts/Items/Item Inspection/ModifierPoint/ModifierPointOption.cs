using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Inventory;

public class ModifierPointOption : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button button;

    private StoredItemID _associatedItem;
    private ModifierPoint _associatedPoint;

    public void SetItemOption(Sprite itemSprite, int count, StoredItemID item, ModifierPoint point)
    {
        itemImage.sprite = itemSprite;
        countText.text = count.ToString();

        _associatedItem = item;
        _associatedPoint = point;

        button.onClick.AddListener(OnItemOptionClicked);
    }

    private void OnItemOptionClicked()
    {
        _associatedPoint.SelectOption(_associatedItem);
        Debug.Log("Item option " + _associatedItem + " on point " + _associatedPoint +  " clicked");
    }
}
