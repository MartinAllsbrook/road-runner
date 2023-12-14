using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshaireController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image crosshaireImage;

    [Header("Settings")]
    [SerializeField] private float baseScale = 1f;
    [SerializeField] private float scaleIncreaseCefficient = 5f;
    [SerializeField] private Color baseColor = Color.green;
    [SerializeField] private float circleFadeOutInaccuracy = 1.5f;

    public void SetCrosshaireInaccuracy(float inaccuracy)
    {
        float scale = (1 + inaccuracy * scaleIncreaseCefficient) * baseScale;
        crosshaireImage.rectTransform.localScale = Vector3.one * scale;

        float alpha = circleFadeOutInaccuracy - inaccuracy;
        alpha = Mathf.Clamp(alpha, 0, 1);
        Color color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        crosshaireImage.color = color;
    }
}
