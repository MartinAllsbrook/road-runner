using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InspectPointUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Basics")]
    [SerializeField] private Button inspectButton;
    [SerializeField] private string inspectPointName = "Unnamed";

    [Header("Preview")]
    [SerializeField] private GameObject previewPanel;
    [SerializeField] private TextMeshProUGUI previewText;

    [Header("Full Inspect")]
    [SerializeField] private GameObject inspectPanel;
    [SerializeField] private TextMeshProUGUI inspectNameText;

    private bool previewOpen = false;
    private bool inspectOpen = false;

    private void Start()
    {
        inspectButton.onClick.AddListener(OnPointClicked);

        previewText.text = inspectPointName;
        inspectNameText.text = inspectPointName;
    }

    #region Preview

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (previewOpen || inspectOpen)
            return;

        OpenPreview();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!previewOpen)
            return;

        ClosePreview();
    }

    private void OpenPreview()
    {
        previewOpen = true;
        previewPanel.SetActive(true);
    }

    private void ClosePreview()
    {
        previewOpen = false;
        previewPanel.SetActive(false);
    }

    #endregion

    #region Full Inspect
    private void OnPointClicked()
    {
        if (inspectOpen)
        {
            CloseInspect();
        }
        else
        {
            OpenInspect();
        }
    }

    private void OpenInspect()
    {
        ClosePreview();

        inspectOpen = true;
        inspectPanel.SetActive(true);
    }

    private void CloseInspect()
    {
        inspectOpen = false;
        inspectPanel.SetActive(false);
    }

    #endregion


}
