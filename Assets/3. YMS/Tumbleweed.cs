using UnityEngine;

public class Tumbleweed : MonoBehaviour
{
   
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.left,120f*Time.deltaTime);
    }
}
