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
    void Fade(Color c)
    {
        color = c;
        color.a = 200f/255f;
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        for (float i = 200f; i >= 0; i -= 5f)
        {
            color.a = i / 255f;
            yield return wait;
        }
    }
}
