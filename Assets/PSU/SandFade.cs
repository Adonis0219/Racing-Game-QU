using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SandFade : MonoBehaviour
{

    void OnEnable()
    {
        Color tempColor = GetComponent<Image>().color;
        tempColor.a = 200f/255f;
        GetComponent<Image>().color = tempColor;
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
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
