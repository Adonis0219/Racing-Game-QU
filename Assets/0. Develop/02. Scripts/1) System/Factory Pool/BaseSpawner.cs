using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseSpawner : MonoBehaviour
{
    protected float[] spawnPosX;

    [SerializeField]
    protected ObjectFactory _factory;

    protected abstract int SpawnTime { get; set; }

    Coroutine coru;

    protected void Start()
    {
        GameManager.instance.onSceneChange += HandleSceneChange;
    }

    private void OnDisable()
    {
        GameManager.instance.onSceneChange -= HandleSceneChange;
    }

    void HandleSceneChange(SceneType type)
    {
        if (type == SceneType.Game)
        {
            coru = StartCoroutine(SpawnRoutine());
        }
        
        // 게임씬이 아닐 때는 무조건 중지 하기 위해
        // 확장성 고려
        else if (coru != null)
        {
            StopCoroutine(coru);
            coru = null;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnTime);

            SpawnConnect();
        }
    }

    /// <summary>
    /// 팩토리에 스포너를 연결해주는 함수
    /// </summary>
    public abstract void SpawnConnect();
}
