using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GlobalItemDictionary;

// TODO: Needs to be renamed to PointItemOption
public class ItemOptionUI : MonoBehaviour 
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button button;

    private StoredItemID _associatedItem;
    public StoredItemID AssociatedItem => _associatedItem;

    public delegate void GenericDelegate<T>(T variable);
    private GenericDelegate<StoredItemID> _callBack;

    public void SetItemOption(StoredItemID item, GenericDelegate<StoredItemID> callBack)
    {
        Sprite sprite = ItemSODictionary[item.UniqueItemID.BaseItemID].UISprite;
        int count = item.UniqueItemID.CounterCount;

        _callBack = callBack;

        itemImage.sprite = sprite;
        countText.text = count.ToString();

        _associatedItem = item;

        button.onClick.AddListener(OnItemOptionClicked);
    }

    private void OnItemOptionClicked()
    {
        _callBack?.Invoke(_associatedItem); // Replacing line below
        //_associatedPoint.SelectOption(_associatedItem);
        Destroy(gameObject);
    }
}
