using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AmbientAudio : MonoBehaviour
{
    [SerializeField] private DaylightCycle daylightCycle;

    [SerializeField] private AudioSource dayAmbientAudio;
    [SerializeField] private AudioSource nightAmbientAudio;

    private void Start()
    {
        daylightCycle.OnDayStart.AddListener(StartDay);
        daylightCycle.OnNightStart.AddListener(StartNight);
    }

    private void StartDay()
    {
        StartCoroutine(FadeToDay());
        Debug.Log("Day Start");
    }

    IEnumerator FadeToDay()
    {
        dayAmbientAudio.gameObject.SetActive(true);
        dayAmbientAudio.volume = 0;
        dayAmbientAudio.Play();

        Debug.Log(daylightCycle.PercentDay);

        while (daylightCycle.PercentDay < 1)
        {
            Debug.Log(daylightCycle.PercentDay);

            dayAmbientAudio.volume = daylightCycle.PercentDay;
            nightAmbientAudio.volume = 1 - daylightCycle.PercentDay;

            yield return null;
        }

        dayAmbientAudio.volume = 1;
        nightAmbientAudio.volume = 0;

        nightAmbientAudio.gameObject.SetActive(false);
    }

    private void StartNight()
    {
        StartCoroutine(FadeToNight());
        Debug.Log("Night Start");
    }

    IEnumerator FadeToNight()
    {
        nightAmbientAudio.gameObject.SetActive(true);
        nightAmbientAudio.volume = 0;
        nightAmbientAudio.Play();

        Debug.Log(daylightCycle.PercentDay);

        while (daylightCycle.PercentDay > 0)
        {
            Debug.Log(daylightCycle.PercentDay);

            dayAmbientAudio.volume = daylightCycle.PercentDay;
            nightAmbientAudio.volume = 1 - daylightCycle.PercentDay;

            yield return null;
        }

        dayAmbientAudio.volume = 0;
        nightAmbientAudio.volume = 1;

        dayAmbientAudio.gameObject.SetActive(false);
    }


    IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        audioSource.gameObject.SetActive(true);
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = 1;
    }

    IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
        audioSource.gameObject.SetActive(false);
    }
}
