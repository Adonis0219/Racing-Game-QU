using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleFade : MonoBehaviour
{
    Color color;
    private void Awake()
    {
        color = GetComponent<Image>().color;
    }

    private void OnEnable()
    {
        Show(color);
    }

    void Show(Color c)
    {
        Color tempColor = color;
        tempColor.a = 200f/255f;
        color = tempColor;
        StartCoroutine(Fade());
    }
    
    IEnumerator Fade()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        for (float i = 200f; i >= 0; i -= 5f)
        {
            Color tempColor = color;
            tempColor.a = i / 255f;
            color = tempColor;
            yield return wait;
        }
    }
}
