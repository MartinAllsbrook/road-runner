using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovementHUD : MonoBehaviour
{
    public static MovementHUD Instance;
    
    [SerializeField] private TextMeshProUGUI speedDisplay;
    [SerializeField] private TextMeshProUGUI maxMoveSpeedDisplay;
    [SerializeField] private GameObject groundedDisplay;
    [SerializeField] private GameObject onSlopeDisplay;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SetSpeedDisplay(float speed)
    {
        speedDisplay.text = speed.ToString();
    }

    public void SetMaxMoveSpeedDisplay(float maxMoveSpeed)
    {
        maxMoveSpeedDisplay.text = maxMoveSpeed.ToString();
    }

    public void SetGroundedDisplay(bool grounded)
    {
        groundedDisplay.SetActive(grounded);
    }
    
    public void SetOnSlopeDisplay(bool onSlope)
    {
        onSlopeDisplay.SetActive(onSlope);
    }
}
