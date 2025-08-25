using System;
using UnityEngine;

public class ItemSH : PoolObject, IFall
{
    [SerializeField]
    float downSpeed;

    public float DownSpeed // 아이템이 내려오는 속도
    {
        get => downSpeed; 
        set => downSpeed = value;
    }         

    void Update()
    {
        MoveDown();
        MoveEffect();
    }
    
    private float verticalMoveSpeed = 2.5f;
    private float moveHeight = .2f;
    private float rotSpeed = 40f;
    
    void MoveEffect()
    {
        // 위 아래 움직임
        float newY = transform.position.y + Mathf.Sin(Time.deltaTime * verticalMoveSpeed) * moveHeight;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // 회전
        transform.Rotate(Vector3.forward, rotSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 아이템 효과 발동
        Debug.Log($"{this.name} 획득");
        ReturnPool();
    }

    public void MoveDown()
    {
        transform.Translate(DownSpeed * Time.deltaTime * Vector3.down, Space.World);

        if (transform.position.y <= -7f)
            ReturnPool();
    }
}
