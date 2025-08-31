using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    private Image fadeImage;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.raycastTarget = false;
    }
    
    private void OnFade(float t)
    {
        StartCoroutine(FadeRoutine(t));
    }

    IEnumerator FadeRoutine(float fadeTime)
    {
        float timer = 0f;
        float percent = 0f;
        while (percent < 1f)
        {
            timer += Time.deltaTime;
            percent = timer / fadeTime;

            float fadeValue = 1 - percent;

            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeValue);

            yield return null;
        }
    }
}