using UnityEngine;
using UnityEngine.UI;

public class ScrollLine : MonoBehaviour
{
    [SerializeField]
    RawImage image;
    [SerializeField]
    float speed = 1;
    Rect offSet;
    private void Start()
    {
        offSet = new Rect(image.uvRect);
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.CurScene == SceneType.Main) return;
        
        offSet.y += Time.deltaTime * speed;
        image.uvRect = offSet;
    }
}
