using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class StatEffectItem : ConsumableItem
{
    [SerializeField] private int deltaFood;
    [SerializeField] private int deltaWater;
    [SerializeField] private int deltaHealth;
    public override void UseItem()
    {
        base.UseItem();
        PlayerStats.Instance.ChangeFood(deltaFood);
        PlayerStats.Instance.ChangeWater(deltaWater);
        PlayerStats.Instance.ChangeHealth(deltaHealth);
    }
}
