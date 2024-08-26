using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    [SerializeField] private float cycleLength = 90f;
    [SerializeField] private float nightPercent = 0.333f;

    [SerializeField] private Light sun;

    // Angles
    [Header("Sunrise/Sunset Angles")]
    [SerializeField] private float sunriseStart = -5f;
    [SerializeField] private float sunriseEnd = 10f;
    [SerializeField] private float sunsetStart = 170f;
    [SerializeField] private float sunsetEnd = 185f;

    private float dayLength;
    private float nightLength;

    private float timeCounter = 0f;
    private float timeOfDay = 0f;

    private void Start()
    {
        nightLength = cycleLength * nightPercent;
        dayLength = cycleLength - nightLength;
    }

    private void Update()
    {
        timeCounter += Time.deltaTime;
        timeCounter %= cycleLength;

        if (timeCounter < dayLength)
        {
            timeOfDay = timeCounter / dayLength;
        }
        else
        {
            timeOfDay = 1f + (timeCounter - dayLength) / nightLength;
        }

        timeOfDay = timeOfDay / 2f + 0.25f;
        timeOfDay %= 1f;

        SetSun(timeOfDay);
    }

    private void SetSun(float percentThroughCycle)
    {
        float angle = Mathf.Lerp(-90, 270, percentThroughCycle);
        sun.transform.localRotation = Quaternion.Euler(new Vector3(angle, 0, 0));

        float intensityMultiplier = 0;
        if (angle > sunriseEnd && angle < sunsetStart)
        {
            intensityMultiplier = 1;
        }
        else if (angle > sunriseStart && angle < sunriseEnd)
        {
            intensityMultiplier = Mathf.InverseLerp(sunriseStart, sunriseEnd, angle);
        }
        else if (angle > sunsetStart && angle < sunsetEnd)
        {
            intensityMultiplier = Mathf.InverseLerp(sunsetEnd, sunsetStart, angle);
        }

        sun.intensity = intensityMultiplier;
    }
}
