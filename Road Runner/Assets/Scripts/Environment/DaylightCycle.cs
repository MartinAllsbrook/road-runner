using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DaylightCycle : MonoBehaviour
{
    public class TimeOfDay
    {
        public float Percent;
        public float SunAngle;
        public float Hour;
    }

    [SerializeField] private float cycleLength = 90f;
    [SerializeField] private float nightPercent = 0.333f;

    [SerializeField] private Light sun;

    // Angles
    [Header("Sunrise/Sunset Angles")]
    [SerializeField] private float sunriseStart = -5f;
    [SerializeField] private float sunriseEnd = 10f;
    [SerializeField] private float sunsetStart = 170f;
    [SerializeField] private float sunsetEnd = 185f;

    public UnityEvent OnDayStart;
    public UnityEvent OnNightStart;

    private float dayLength;
    private float nightLength;

    private float timeCounter = 0f;
    
    // Time of day formats
    private float timeOfDay = 0f;

    private float percentDay = 0f;
    public float PercentDay
    {
        get { return percentDay; }
    }

    private bool isDay = false;
    public bool IsDay
    {
        get { return isDay; }
        private set
        {
            if (isDay != value)
            {
                isDay = value;
                if (isDay)
                {
                    OnDayStart.Invoke();
                    //Debug.Log("Day Start");
                }
                else
                {
                    OnNightStart.Invoke();
                    //Debug.Log("Night Start");
                }
            }
        }
    }

    private void Awake()
    {
        OnDayStart = new UnityEvent();
        OnNightStart = new UnityEvent();
    }

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

        percentDay = 0;
        if (angle > sunriseEnd && angle < sunsetStart)
        {
            percentDay = 1;
        }
        else if (angle > sunriseStart && angle < sunriseEnd)
        {
            percentDay = Mathf.InverseLerp(sunriseStart, sunriseEnd, angle);
            IsDay = true;
        }
        else if (angle > sunsetStart && angle < sunsetEnd)
        {
            percentDay = Mathf.InverseLerp(sunsetEnd, sunsetStart, angle);
            IsDay = false;
        }
        else if (angle < sunriseStart || angle > sunsetEnd)
        {
            percentDay = 0;
        }

        sun.intensity = percentDay;
    }
}
