using UnityEngine;

public class MapObjLoop : MonoBehaviour, IFall
{
    public float DownSpeed { get; set; }

    public void MoveDown()
    {
        transform.position += Vector3.down * 7f * Time.deltaTime;
    }

    void Update()
    {
        MoveDown();
        if (transform.position.y <= -10f)
        {
            transform.position=new Vector3(transform.position.x,30,transform.position.z);
        }
    }
}
