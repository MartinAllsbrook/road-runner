using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausedUI : MonoBehaviour
{
    [SerializeField] private Slider mouseSensitivityVerticalSlider;
    [SerializeField] private Slider mouseSensitivityHorizontalSlider;

    private void OnEnable()
    {
        if (CameraController.Instance == null)
            return;

        // Set the mouse sensitivity sliders to the current mouse sensitivity
        mouseSensitivityHorizontalSlider.value = CameraController.Instance.Sensitivity.x;
        mouseSensitivityVerticalSlider.value = CameraController.Instance.Sensitivity.y;
    }

    private void OnDisable()
    {
        if (CameraController.Instance == null)
            return;

        Vector2 newSensitivity = new Vector2(mouseSensitivityHorizontalSlider.value, mouseSensitivityVerticalSlider.value);
        // Set the new mouse sensitivity
        CameraController.Instance.Sensitivity = newSensitivity;
    }
}

