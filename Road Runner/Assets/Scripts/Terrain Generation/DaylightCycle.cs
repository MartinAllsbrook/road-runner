using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    [SerializeField] private float dayLength;
    [SerializeField] private AnimationCurve fogColorCurve;
    [SerializeField] private AnimationCurve fogIntensityCurve;
    [SerializeField] private AnimationCurve sunCurve;
    [SerializeField] private AnimationCurve moonCurve;
    [SerializeField] private Color fogColor;
    private float _dayTimer;
    private Light sun;
    private Light moon;
    
    private Quaternion _sunRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        _dayTimer = 0; 
        _sunRotation = transform.rotation;
        sun = gameObject.GetComponent<Light>();
        moon = transform.GetChild(0).GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        // Count though day
        if (_dayTimer < dayLength) _dayTimer += Time.deltaTime;
        else _dayTimer = 0;
        var dayPercent = _dayTimer / dayLength;
        
        // Set sun
        var sunPosition = 360 * dayPercent;
        transform.rotation = Quaternion.Euler(new Vector3(sunPosition, -30f, 0f));
        sun.intensity = sunCurve.Evaluate(dayPercent);
        moon.intensity = moonCurve.Evaluate(dayPercent);

        // Set fog
        var fogColorMultiplier = fogColorCurve.Evaluate(dayPercent);
        
        // RenderSettings.fogColor = new Color(fogColor * 0.9f, fogColor * 0.9f, fogColor * 1.1f);
        RenderSettings.fogColor = fogColor * fogColorMultiplier;

        var fogIntensity = fogIntensityCurve.Evaluate(dayPercent);
        RenderSettings.fogDensity = fogIntensity;

    }
}
