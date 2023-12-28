using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// TODO: Move portions of the ItemInspectorHUD class into this class to make it more modular.
// Also make this class spawn and set it's own UI elements. This will make it easier to create new inspect points.

public class InspectPoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InspectPointUIElement inspectPointUIElement;

    [Header("Inspect Point Settings")]
    [SerializeField] private string inspectPointName;
    public string InspectPointName
    {
        get { return inspectPointName; }
    }

    [SerializeField] private string inpectPointDescription;
    public string InspectPointDescription
    {
        get { return inpectPointDescription; }
    }

    [SerializeField] private PointType inspectPointType;
    public PointType InspectPointType
    {
        get { return inspectPointType; }
    }

    public enum PointType
    {
        Inspector,
        User,
        Consumer,
        Transformer,
        Modifier,
        Adder
    }
}

public class InspectPointUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Basics")]
    [SerializeField] private Button inspectButton;

    [Header("Preview")]
    [SerializeField] private GameObject previewPanel;
    [SerializeField] private TextMeshProUGUI previewTitle;

    [Header("Full Inspect")]
    [SerializeField] private GameObject inspectPanel;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    private bool previewOpen = false;
    private bool inspectOpen = false;

    public void SetPoint(string pointTitle, string pointDescription)
    {
        inspectButton.onClick.AddListener(OnPointClicked);

        SetTitle(pointTitle);
        SetDescription(pointDescription);
    }

    private void SetTitle(string pointTitle)
    {
        previewTitle.text = pointTitle;
        title.text = pointTitle;
    }

    private void SetDescription(string pointDescription)
    {
        description.text = pointDescription;
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
