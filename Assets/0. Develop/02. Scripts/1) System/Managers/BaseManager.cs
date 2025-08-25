using UnityEngine;

public abstract class BaseManager : MonoBehaviour, IManager
{
    public ManagerType type;
    
    // ���߿� �� ���� �ڽ��� ����� IInitializable �������̽��� �и�������
    public abstract void Initialize();
}
