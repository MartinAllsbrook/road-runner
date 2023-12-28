using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectPoint : MonoBehaviour
{
    [SerializeField] private string inspectPointName;
    public string InspectPointName
    {
        get { return inspectPointName; }
        private set { inspectPointName = value; }
    }

    [SerializeField] private string inpectPointDescription;
    public string InspectPointDescription
    {
        get { return inpectPointDescription; }
        private set { inpectPointDescription = value; }
    }

    [SerializeField] private PointType inspectPointType;
    public PointType InspectPointType
    {
        get { return inspectPointType; }
        private set { inspectPointType = value; }
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
