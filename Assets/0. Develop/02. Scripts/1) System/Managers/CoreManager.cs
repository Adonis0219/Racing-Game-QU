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
            // 딕셔너리에 넣기
            managerInstances.Add(manager.type, manager);
            // 넣고 바로 초기화
            manager.Initialize();
        }
    }

    void PullManager()
    {
        // Linq 공부
        // FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None) : MonoBehaviour를 가지는 모든 객체를 찾자
        // FindObjectsInactive.Include : 꺼져있는 오브젝트도 포함 (PoolManager 장애물)
        // 장애물은 그냥 Enable 할 때 PullUseManager 직접 호출
        // FindObjectsSortMode.None : 정렬 안 함
        // .OfType<T>() : LINQ 메서드로 제네릭 T와 일치하는 요서만 필터링하여 반환
        // .ToArray() : IEnumerable<TResult>의 요소 복사본을 포함하는 배열로 변환
        IPullManager[] pulledManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IPullManager>().ToArray();
        
        foreach (var manager in pulledManagers)
        {
            manager.PullUseManager();
        }
    }

    // 제네릭 T를 class, IManager를 둘 다 가진 형태로만 한정
    /// <summary>
    /// CoreManager.instance.managerInstances로 접근하면 IManager 형태로 반환된다.
    /// 이에 따른 추가적인 형변환을 피하기 위해
    /// </summary>
    /// <typeparam name="T">반환해줄 매니저 타입</typeparam>
    /// <returns>반환해줄 매니저</returns>
    public T GetManager<T>() where T : class, IManager
    {
        // Values : Dictionary 컬렉션에 저장된 모든 값을 가져오는 속성
        // FirstOrDefalut : 키에 해당하는 첫번째 값 또는 기본값을 가져와준다 -> 값이 존재하지 않으면 null
        // m => m is T : T타입의 IManager m을 반환 (is 형변환 : 형변환 가능 불가능 여부)
        // IManager as T -> 넣어준 제네릭 값으로 IManager 형변환 (as 형변환 : 가능하면 캐스팅 불가능하면 null)
        return managerInstances.Values.FirstOrDefault(m => m is T) as T;
    }
}