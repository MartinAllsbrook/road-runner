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
    #region References
    [Header("Basics")]
    [SerializeField] private Button inspectButton;

    [Header("Preview")]
    [SerializeField] private GameObject previewPanel;
    [SerializeField] private TextMeshProUGUI previewTitle;

    [Header("Full Inspect")]
    [SerializeField] private GameObject inspectPanel;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    #endregion

    private bool previewOpen = false;
    private bool inspectOpen = false;

    protected bool _set = false;

    //public virtual void GenericSet<T>(T point)
    //{
    //    InspectPoint inspectPoint = point as InspectPoint;
    //    SetInspectPoint(inspectPoint.InspectPointName, inspectPoint.InspectPointDescription);

    //    _set = true;
    //}

    // TODO: IDK if we should just call SetInspectPoint 
    public virtual void SetBasics(string pointTitle, string pointDescription)
    {
        SetInspectPoint(pointTitle, pointDescription);
    }

    private void SetInspectPoint(string pointTitle, string pointDescription)
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
