using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    private Color color;

    private void Awake()
    {
        color = GetComponent<Image>().color;
    }

    public void Excute()
    {
        Color tempColor = GetComponent<Image>().color;
        tempColor.a = 200f/255f;
        GetComponent<Image>().color = tempColor;
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        
        for (float i = 200f; i >= 0; i -= 5f)
        {
            Color tempColor = GetComponent<Image>().color;
            tempColor.a = i / 255f;
            GetComponent<Image>().color = tempColor;
            
            yield return wait;
        }
        
        gameObject.SetActive(false);
    }
}
