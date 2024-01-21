using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PausedUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Slider mouseSensitivityVerticalSlider;
    [SerializeField] private Slider mouseSensitivityHorizontalSlider;

    [Header("Saving")]
    [SerializeField] private Button saveAndEnterLimboButton;
    [SerializeField] private TextMeshProUGUI waitTimeText;
    [SerializeField] private int waitTimeBeforeSaving = 10;

    private void Start()
    {
        saveAndEnterLimboButton.onClick.AddListener(OnSaveAndEnterLimbo);
    }

    private void OnEnable()
    {
        if (CameraController.Instance == null)
            return;

        // Set the mouse sensitivity sliders to the current mouse sensitivity
        mouseSensitivityHorizontalSlider.value = CameraController.Instance.Sensitivity.x;
        mouseSensitivityVerticalSlider.value = CameraController.Instance.Sensitivity.y;
    
        saveAndEnterLimboButton.gameObject.SetActive(false);
        StartCoroutine(ShowSaveCharacterAfterWait());
    }

    private void OnDisable()
    {
        if (CameraController.Instance == null)
            return;

        Vector2 newSensitivity = new Vector2(mouseSensitivityHorizontalSlider.value, mouseSensitivityVerticalSlider.value);
        // Set the new mouse sensitivity
        CameraController.Instance.Sensitivity = newSensitivity;
    }

    private IEnumerator ShowSaveCharacterAfterWait()
    {
        int timeToWait = waitTimeBeforeSaving;

        while (timeToWait > 0)
        {
            waitTimeText.text = timeToWait.ToString();
            yield return new WaitForSeconds(1);
            timeToWait--;
        }

        saveAndEnterLimboButton.gameObject.SetActive(true);
    }

    private void OnSaveAndEnterLimbo()
    {
        CharacterPersistanceManager.Instance.SaveCharacter();

        Inventory.Instance.ClearInventory();

        Player.LocalInstance.EnterLimbo();
    }
}