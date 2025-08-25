using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolObjectType
{
    O_Tree, O_Rock,
    I_SpeedUp, I_Recovery,
    P_Explosion
}

[Serializable]
public class PoolObjectData
{
    public PoolObjectType type;
    public int initCount;
    public GameObject original;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    [SerializeField]    
    public PoolObjectData[] poolObjDatas; 
    
    Dictionary<PoolObjectType, Queue<GameObject>> objectDict;
    
    Transform[] parents;

    private void Awake()
    {
        instance = this;

        objectDict = new Dictionary<PoolObjectType, Queue<GameObject>>();

        CreateParents();
        InitPoolSet();  
    }
    
    void InitPoolSet()
    {
        foreach (var data in poolObjDatas)
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            objectDict[data.type] = newQueue;

            for (int i = 0; i < data.initCount; i++)
            {
                newQueue.Enqueue(CreateObject(data));
            }
        }
    }
    
    void CreateParents()
    {
        parents = new Transform[poolObjDatas.Length];

        for (int i = 0; i < poolObjDatas.Length; i++)
        {
            GameObject parent = new GameObject(poolObjDatas[i].type.ToString());
            parent.transform.SetParent(transform);

            parents[i] = parent.transform;
        }
    }
    
    GameObject CreateObject(PoolObjectData data)
    {
        int createIndex = (int)data.type;
        
        GameObject obj = Instantiate(data.original, parents[createIndex]);
        obj.name = data.type.ToString();
        obj.SetActive(false);

        return obj;
    }
    
    public GameObject GetPool(PoolObjectType type)
    {
        if (!objectDict.ContainsKey(type))
        {
            Debug.LogWarning($"PoolManager : {type}�� ���� Ǯ ������ �����ϴ�");
            return null;
        }
        
        var queue = objectDict[type];
        GameObject returnObj;
       
        if (queue.Count == 0)
        {
            var data = poolObjDatas[(int)type];
            returnObj = CreateObject(data);
        }
        else
        {
            returnObj = queue.Dequeue();
        }

        returnObj.SetActive(true);
        
        Debug.Log(queue.Count, returnObj);

        return returnObj;
    }

    public void SetPool(GameObject setObj, PoolObjectType type)
    {
        if (!objectDict.ContainsKey(type))
        {
            Debug.LogWarning($"PoolManager: {type}�� ���� Ǯ ������ �����ϴ�.");
            return; 
        }

        setObj.transform.ResetForce();

        setObj.SetActive(false);
        objectDict[type].Enqueue(setObj);
    }

    public void ClearPool()
    {
        foreach (var parent in parents)
        {
            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
