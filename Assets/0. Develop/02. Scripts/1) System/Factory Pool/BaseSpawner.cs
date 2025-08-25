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
        
        // ���Ӿ��� �ƴ� ���� ������ ���� �ϱ� ����
        // Ȯ�强 ���
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
    /// ���丮�� �����ʸ� �������ִ� �Լ�
    /// </summary>
    public abstract void SpawnConnect();
}
