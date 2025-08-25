using UnityEngine;

public abstract class BaseManager : MonoBehaviour, IManager
{
    public ManagerType type;
    
    // 나중에 안 쓰는 자식이 생기면 IInitializable 인터페이스로 분리해주자
    public abstract void Initialize();
}
