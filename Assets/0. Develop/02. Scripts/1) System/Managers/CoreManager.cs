using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ManagerType { Data, Audio, Stage, Score, Fuel }

public class CoreManager : MonoBehaviour
{
    public static CoreManager instance;

    [Header("# Managers")]
    [Tooltip("Data, Audio, Stage, Score, Fuel")]
    public BaseManager[] managers;

    private Dictionary<ManagerType, IManager> managerInstances;
    
    [Header("# Main Entity")]
    public Player player;
    
    private void Awake()
    {
        Init();
        PullManager();
    }

    void Init()
    {
        instance = this;

        managerInstances = new Dictionary<ManagerType, IManager>();

        foreach (var manager in managers)
        {
            // ��ųʸ��� �ֱ�
            managerInstances.Add(manager.type, manager);
            // �ְ� �ٷ� �ʱ�ȭ
            manager.Initialize();
        }
    }

    void PullManager()
    {
        // Linq ����
        // FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None) : MonoBehaviour�� ������ ��� ��ü�� ã��
        // FindObjectsInactive.Include : �����ִ� ������Ʈ�� ���� (PoolManager ��ֹ�)
        // ��ֹ��� �׳� Enable �� �� PullUseManager ���� ȣ��
        // FindObjectsSortMode.None : ���� �� ��
        // .OfType<T>() : LINQ �޼���� ���׸� T�� ��ġ�ϴ� �伭�� ���͸��Ͽ� ��ȯ
        // .ToArray() : IEnumerable<TResult>�� ��� ���纻�� �����ϴ� �迭�� ��ȯ
        IPullManager[] pulledManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IPullManager>().ToArray();
        
        foreach (var manager in pulledManagers)
        {
            manager.PullUseManager();
        }
    }

    // ���׸� T�� class, IManager�� �� �� ���� ���·θ� ����
    /// <summary>
    /// CoreManager.instance.managerInstances�� �����ϸ� IManager ���·� ��ȯ�ȴ�.
    /// �̿� ���� �߰����� ����ȯ�� ���ϱ� ����
    /// </summary>
    /// <typeparam name="T">��ȯ���� �Ŵ��� Ÿ��</typeparam>
    /// <returns>��ȯ���� �Ŵ���</returns>
    public T GetManager<T>() where T : class, IManager
    {
        // Values : Dictionary �÷��ǿ� ����� ��� ���� �������� �Ӽ�
        // FirstOrDefalut : Ű�� �ش��ϴ� ù��° �� �Ǵ� �⺻���� �������ش� -> ���� �������� ������ null
        // m => m is T : TŸ���� IManager m�� ��ȯ (is ����ȯ : ����ȯ ���� �Ұ��� ����)
        // IManager as T -> �־��� ���׸� ������ IManager ����ȯ (as ����ȯ : �����ϸ� ĳ���� �Ұ����ϸ� null)
        return managerInstances.Values.FirstOrDefault(m => m is T) as T;
    }
}