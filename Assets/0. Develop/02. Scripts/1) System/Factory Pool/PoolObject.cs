using UnityEngine;

public class PoolObject : MonoBehaviour
{
    [SerializeField]
    PoolObjectType poolType; // 풀 오브젝트 타입

    public readonly int[] spawnPosX = new int[5] { -2, -1, 0, 1, 2};

    protected virtual void ReturnPool()
    {
        // 풀 매니저에 반환
        PoolManager.instance.SetPool(gameObject, poolType);
    }
}