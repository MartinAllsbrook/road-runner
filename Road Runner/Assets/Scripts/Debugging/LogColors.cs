using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogColors : MonoBehaviour
{
    public static string PlayerColor = "#0000ffff";
    public static string SaveAndLoadColor = "#ffff00ff";
    public static string InventoryColor = "#00ff00ff";

    public static string GetColoredTag(string tag, string color)
    {
        return "<color=" + color + ">" + tag + "</color>";
    }
}
