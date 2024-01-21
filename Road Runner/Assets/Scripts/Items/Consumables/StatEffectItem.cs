using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class StatEffectItem : ConsumableItem
{
    [SerializeField] private int deltaFood;
    [SerializeField] private int deltaWater;
    [SerializeField] private int deltaHealth;
    public override void OnUseItemInput()
    {
        if (used)
            return;

        LocalPlayerStats.Instance.ChangeFood(deltaFood);
        LocalPlayerStats.Instance.ChangeWater(deltaWater);
        LocalPlayerStats.Instance.ChangeHealth(deltaHealth);

        base.OnUseItemInput();

    }
}
